using System.Collections.Generic;
using UnityEngine;

public class ReplayManager : MonoBehaviour
{
    /// References
    [SerializeField]
    private Brush m_playerBrush;
    [SerializeField]
    private Transform m_playerWayTransform;
    [SerializeField]
    private Brush m_goodWayBrush;
    [SerializeField]
    private Transform m_goodWayTransform;
    [SerializeField]
    private Camera m_replayCameraPrefab;
    private Camera m_replayCameraInstance;

    private GameManagerInfinity m_gm;
    private Difficulty m_difficulty;
    private Player m_player;
    private MyMaze m_maze;
    private InfinityEndMenu m_endMenu;

    /// Attributes
    private bool m_initialized, m_recording, m_replaying;
    private bool m_drawingGoodWay, m_drawingPlayerWay;
    private List<IntVector2> m_goodWay;
    private float m_startTime;
    
    /// Public Methods
    public void Initialize(GameManagerInfinity gm, Difficulty difficulty, Player player, MyMaze maze, InfinityEndMenu endMenu)
    {
        m_gm = gm;
        m_difficulty = difficulty;
        Debug.Log("Replay seed : " + m_difficulty.data.lastSeed);
        m_player = player;
        m_maze = maze;
        m_endMenu = endMenu;
        m_playerBrush.Initialize(m_playerWayTransform);
        m_goodWayBrush.Initialize(m_goodWayTransform);

        m_initialized = true;
    }
    public void StartRecording()
    {
        if (!m_initialized)
            throw new System.InvalidOperationException("ReplayManager n'est pas initialisé");
        if (m_recording)
            throw new System.InvalidOperationException("ReplayManager est déjà en train d'enregistrer la partie.");
        if (m_replaying)
            throw new System.InvalidOperationException("ReplayManager ne peut enregistrer et diffuser en même temps.");
        if (!SaveSystem.DifficultyTracking.isActive)
            throw new System.Exception("(ReplayManager) DifficultyTracking doit être activé pour que l'enregistrement fonctionne.");

        m_startTime = m_gm.startTime;

        m_difficulty.data.continueLastGame = true; // Si jamais le joueur quitte le jeu en pleine partie
        SaveSystem.SaveDifficulty(m_difficulty);

        m_recording = true;
    }
    public void StopRecording()
    {
        //Debug.Log("Stop Recording !");
        m_recording = false;
    }
    public void StartReplaying()
    {
        if (!m_initialized)
            throw new System.InvalidOperationException("ReplayManager n'est pas initialisé");
        if (!SaveSystem.DifficultyTracking.isActive)
            throw new System.Exception("(ReplayManager) DifficultyTracking doit être activé pour que le visionnage fonctionne.");

        // Reset maze
        m_maze.Generate(m_difficulty.data.lastSeed, 1, m_difficulty.configuration.pieceDensity, m_difficulty.data.lastPieceAdvancement);

        // Instantiate a replay camera if it's null
        if (m_replayCameraInstance == null) CreateReplayCamera();

        // Set time
        Time.timeScale = (m_maze.width + m_maze.height) / 6f;

        // Enable brushes
        EnableBrushes();

        // Switch to draw the good way
        SwitchToDrawGoodWay();

        m_replaying = true;
    }
    public void StopReplaying()
    {
        m_drawingGoodWay = false;
        m_drawingPlayerWay = false;
        m_replaying = false;

        SaveSystem.DifficultyTracking.ResetReading();
    }

