using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // Custom Editor --> MazeGeneratorEditor

    [Header("Editor")]
    [SerializeField]
    private bool m_generateTerrain = true;
    [SerializeField]
    private bool m_generateMaze = true;
    [SerializeField]
    private bool m_autoUpdateTerrain = false;
    [SerializeField]
    private bool m_autoUpdateMaze = false;
    [SerializeField]
    private bool m_updateOnStart = false;

    [Header("Settings")]
    [SerializeField]
    private IntVector2 m_mazeSize = new IntVector2(3, 3);
    public IntVector2 MazeSize{ get { return m_mazeSize; } set { m_mazeSize = value; } }
    [SerializeField]
    private int m_seed = 0;
    public int Seed { get { return m_seed; } set { m_seed = value; } }
    [SerializeField]
    private int m_levelOfDetail = 2;
    public int LevelOfDetail { get { return m_levelOfDetail; } set { m_levelOfDetail = value; } }
    [SerializeField]
    private float m_pieceDensity;
    public float PieceDensity { get { return m_pieceDensity; } set { m_pieceDensity = value; } }
    [SerializeField]
    private int m_pieceAdvancement = 0;
    public int pieceAdvancement { get => m_pieceAdvancement; }

    [Header("References")]
    [SerializeField]
    private MyMap m_mapPrefab;
    [SerializeField]
    private MyMap m_mapInstance;

    // Method that generate a new map in the scene
    private void GenerateMap()
    {
        // Verify attributes
        VerifyAttributes();

        // CreateMap
        m_mapInstance.generateTerrain = m_generateTerrain;
        m_mapInstance.generateMaze = m_generateMaze;
        m_mapInstance.Create(m_mazeSize, m_seed, m_levelOfDetail, m_pieceDensity, m_pieceAdvancement);
    }

    // Method that generate a new map in the scene
    public MyMap CreateNewMap(IntVector2 mazeSize, int seed, int levelOfDetail, float pieceDensity, int pieceAdvancement)
    {
        m_mazeSize = mazeSize;
        m_seed = seed;
        m_levelOfDetail = levelOfDetail;
        m_pieceDensity = pieceDensity;
        m_pieceAdvancement = pieceAdvancement;

        m_mapInstance = Instantiate(m_mapPrefab, transform.position, transform.rotation);
        GenerateMap();
        return m_mapInstance;
    }

    // Method that update last map created with the new attributes
    public MyMap UpdateMap(IntVector2 mazeSize, int seed, int levelOfDetail)
    {
        m_mazeSize = mazeSize;
        m_seed = seed;
        m_levelOfDetail = levelOfDetail;

        DeleteMap();
        m_mapInstance = Instantiate(m_mapPrefab, transform.position, transform.rotation);
        GenerateMap();
        return m_mapInstance;
    }

    public void DeleteMap()
    {
        if (m_mapInstance == null)
            return;

        // Destroy map display
        DestroyImmediate(m_mapInstance.gameObject);
        m_mapInstance = null;
    }

    private void Start()
    {
        if (m_updateOnStart)
            UpdateMap(m_mazeSize, m_seed, m_levelOfDetail);
    }

    // This method is called when attributes changed in the inspector
    private void OnValidate()
    {
        // Verify attributes
        VerifyAttributes();

        // Update terrain
        if (m_autoUpdateTerrain && m_mapInstance != null)
        {
            bool valueMaze = m_generateMaze;
            m_generateMaze = false;
            UpdateMap(m_mazeSize, m_seed, m_levelOfDetail);
            m_generateMaze = valueMaze;
        }

        // Update maze
        if (m_autoUpdateMaze && m_mapInstance != null)
        {
            bool valueTerrain = m_generateTerrain;
            m_generateTerrain = false;
            UpdateMap(m_mazeSize, m_seed, m_levelOfDetail);
            m_generateMaze = valueTerrain;
        }
    }

    public void VerifyAttributes ()
    {
        if (m_mazeSize.x < 0)
        {
            m_mazeSize += IntVector2.OneX;
        }
        if (m_mazeSize.y < 0)
        {
            m_mazeSize += IntVector2.OneY;
        }
        while (m_mazeSize.x >= 255)
        {
            m_mazeSize += IntVector2.NegativeOneX;
        }
        while (m_mazeSize.y >= 255)
        {
            m_mazeSize += IntVector2.NegativeOneY;
        }
    }

    //public void MoveWalls(int numberOfChanges)
    //{
    //    m_mapInstance.MoveWallsWithDepartureCoord(numberOfChanges);
    //}
}
