using System.Collections.Generic;
using UnityEngine;

public class GameManagerInfinity : GameManager
{
    // References
    [SerializeField]
    private MapGenerator m_generator;
    [SerializeField]
    private Player m_playerPrefab;
    private PlayerUI m_ui;
    [SerializeField]
    private InfinityEndMenu m_endMenu;
    [SerializeField]
    private ReplayManager m_replayManager;

    // Settings
    [Header("Settings")]
    private int m_levelOfDetail = 2;
    public const float WAITING_SECONDS_BETWEEN_MOVES = 30f;
    public const float MOVING_SECONDS = 3f;

    // Attributes
    private int m_currentLevel;
    private Difficulty m_difficulty;
    public float remainingTime { get; private set; }
    public float fixedReplayGameTime { get; private set; }
    public float startTime { get => m_map.maze.CalculateMinTime(6) * (m_difficulty.configuration.timeCoefficient + (1 / (3 + m_currentLevel))); } // 6 = playerSpeed
    private bool m_playWithTime;
    public float fixedRemainingTimeBeforeMoving { get; private set; }
    private bool m_isMovingWalls;
    private int m_movesAvancement = -1;
    public int movesAvancement { get => m_movesAvancement; }
    private List<MazeMove> m_currentMoves;

    private bool m_isPlayingGame;
    public bool isPlayingGame { get { return m_isPlayingGame; } }

    private void Awake()
    {
        InitializeGame();
    }
    private void Start()
    {
        StartGame();
    }
    private void Update()
    {
        if (isPlayingGame && m_hasEnteredTheMaze)
        {
            gameTime += Time.deltaTime;

            if (m_playWithTime)
            {
                remainingTime -= Time.deltaTime;

                if (remainingTime <= 0f)
                {
                    FinishGame(Result.TimeOut);
                }
                m_ui.UpdateTime(remainingTime);
            }
        }
    }
    private void FixedUpdate()
    {
        if (isPlayingGame && m_hasEnteredTheMaze)
        {
            if (m_playWithTime)
                fixedReplayGameTime -= Time.fixedDeltaTime;
            else
                fixedReplayGameTime += Time.fixedDeltaTime;
        }

        if (isPlayingGame || m_isDying)
        {
            // Chunks
            m_map.maze.UpdateChunks(m_player.transform.position);

            // Wall moves
            if (kindOfLevel == KindOfLevel.Desert && m_hasEnteredTheMaze)
            {
                if (m_isMovingWalls) // Si les murs bougent
                {
                    UpdateMoves(false);
                }
                else // Si les murs ne bougent pas
                {
                    m_movesAvancement = -1;
                    fixedRemainingTimeBeforeMoving -= Time.fixedDeltaTime;
                    if (fixedRemainingTimeBeforeMoving <= 0f)
                    {
                        StartMoves(true);
                    }
                    else
                    {
                        m_ui.UpdateMovesTime(fixedRemainingTimeBeforeMoving);
                    }
                }
            }

            // Time pieces 
            m_map.maze.MoveTimePieces();
            AddTime(m_map.maze.PlayerCollisionWithPieces(m_player.transform.position));
        }
    }
    private void OnApplicationQuit()
    {
        SaveSystem.DifficultyTracking.Disable();
    }