    /// Private Methods
    private void FixedUpdate() // After all of them, thanks to the Script Execution Order
    {
        if (m_recording)
        {
            UpdateRecording();
        }
        else if (m_replaying)
        {
            UpdateReplaying();
        }
    }
    private void UpdateRecording()
    {
        SaveSystem.DifficultyTracking.Write(new float[7] {
            m_player.transform.position.x,
            m_player.transform.position.z,
            m_player.transform.rotation.eulerAngles.y,
            m_gm.fixedReplayGameTime,
            m_maze.piecesAdvancement,
            m_gm.movesAvancement,
            m_gm.fixedRemainingTimeBeforeMoving
        });
    }
    private void UpdateReplaying()
    {
        if (m_drawingGoodWay)
        {
            // Show start time
            m_endMenu.UpdateReplayRemainingTime(m_startTime);

            // Draw good way
            if (m_goodWay == null || m_goodWay.Count == 0)
            {
                SwitchToDrawPlayerWay();
            }
            else
            {
                m_goodWayBrush.transform.position = m_maze.ToRealPosition(m_goodWay[0], 0.1f);
                //Debug.Log(m_goodWay[0]);
                m_goodWayBrush.DrawingUpdate();
                m_goodWay.RemoveAt(0);
            }
        }
        else if (m_drawingPlayerWay)
        {
            try
            {
                float[] datas = SaveSystem.DifficultyTracking.Read();
                // Load player position
                m_playerBrush.transform.position = new Vector3(datas[0], 0.5f, datas[1]);
                m_playerBrush.transform.rotation = Quaternion.Euler(new Vector3(0, datas[2], 0));
                m_playerBrush.DrawingUpdate();

                // Show replay remaining time
                m_endMenu.UpdateReplayRemainingTime(datas[3]);

                // Update pieces
                m_maze.MoveTimePieces(Mathf.RoundToInt(datas[4]));
                m_maze.PlayerCollisionWithPieces(new Vector2(datas[0], datas[1]));

                // The walls move if it's the case
                int movesAdvancement = (int)datas[5];
                if (movesAdvancement == 0)
                {
                    List<IntVector2> lastGoodWay = m_maze.SearchGoodWay(m_maze.ToMazeCoord(m_playerBrush.transform.position));
                    m_gm.StartMoves(false, new Vector3(datas[0], 0.5f, datas[1]));

                    List<IntVector2> goodWay = m_maze.SearchGoodWay(m_maze.ToMazeCoord(m_playerBrush.transform.position));
                    if (lastGoodWay != goodWay) UpdateGoodWayImmediately(goodWay);
                }
                else if (movesAdvancement > 0)
                {
                    m_gm.UpdateMoves(movesAdvancement, false);
                }
            }
            catch (System.IO.EndOfStreamException)
            {
                StopReplaying();
                StartReplaying(); // Restart replaying
            }
        }
        else
        {
            throw new System.InvalidOperationException("Je ne devrais pas être ici");
        }
    }

    private void CreateReplayCamera()
    {
        m_replayCameraInstance = Instantiate(m_replayCameraPrefab, transform);
        m_replayCameraInstance.transform.localPosition = new Vector3(0, 15, 0);
        m_replayCameraInstance.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));

        // Calculate camera size
        float size = 1.5f * Mathf.Max(m_maze.width, m_maze.height) + 2.5f;
        m_replayCameraInstance.orthographicSize = size;
    }
    private void ResetGoodWay(List<IntVector2> goodWay)
    {
        m_goodWayBrush.Erase();
        m_goodWay = goodWay;
        m_goodWay.Add(m_maze.ArrivalCoord + IntVector2.OneY);
        if (m_goodWay[0] == m_maze.DepartureCoord)
            m_goodWay.Insert(0, m_maze.DepartureCoord + IntVector2.NegativeOneY);

        m_goodWayBrush.transform.position = m_maze.ToRealPosition(m_goodWay[0], 0.1f);
        m_goodWayBrush.SimpleUpdate();
        m_goodWay.RemoveAt(0);
    }
    private void ResetPlayerWay()
    {
        m_playerBrush.Erase();
        m_playerBrush.transform.position = m_maze.ToRealPosition(m_maze.DepartureCoord - IntVector2.OneY, 0.5f);
        m_playerBrush.SimpleUpdate();
    }
    private void UpdateGoodWayImmediately(List<IntVector2> goodWay)
    {
        ResetGoodWay(goodWay);

        foreach (IntVector2 coord in m_goodWay)
        {
            m_goodWayBrush.transform.position = m_maze.ToRealPosition(coord, 0.1f);
            m_goodWayBrush.DrawingUpdate();
        }
    }
    private void SwitchToDrawGoodWay()
    {
        // Reset good way and player way attributes
        ResetGoodWay(m_maze.SearchGoodWay(m_maze.DepartureCoord));
        ResetPlayerWay();

        m_drawingGoodWay = true;
        m_drawingPlayerWay = false;
    }
    private void SwitchToDrawPlayerWay()
    {
        m_drawingGoodWay = false;
        m_drawingPlayerWay = true;
    }
    private void EnableBrushes()
    {
        m_playerBrush.gameObject.SetActive(true);
        m_goodWayBrush.gameObject.SetActive(true);
    }
}
