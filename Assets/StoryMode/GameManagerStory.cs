using UnityEngine;

public class GameManagerStory : GameManager
{
    // References
    [SerializeField]
    private MapGenerator m_generator;
    [SerializeField]
    private Player m_playerPrefab;
    private PlayerUI m_ui;
    [SerializeField]
    private InfinityEndMenu m_endMenu;

    // Attributes
    private Level m_level;
    public float remainingTime { get; private set; }

    private void Awake()
    {
        InitializeGame();
    }
    protected override void InitializeGame()
    {
        /* 
         * Appelée à Awake(), initialise la partie : créer la map, le joueur et met en place tous les détails
         */

        if (instance != null) Debug.LogError("Il y a plusieur GameManagerInfinity dans la scène.");
        else instance = this;

        Debug.Log("Chargement du niveau.");

        // Récupération du niveau qui contient toutes les informations
        if (LevelManager.currentLevel < 1)
        {
            Debug.LogError("Impossible de savoir quel niveau a été lancé.");
            MySceneManager.LoadScene("StoryMenu"); return; // Au cas où
        }
        m_level = SaveSystem.GetLevel(LevelManager.currentLevel);

        // Mise en place des variables
        gameTime = m_level.data.lastGameTime; // Chrono
        remainingTime = m_level.configuration.maxTime - gameTime; // Temps restant

        // Création de la map
        m_map = m_generator.CreateNewMap(m_level.configuration.mazeSize, m_level.configuration.seed, 2, 0, m_level.configuration.pieceAdvancement);
        m_map.maze.DisableAllChunks();
        m_map.maze.UpdateChunks(m_map.SpawnPosition);

        // Création du joueur
        if (m_level.data.continueLastGame)
            m_player = Instantiate(m_playerPrefab, m_level.data.lastPlayerPosition, m_level.data.lastPlayerRotation);
        else
            m_player = Instantiate(m_playerPrefab, m_map.SpawnPosition, Quaternion.identity);
        m_player.EnableUI();
        m_player.SetLevel(m_level.data.number);

        // Création des UI
        m_endMenu.Disable();
        m_ui = m_player.UI;
        m_ui.Enable();
        m_ui.UpdateTime(m_level.configuration.maxTime);
        m_ui.SetStoryLabel();

        // On regarde si le joueur est entré dans le labyrinthe
        m_hasEnteredTheMaze = m_map.maze.ToMazeCoord(m_player.transform.position).y > 0;

        // On active LevelTracking
        SaveSystem.LevelTracking.Enable(m_level);

        Debug.Log("Niveau chargé avec succés.");
    }

    private void Start()
    {
        StartGame();
        Debug.Log("min time : " + m_map.maze.CalculateMinTime(m_player.controller.moveSpeed));
    }
    public override void StartGame()
    {
        /*
         * Appelé à Start(), démarre la partie
         */

        RestartTime();
    }

    private void Update()
    {
        if (m_hasEnteredTheMaze)
        {
            gameTime += Time.deltaTime;
            remainingTime -= Time.deltaTime;

            m_ui.UpdateTime(remainingTime);

            if (remainingTime <= 0)
            {
                FinishGame(Result.TimeOut);
            }
        }
    }
    private void FixedUpdate()
    {
        // Chunks
        m_map.maze.UpdateChunks(m_player.transform.position);

        if (m_hasEnteredTheMaze)
        {
            // LevelTracking
            SaveSystem.LevelTracking.UpdateLevel(m_player.transform.position, m_player.transform.rotation, gameTime);
        }
    }

    public override void PauseTime()
    {
        /* Stop le temps */
        Time.timeScale = 0;
    }
    public override void RestartTime()
    {
        /* Rédemarre le temps */
        Time.timeScale = 1;
    }

    public override void EnterTheMaze()
    {
        if (!m_hasEnteredTheMaze)
        {
            m_level.data.continueLastGame = true; // Si jamais le joueur quitte le jeu en pleine partie
            SaveSystem.SaveLevel(m_level);
            m_hasEnteredTheMaze = true;
        }
    }
    public override void FinishGame(Result result)
    {
        // On sauvegarde tout d'abord le résultat de la partie
        SaveResult(result);

        // Si le joueur a perdu, on l'empêche de bouger et on affiche le menu time out / abandon
        if (result == Result.TimeOut || result == Result.Abandon)
        {
            m_player.controller.CanMove = false;
            remainingTime = 0;
            if (result == Result.TimeOut) m_ui.TimeOut();
            else m_ui.Abandon();
        }
        // Sinon le joueur va au menu du mode histoire
        else if (result == Result.FinishLevel || result == Result.ContinueLater)
        {
            SwitchToEndMenu(result);
        }
        else
        {
            throw new System.InvalidOperationException("I shouldn't be here.");
        }
    }
    protected override void StopGame()
    {
        throw new System.NotImplementedException();
    }
    public override void SwitchToEndMenu(Result result)
    {
        MySceneManager.LoadScene("StoryMenu");
    }

    protected override void SaveResult(Result result)
    {
        float lastBestTime = m_level.data.bestTime;

        if (result == Result.ContinueLater)
        {
            // Rien car tt est fait en sorte pour que le jouer puisse fermer le jeu
            // sans que sa progression soit perdue
        }
        else
        {
            m_level.data.lastPlayerPosition = new Vector3(0, -100, 0);
            m_level.data.lastPlayerRotation = Quaternion.identity;
            m_level.data.lastGameTime = 0;
            m_level.data.continueLastGame = false;

            if (result == Result.FinishLevel)
            {
                m_level.data.isFinish = true;
                m_level.data.finishedAttempts++;

                if (m_level.data.bestTime == 0 || gameTime < m_level.data.bestTime)
                {
                    m_level.data.bestTime = gameTime;
                }
            }

            SaveSystem.SaveLevel(m_level);
        }
        LevelManager.SetResult(m_level.data.number, result, gameTime, lastBestTime);
    }
    private void OnApplicationQuit()
    {
        SaveSystem.LevelTracking.Disable();
    }
}