    /// Game
    protected override void InitializeGame()
    {
        if (instance != null)
            Debug.LogError("Il y a plusieur GameManagerInfinity dans la scène.");
        else
            instance = this;

        // Get last saves
        m_mode = Mode.Infinity;
        m_difficulty = SaveSystem.GetDifficulty(DifficultyManager.currentDifficultyIndex);
        if (m_difficulty.data.currentLevel < 1 || m_difficulty.data.currentLevel > m_difficulty.data.bestLevel + 1)
        {
            m_difficulty.data.currentLevel = 1;
        }
        m_currentLevel = m_difficulty.data.currentLevel;
        m_isPlayingGame = false;
        m_playWithTime = m_difficulty.configuration.timeCoefficient != 0;
        fixedRemainingTimeBeforeMoving = WAITING_SECONDS_BETWEEN_MOVES;
        m_hasEnteredTheMaze = false;
        m_movesAvancement = -1;

        // Create map
        if (!m_difficulty.data.continueLastGame)
        {
            m_difficulty.data.lastSeed = Random.Range(0, 1000000);
            m_difficulty.data.lastPieceAdvancement = Random.Range(0, 1000);
        }

        m_map = m_generator.CreateNewMap(m_difficulty.configuration.startMazeSize + (m_difficulty.configuration.addingMazeSize * (m_currentLevel - 1)), m_difficulty.data.lastSeed, 
            m_levelOfDetail, m_difficulty.configuration.pieceDensity, m_difficulty.data.lastPieceAdvancement);
        m_map.maze.DisableAllChunks();

        // On initialise ça mtn car sinon ça pose pb
        SaveSystem.DifficultyTracking.Enable(m_difficulty);

        // Simulate last game to move the walls to get the right maze's position, get the last player position
        // And to set the attibutes
        Vector3 playerPosition;
        Quaternion playerRotation;
        SimulateLastGame(out playerPosition, out playerRotation);

        // Instantiate the player
        m_player = Instantiate(m_playerPrefab, playerPosition, playerRotation);
        m_player.EnableUI();
        m_player.SetLevel(m_currentLevel);
        m_map.maze.UpdateChunks(playerPosition);

        // Set UI
        m_endMenu.Disable();
        m_ui = m_player.UI;
        m_ui.Enable();
        if (m_playWithTime) m_ui.UpdateTime(remainingTime);
        else m_ui.TimerToInfinity();
        m_ui.SetDifficulty(m_difficulty);

        // Initialize ReplayManager
        m_replayManager.Initialize(this, m_difficulty, m_player, m_map.maze, m_endMenu);
        if (m_hasEnteredTheMaze) m_replayManager.StartRecording();
    }
    public void SimulateLastGame(out Vector3 playerPosition, out Quaternion playerRotation)
    {
        Debug.Log("Je simule partie précedente sans que le joueur le voit.");
        float[] lastDatas = null;
        int cpt = 0;
        try
        {
            while (true) // Sera arrêtée par une exception
            {
                float[] datas = SaveSystem.DifficultyTracking.Read();
                
                if (datas[5]== 0)
                {
                    StartMoves(false, new Vector3(datas[0], 1, datas[1]));
                }
                else if ((lastDatas != null) && lastDatas[5] > datas[5])
                {
                    UpdateMoves(Mathf.RoundToInt(lastDatas[5]), false);
                }
                else if ((lastDatas != null) && lastDatas[5] < datas[5])
                {
                    UpdateMoves(1000, false);
                }
                lastDatas = datas;
                cpt++;
            }
        }
        catch(System.IO.EndOfStreamException)
        {
            if (lastDatas == null)
            {
                // if we start a new game set start attributes
                remainingTime = startTime;
                fixedReplayGameTime = remainingTime;

                playerPosition = m_map.maze.SpawnPosition;
                playerRotation = Quaternion.identity;
            }
            else
            {
                // otherwise we set the attributes to the last situation
                playerPosition = new Vector3(lastDatas[0], 1.08f, lastDatas[1]);
                playerRotation = Quaternion.Euler(new Vector3(0, lastDatas[2], 0));

                if (m_playWithTime)
                {
                    remainingTime = lastDatas[3];
                    fixedReplayGameTime = remainingTime;
                    gameTime = 0;
                }
                else
                {
                    gameTime = lastDatas[3];
                    fixedReplayGameTime = gameTime;
                    remainingTime = 0;
                }

                m_map.maze.MoveTimePieces(Mathf.RoundToInt(lastDatas[4]));

                if (lastDatas[5] >= 0)
                {
                    if (lastDatas[5] == 1)
                    {
                        UpdateMoves(Mathf.RoundToInt(lastDatas[5]), false);
                    }
                    else
                    {
                        UpdateMoves(Mathf.RoundToInt(lastDatas[5]), true);
                    }
                }

                fixedRemainingTimeBeforeMoving = lastDatas[6];

                if (m_map.maze.ToMazeCoord(playerPosition).y >= 0)
                {
                    m_hasEnteredTheMaze = true;
                }

                Debug.Log("J'arrête de simuler la partie : " + cpt);
            }
        }
    }
    public override void StartGame()
    {
        if (!m_isPlayingGame)
        {
            m_isPlayingGame = true;

            RestartTime();
        }
    }
    protected override void StopGame()
    {
        StopAllCoroutines();
        m_isPlayingGame = false;
        m_isDying = false;

        Destroy(m_player.gameObject);
    }

    /// Time.timeScale
    public override void PauseTime()
    {
        Time.timeScale = 0;
    }
    public override void RestartTime()
    {
        Time.timeScale = 1;
    }

