using UnityEngine;

public class MyMap : MonoBehaviour
{
    // Settings
    //[SerializeField]
    //private IntVector2 m_terrainAddingSize = new IntVector2(30, 30);
    [SerializeField]
    private int m_scaleMultiplicator = 3;
    [SerializeField]
    protected IntVector2 m_spawnLocationSize;
    [SerializeField]
    private float m_spawnHeight = 1;
    public float spawnHeight { get { return m_spawnHeight; } }

    // References
    [SerializeField]
    private MyMaze m_mazeInstance;
    public MyMaze maze { get { return m_mazeInstance; } }
    private Location2D m_mazeLocation;
    [SerializeField]
    private MyTerrain m_terrainInstance;
    public MyTerrain terraiin { get { return m_terrainInstance; } }
    private Location2D m_terrainLocation;

    // Attributes
    private IntVector2 m_mapSize;
    public int width { get { return m_mapSize.x;  } }
    private int height { get { return m_mapSize.y; } }
    private IntVector2 m_mazeSize;
    private int m_seed;
    private int m_levelOfDetail;

    public bool generateTerrain { get; set; }
    public bool generateMaze { get; set; }

    public Vector3 SpawnPosition
    {
        get
        {
            // return m_mazeInstance.ToRealPosition(m_mazeInstance.DepartureCoord - IntVector2.OneY, 1.08f);
            return ToRealPosition(MazeCoordToMapCoord(m_mazeInstance.DepartureCoord) - new IntVector2(0, m_spawnLocationSize.y / 2f) * m_levelOfDetail) + new Vector3(-0.75f, m_spawnHeight, 0);
        }
    }

    public Location2D SpawnLocation
    {
        get
        {
            return new Location2D(MazeCoordToMapCoord(m_mazeInstance.DepartureCoord) + (new IntVector2(-m_spawnLocationSize.x / 2f, -m_spawnLocationSize.y) * m_levelOfDetail),
                MazeCoordToMapCoord(m_mazeInstance.DepartureCoord) + (new IntVector2(m_spawnLocationSize.x / 2f, 0) * m_levelOfDetail) + (new IntVector2(1, 0) * m_levelOfDetail / 2f));
        }
    }

    public Location2D FinishLocation
    {
        get
        {
            return new Location2D(MazeCoordToMapCoord(m_mazeInstance.ArrivalCoord) + (new IntVector2(-m_spawnLocationSize.x / 2f, 0) * m_levelOfDetail) + (new IntVector2(0, 3) * m_levelOfDetail / 2f),
                MazeCoordToMapCoord(m_mazeInstance.ArrivalCoord) + (new IntVector2(m_spawnLocationSize.x / 2f, m_spawnLocationSize.y) * m_levelOfDetail) + (new IntVector2(1, 3) * m_levelOfDetail / 2f));
        }
    }

    public IntVector2 MazeCoordToMapCoord (int x, int y)
    {
        return MazeCoordToMapCoord(new IntVector2(x, y));
    }

    public IntVector2 MazeCoordToMapCoord (IntVector2 mazeCoord)
    {
        return m_mazeLocation.firstCoord + mazeCoord * m_levelOfDetail + (new IntVector2(1, 0) * m_levelOfDetail  / 2f);
    }

    public Vector3 ToRealPosition(IntVector2 coord, float y = 0f)
    {
        return ToRealPosition(coord.x, y, coord.y);
    }

    public Vector3 ToRealPosition(float x, float y, float z)
    {
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;
        return new Vector3(-(topLeftX + x) / m_levelOfDetail, y, -(topLeftZ - z) / m_levelOfDetail) * m_scaleMultiplicator;
    }

    public void Create(IntVector2 mazeSize, int seed, int levelOfDetail, float pieceDensity, int pieceAdvancement)
    {
        //m_mapSize = (mazeSize + m_terrainAddingSize) * levelOfDetail;
        m_mapSize = new IntVector2(126, 126);
        m_mazeSize = mazeSize;
        m_terrainLocation = new Location2D(IntVector2.Zero, m_mapSize);
        m_mazeLocation = new Location2D(m_terrainLocation.Center - ((m_mazeSize.ToVector2 + Vector2.one) / 2f  * levelOfDetail), m_terrainLocation.Center + ((m_mazeSize.ToVector2) / 2f * levelOfDetail));
        m_seed = seed;
        m_levelOfDetail = levelOfDetail;

        // Generate Maze
        if (m_terrainInstance != null && generateMaze)
        {
            m_mazeInstance.Initialize(this, m_mazeSize);
            m_mazeInstance.Generate(m_seed, 1, pieceDensity, pieceAdvancement);
            m_mazeInstance.EnableAllChunks();
        }

        // Generate Terrain
        if (m_terrainInstance != null && generateTerrain)
        {
            m_terrainInstance.Initialize(this, m_terrainLocation, m_mazeLocation, m_seed, m_levelOfDetail);
            m_terrainInstance.Generate();
        }
    }

    //public void MoveWallsWithDepartureCoord (int numberOfChanges)
    //{
    //    m_mazeInstance.MoveWalls(numberOfChanges, m_mazeInstance.ToRealPosition(m_mazeInstance.DepartureCoord));
    //}
}