    /// End of the game
    public override void FinishGame(Result result)
    {
        m_replayManager.StopRecording();
        m_isPlayingGame = false;

        SaveResult(result);

        if (result == Result.TimeOut || result == Result.Abandon)
        {
            m_isDying = true;
            m_player.controller.CanMove = false;
            remainingTime = 0;
            fixedReplayGameTime = 0;
            if (result == Result.TimeOut) m_ui.TimeOut();
            else m_ui.Abandon();
        }
        else if (result == Result.FinishLevel)
        {
            SwitchToEndMenu(result);
        }
        else if (result == Result.ContinueLater)
        {
            Debug.Log("Go scene because continue later.");
            MySceneManager.LoadScene("InfinityMenu");
        }
        else
        {
            throw new System.InvalidOperationException("I shouldn't be here.");
        }
    }
    public override void SwitchToEndMenu(Result result)
    {
        // Disable all physics and inputs
        StopGame();

        m_endMenu.Enable(result, m_currentLevel, m_difficulty.data.bestLevel, fixedReplayGameTime, m_playWithTime);

        m_replayManager.StartReplaying();
    }

    protected override void SaveResult(Result result)
    {
        if (result != Result.ContinueLater)
        {
            //m_difficulty.lastPlayerPosition = new Vector3(0, -100, 0);
            //m_difficulty.lastGameTime = 0;
            //m_difficulty.lastRemainingTime = 0;
            //m_difficulty.SetMemoryStream(new MemoryStream());

            m_difficulty.data.continueLastGame = false;
            SaveSystem.DifficultyTracking.resetTracking = true;

            if (result == Result.FinishLevel)
            {
                m_difficulty.data.currentLevel++;

                if (m_difficulty.data.bestLevel < m_currentLevel)
                {
                    m_difficulty.data.bestLevel = m_currentLevel;
                }
            }
            else // Result.PlayerDie
            {
                m_difficulty.data.currentLevel = 1;
            }

            SaveSystem.SaveDifficulty(m_difficulty);
        }
        else // Result.ContinueLater
        {
            Debug.Log("Rien !!!!!!!");            // Pour ContinueLater tout est déjà fait,
            // de sorte que si le joueur quitte l'application il pourra tout de même continuer sa partie
        }
    }

    /// Remaining Time
    public void AddTime(float quantity)
    {
        if (m_isPlayingGame && quantity != 0)
        {
            remainingTime += quantity;
            fixedReplayGameTime = remainingTime;
            m_ui.UpdateTime(remainingTime);
            m_ui.addingTimeCoroutine = StartCoroutine(m_ui.AddTime((int)quantity, 1f));
            MySound.PieceSound();
            Vibration.Vibrate((long) (40f * quantity));
        }
    }

    /// Replay
    public override void EnterTheMaze()
    {
        if (!m_hasEnteredTheMaze && isPlayingGame)
        {
            m_replayManager.StartRecording();
            m_hasEnteredTheMaze = true;
        }
    }

    public void StartMoves(bool showItToPlayer)
    {
        StartMoves(showItToPlayer, m_player.transform.position);
    }
    public void StartMoves(bool showItToPlayer, Vector3 playerPosition)
    {
        Debug.Log("Je commence à bouger les murs !");
        m_movesAvancement = 0;
        m_currentMoves = m_map.maze.GetNewMoves(playerPosition);
        m_map.maze.UpdateMoveWalls(m_currentMoves, MOVING_SECONDS, m_movesAvancement);

        m_isMovingWalls = true;

        if (showItToPlayer)
        {
            m_ui.MoveWalls();
            Vibration.Vibrate((long)MOVING_SECONDS * 1000);
        }
    }
    public void UpdateMoves(bool showItToPlayer)
    {
        UpdateMoves(m_movesAvancement + (int)(Time.fixedDeltaTime * 1000 / MOVING_SECONDS), showItToPlayer);
    }
    public void UpdateMoves(int advancement, bool showItToPlayer)
    {
        m_movesAvancement = advancement;
        if (m_movesAvancement >= 1000)
        {
            m_map.maze.UpdateMoveWalls(m_currentMoves, MOVING_SECONDS, 1000);
            m_movesAvancement = 1000;
            m_currentMoves = null;
            m_isMovingWalls = false;
            fixedRemainingTimeBeforeMoving = WAITING_SECONDS_BETWEEN_MOVES;

            if (showItToPlayer)
            {
                Vibration.Vibrate((long)((1000 - advancement) * MOVING_SECONDS));
            }
        }
        else
        {
            m_map.maze.UpdateMoveWalls(m_currentMoves, MOVING_SECONDS, m_movesAvancement);
        }
    }
}
