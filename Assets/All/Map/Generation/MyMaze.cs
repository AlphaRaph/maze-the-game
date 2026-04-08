using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MyMaze : MonoBehaviour
{
    /// Settings
    // Maze
    [Header("Maze")]
    [SerializeField]
    protected MazeCell m_cellPrefab;
    public MazeCell CellPrefab { get { return m_cellPrefab; } }
    [SerializeField]
    protected MazePassage m_passagePrefab;
    public MazePassage PassagePrefab { get { return m_passagePrefab; } }
    [SerializeField]
    protected MazeDeparture m_departurePrefab;
    public MazeDeparture DeparturePrefab { get { return m_departurePrefab; } }
    [SerializeField]
    protected MazeArrival m_arrivalPrefab;
    public MazeArrival ArrivalPrefab { get { return m_arrivalPrefab; } }
    [SerializeField]
    protected MazeWall m_wallPrefab;
    public MazeWall WallPrefab { get { return m_wallPrefab; } set { m_wallPrefab = value; } }
    [SerializeField]
    protected MazePilar m_pilar0Prefab;
    [SerializeField]
    protected MazePilar m_pilar1Prefab;
    [SerializeField]
    protected MazePilar m_pilar2Prefab;
    [SerializeField]
    protected MazePilar m_pilar3Prefab;
    [SerializeField]
    protected MazePilar m_pilar4Prefab;
    [SerializeField]
    protected MazePilar m_pilar5Prefab;

    // Chunks
    [Header("Chunks")]
    [SerializeField]
    protected MazeChunk m_chunkPrefab;
    [SerializeField]
    protected IntVector2 m_chunkSize;
    protected int m_playerHorizontalChunkView = 5;
    protected int m_playerLateralChunkView = 2;

    // Barriers
    [Header("Barriers")]
    [SerializeField]
    protected MazeBarrier m_barrierPrefab;
    public MazeBarrier BarrierPrefab { get { return m_barrierPrefab; } set { m_barrierPrefab = value; } }
    [SerializeField]
    protected float m_barrierDistanceX = 8;

    // Pieces
    [System.Serializable]
    protected class MazeTimePiece
    {
        public string name;
        public TimePiece prefab;
        public float density;
        public float spawnHeight;
        public AnimationCurve curve;
    }
    [Header("Pieces")]
    [SerializeField]
    protected List<MazeTimePiece> m_mazeTimePieces;
    protected float[] m_stades = new float[3] { 0f, 0.3f, 0.7f };

    // Scale multiplicator
    [Header("Scale multipliacator")]
    [SerializeField]
    protected float m_scaleMultiplicator = 3;
    public float scaleMultiplicator { get { return m_scaleMultiplicator; } }

    // Spawn
    [Header("Spawn")]
    [SerializeField]
    protected int m_spawnDistance = -7;
    [SerializeField]
    protected int m_spawnHeight = 1;

    // Replay
    [Header("Replay")]
    [SerializeField]
    private Transform m_playerPathTransform;
    public Transform playerWayTransform { get { return m_playerPathTransform; } }
    //[SerializeField]
    //private Transform m_goodWayTransform;
    //[SerializeField]
    //private Brush m_goodWayBrush;

    /// Attributes
    protected MyMap m_map;
    protected IntVector2 m_size;
    public int width { get { return m_size.x; } }
    public int height { get { return m_size.y; } }
    protected int m_seed;
    protected int m_baseSeed;
    public int seed { get { return m_seed; } }
    protected System.Random m_prng;
    protected int m_numberOfPossibilities;
    protected float m_pieceDensity;
    protected int m_pieceAdvancement;
    public int piecesAdvancement { get => m_pieceAdvancement; }
    protected MazeChunk[,] m_chunks;
    protected MazeCell[,] m_mazeCells;
    protected MazeData m_mazeData;
    protected MazeBarrier[] m_mazeBarriers;
    protected Dictionary<IntVector2, ChangeData> m_changes;
    protected int m_wallAdvancement = -1;

    protected bool m_isInitialized;

    // Settings for children
    public virtual IntVector2 DepartureCoord
    {
        get
        {
            return new IntVector2(m_size.x / 2, 0);
        }
    }
    public virtual Direction DepartureDirection
    {
        get
        {
            return CardinalPoints.South;
        }
    }
    public virtual IntVector2 ArrivalCoord
    {
        get
        {
            return new IntVector2(m_size.x / 2, m_size.y - 1);
        }
    }
    public virtual Location2D SpawnLocation
    {
        get
        {
            return new Location2D(DepartureCoord + new IntVector2(m_spawnDistance, 0), DepartureCoord + new IntVector2(-m_spawnDistance, m_spawnDistance * 2));
        }
    }
    public virtual Vector3 SpawnPosition
    {
        get
        {
            return ToRealPosition(DepartureCoord) + new Vector3(0, m_spawnHeight, m_spawnDistance);
        }
    }

    #region Initialize Methods

    // Méthode initializant le labyrinthe, indispensable pour appelle de toutes methodes de la classe
    // Doit s'utiliser comme un constructeur
    public virtual void Initialize(MyMap map, IntVector2 mazeSize)
    {
        if (m_isInitialized)
            return;

        m_map = map;
        m_size = mazeSize;

        CreateChunks();
        CreateCells();
        CreateBarriers();

        transform.name = "Maze(" + mazeSize.x + " x " + mazeSize.y + ") - Not generated";

        //m_goodWayBrush.Initialize(m_goodWayTransform);
        m_changes = new Dictionary<IntVector2, ChangeData>();
    }
    protected virtual void CreateChunks()
    {
        m_chunks = new MazeChunk[width / m_chunkSize.x + 1, height / m_chunkSize.y + 1];

        for (int y = 0; y < m_chunks.GetLength(1); y++)
        {
            for (int x = 0; x < m_chunks.GetLength(0); x++)
            {
                m_chunks[x, y] = Instantiate(m_chunkPrefab, transform);
                m_chunks[x, y].Initialize(this, x, y);
            }
        }
    }
    // Méthode qui créer toutes les cellules du labyrinthe sans les initialiser (sans ajouter des murs, ...)
    protected virtual void CreateCells()
    {
        m_mazeCells = new MazeCell[m_size.x, m_size.y];
        for (int y = 0; y < m_size.y; y++)
        {
            for (int x = 0; x < m_size.x; x++)
            {
                m_mazeCells[x, y] = Instantiate(m_cellPrefab, GetChunk(x, y).transform);
            }
        }
    }
    // Méthode détruisant toutes les barriers puis les reconstruisant
    protected virtual void CreateBarriers()
    {
        m_mazeBarriers = new MazeBarrier[4];

        // North barrier
        m_mazeBarriers[0] = Instantiate(m_barrierPrefab, new Vector3(0, 0, m_size.y / 2f * m_scaleMultiplicator + m_barrierDistanceX + 0.5f), Quaternion.identity, transform);
        m_mazeBarriers[0].transform.localScale = new Vector3(m_size.x * m_scaleMultiplicator, 100, 1);

        // East barrier
        m_mazeBarriers[1] = Instantiate(m_barrierPrefab, new Vector3(-m_size.x / 2f * m_scaleMultiplicator, 0, 0), Quaternion.identity, transform);
        m_mazeBarriers[1].transform.localScale = new Vector3(1, 100, m_size.y * m_scaleMultiplicator + 2 * m_barrierDistanceX);

        // South barrier
        m_mazeBarriers[2] = Instantiate(m_barrierPrefab, new Vector3(0, 0, -(m_size.y / 2f * m_scaleMultiplicator + m_barrierDistanceX + 0.5f)), Quaternion.identity, transform);
        m_mazeBarriers[2].transform.localScale = new Vector3(m_size.x * m_scaleMultiplicator, 100, 1);

        // West barrier
        m_mazeBarriers[3] = Instantiate(m_barrierPrefab, new Vector3(m_size.x / 2f * m_scaleMultiplicator, 0, 0), Quaternion.identity, transform);
        m_mazeBarriers[3].transform.localScale = new Vector3(1, 100, m_size.y * m_scaleMultiplicator + 2 * m_barrierDistanceX);
    }
    // Méthode vérifiant si tous les attributs sont possibles
    protected virtual void VerifyAttributes()
    {
        if (m_size.x <= 0)
            m_size.x = 3;
        if (m_size.y <= 0)
            m_size.y = 3;
    }

    #endregion
   
    public virtual void Generate(int seed, int numberOfPossibilities, float pieceDensity, int pieceAdvancement) // Méthode générant le labyrinthe
    {
        if (!m_isInitialized)
            throw new System.Exception(this + " n'est pas initialisé.");

        m_seed = seed;
        m_baseSeed = seed;
        m_prng = new System.Random(m_seed);
        m_numberOfPossibilities = numberOfPossibilities;
        m_pieceDensity = pieceDensity;
        m_pieceAdvancement = pieceAdvancement;
        m_wallAdvancement = -1;
        transform.name = "Maze(" + width + " x " + height + ") - seed=" + seed;

        CreateMazeData(); // Generate maze data
        UpdateMaze(); // Create maze
        UpdateTimePieces(); // Create pieces
        EnableAllChunks();
    } 
    //public void ResetMaze() // Méthode réintialisant le labyrinthe grâce à la seed à laquelle il a été créé
    //{
    //    Generate(m_baseSeed, m_numberOfPossibilities, m_pieceDensity);
    //}

    #region Utils Methods
    // Méthode permettant de récupérer la prefab d'un bord grâce à son index
    protected virtual MazeCellEdge GetEdgePrefab(int index)
    {
        switch (index)
        {
            case 0:
                return m_passagePrefab;
            case 1:
                return m_wallPrefab;
            case 2:
                return m_departurePrefab;
            case 3:
                return m_arrivalPrefab;
            case 4:
                return m_pilar0Prefab;
            default:
                break;
        }
        if (index % 2 == 0)
        {
            return m_pilar0Prefab;
        }
        else if (index % 3 == 0)
        {
            return m_pilar1Prefab;
        }
        else if (index % 5 == 0)
        {
            return m_pilar2Prefab;
        }
        else if (index % 7 == 0)
        {
            return m_pilar3Prefab;
        }
        else if (index % 11 == 0)
        {
            return m_pilar4Prefab;
        }
        else if (index % 13 == 0)
        {
            return m_pilar5Prefab;
        }
        throw new System.InvalidOperationException(index + " ne correspond à aucun type de bord");
    }
    // Méthode permettant de récupérer la direction d'un pilier grâce à son index
    public Direction GetPilarDir(int index)
    {
        if (index % 17 == 0)
            return CardinalPoints.North;
        else if (index % 19 == 0)
            return CardinalPoints.East;
        else if (index % 23 == 0)
            return CardinalPoints.South;
        else if (index % 29 == 0)
            return CardinalPoints.West;

        throw new System.InvalidOperationException("Impossible de reconnaitre la direction du pilier : " + index);
    }
    // Méthode permettant de savoir si des coordonnées sont dans l'espace du labyrinthe ou pas
    // True : les coordonnées sont dans l'espace du labyrinthe
    // False : les coordonnées ne sont pas dans l'espace du labyrinthe
    protected bool IsPossibleCoord(IntVector2 coord)
    {
        return IsPossibleCoord(coord.x, coord.y);
    }
    protected bool IsPossibleCoord(int x, int y)
    {
        return x >= 0 && x < m_size.x && y >= 0 && y < m_size.y;
    }
    // Méthode permetter de récupérer une cellule grâce à ces coordonnées
    protected MazeCell Get(IntVector2 coord)
    {
        return !IsPossibleCoord(coord) ? null : m_mazeCells[coord.x, coord.y];
    }
    // Méthode permettant de récupérer les coordonnées d'une cellule.
    protected IntVector2 GetCoord(MazeCell cell)
    {
        for (int y = 0; y < m_size.y; y++)
        {
            for (int x = 0; x < m_size.x; x++)
            {
                if (m_mazeCells[x, y] != null && m_mazeCells[x, y].Equals(cell))
                    return new IntVector2(x, y);
            }
        }

        return new IntVector2(-1, -1);
    }
    // Méthodes convertissant des coordonnées du labyrinthe en coordonnées du GameObject parent (ici mapDisplay normalement)
    public Vector3 ToRealPosition(IntVector2 coord, float y = 0)
    {
        return ToRealPosition(coord.x, y, coord.y);
        //return m_map.ToRealPosition(m_map.MazeCoordToMapCoord(coord), 0);
    }
    public Vector3 ToRealPosition(int x, float y, int z)
    {
        //return m_map.ToRealPosition(m_map.MazeCoordToMapCoord(x, z), y);
        return (new Vector3(x - (m_size.x / 2f) + 0.5f, y, z - (m_size.y / 2f) + 0.5f) * m_scaleMultiplicator) + transform.position;
    }
    // Méthodes convertissant un position de la scène en coordonnées du labyrinthe
    public IntVector2 ToMazeCoord(Vector3 vector)
    {
        return ToMazeCoord(vector.x, vector.z);
    }
    public IntVector2 ToMazeCoord(Vector2 vector)
    {
        return ToMazeCoord(vector.x, vector.y);
    }
    public IntVector2 ToMazeCoord(float x, float y)
    {
        //return (new IntVector2(x, y) - transform.position) / scaleMultiplicator + (new Vector2(width - 0.5f, height - 0.5f) / 2f);
        //Vector2 position = new Vector2(x, y);
        //Vector2 mazePosition = new Vector2(transform.position.x, transform.position.z);
        //Vector2 posInMazeRef = (position - mazePosition) / scaleMultiplicator;
        //Vector2 cellCoordOffset = (m_size.ToVector2 / 2f);
        //Vector2 TerrainPixelOffset = new Vector2(0.5f, 0.5f);
        //return new IntVector2(posInMazeRef + cellCoordOffset - TerrainPixelOffset);
        // En une seul ligne ça donne : 
        return new IntVector2(((new Vector2(x, y) - new Vector2(transform.position.x, transform.position.z)) / scaleMultiplicator)
            + (m_size.ToVector2 / 2f) - new Vector2(0.5f, 0.5f));
    }
    public MazeChunk GetChunk(int x, int y)
    {
        return m_chunks[x / m_chunkSize.x, y / m_chunkSize.y];
    }
    public float CalculateMinTime(float playerSpeed)
    {
        return SearchGoodWay(DepartureCoord).Count * m_scaleMultiplicator / playerSpeed;
    }
    public float CalculateAreaTime(float playerSpeed)
    {
        return width * height * m_scaleMultiplicator / playerSpeed;
    }
    public List<IntVector2> SearchGoodWay(IntVector2 playerCoord)
    {
        if (!IsPossibleCoord(playerCoord)) playerCoord = DepartureCoord;

        List<IntVector2> goodWay;
        if (m_mazeData.SearchGoodWay(playerCoord, out goodWay)) 
            return goodWay;
        else 
            return null;
    }
    #endregion

    #region MazeData
    // Méthode qui génère une carte du labyrinthe
    protected virtual void CreateMazeData()
    {
        // Create a new maze data
        m_mazeData = new MazeData(m_size, m_numberOfPossibilities, m_seed);

        // Generate maze
        m_mazeData.Generate();

        // Generate pieces
        if (m_pieceDensity != 0)
        {
            for (int i = 0; i < m_mazeTimePieces.Count; i++)
            {
                m_mazeData.AddTimePieces(m_mazeTimePieces[i].density * m_pieceDensity, i);
            }
        }
    }
    #endregion

    #region Maze Generation
    protected virtual void UpdateMaze()
    {
        if (!m_isInitialized || m_mazeData == null)
            throw new System.InvalidOperationException("Le labyrinthe n'a pas été initializé ou mazeData est null");

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                MazeCell cell = m_mazeCells[x, z];
                IntVector2 coord = new IntVector2(x, z);
                cell.Initialize(this, GetChunk(x, z), coord);

                if (x == 0)
                {
                    UpdateEdge(cell, coord, 6); // West
                    UpdateEdge(cell, coord, 7); // North West
                }
                if (z== 0)
                {
                    UpdateEdge(cell, coord, 4); // South
                    UpdateEdge(cell, coord, 3); // South East
                }
                if (x == 0 && z == 0)
                {
                    UpdateEdge(cell, coord, 5); // South West
                }

                for (int iDir = 0; iDir < 3; iDir++) // North, North East, East
                {
                    UpdateEdge(cell, coord, iDir);
                }
            }
        }
    }
    protected void UpdateEdge(MazeCell cell, IntVector2 coord, int iDir)
    {
        if (cell.GetEdge(iDir) == null)
        {
            CreateEdge(cell, coord, iDir);
        }
        else if (m_mazeData.cells[coord.x, coord.y].edges[iDir] != cell.GetEdge(iDir).Value)
        {
            DestroyEdge(cell, coord, iDir);
            CreateEdge(cell, coord, iDir);
        }
    }
    protected void DestroyEdge(MazeCell cell, IntVector2 coord, int iDir)
    {
        // Destroy Edge
        MazeCellEdge edge = cell.GetEdge(iDir);
        Direction direction = CardinalPoints.Get(iDir);
        Destroy(edge.gameObject);
        edge = null;

        // Set edge for its cells
        cell.SetEdge(direction, null);
        MazeCell neighbor = Get(coord + direction.ToIntVector2);
        if (neighbor != null)
        {
            neighbor.SetEdge(direction.Opposite, edge);
        }

        if (!CardinalPoints.IsMainCardinalPoint(direction))
        {
            neighbor = Get(coord + CardinalPoints.Last(direction).ToIntVector2);
            if (neighbor != null)
            {
                neighbor.SetEdge(CardinalPoints.Last(direction, 2).Opposite, edge);
            }
            neighbor = Get(coord + CardinalPoints.Next(direction).ToIntVector2);
            if (neighbor != null)
            {
                neighbor.SetEdge(CardinalPoints.Next(direction, 2).Opposite, edge);
            }
        }
    }
    protected void CreateEdge(MazeCell cell, IntVector2 coord, int iDir)
    {
        // Create Edge
        MazeCellEdge edge = Instantiate(GetEdgePrefab(m_mazeData.cells[coord.x, coord.y].edges[iDir])) as MazeCellEdge;
        if (iDir % 2 == 1)
        {
            edge.GFX.transform.localRotation = GetPilarDir(m_mazeData.cells[coord.x, coord.y].edges[iDir]).ToRotation;
        }
        Direction direction = CardinalPoints.Get(iDir);
        edge.Initialize(this, cell, direction);

        // Set edge for its cells
        cell.SetEdge(direction, edge);
        MazeCell neighbor = Get(coord + direction.ToIntVector2);
        if (neighbor != null)
        {
            neighbor.SetEdge(direction.Opposite, edge);
        }

        if (!CardinalPoints.IsMainCardinalPoint(direction))
        {
            neighbor = Get(coord + CardinalPoints.Last(direction).ToIntVector2);
            if (neighbor != null)
            {
                neighbor.SetEdge(CardinalPoints.Last(direction, 2).Opposite, edge);
            }
            neighbor = Get(coord + CardinalPoints.Next(direction).ToIntVector2);
            if (neighbor != null)
            {
                neighbor.SetEdge(CardinalPoints.Next(direction, 2).Opposite, edge);
            }
        }
    }
    #endregion

    #region Changes
    public List<MazeMove> GetNewMoves(Vector3 playerPosition)
    {
        return GetNewMoves(new Vector2(playerPosition.x, playerPosition.z));
    }
    public List<MazeMove> GetNewMoves(IntVector2 playerCoord)
    {
        return GetNewMoves(ToRealPosition(playerCoord));
    }
    public List<MazeMove> GetNewMoves(Vector2 playerPosition)
    {
        /*
         * Déplacent les murs du labyrinthe pendant un certain nombre de secondes et selon la position du joueur
         */

        // Calculate the number of changes
        int numberOfChanges = m_map.maze.width * m_map.maze.height / 15;
        if (numberOfChanges != 0)
        {
            // Calculate the player coordinates
            IntVector2 playerCoord = ToMazeCoord(playerPosition);
            if (!IsPossibleCoord(playerCoord)) playerCoord = DepartureCoord;

            // Change maze data
            Dictionary<IntVector2, ChangeData> newChanges = m_mazeData.ChangeWalls(numberOfChanges, playerCoord);

            // Change maze
            if (newChanges == null)
            {
                Debug.Log("Il n'y pas de possibilité de changement dans ce labyrinthe");
            }
            else
            {
                return ConvertChangeDatasInMoves(newChanges);
            }
        }
        return null;
    }
    protected List<MazeMove> ConvertChangeDatasInMoves(Dictionary<IntVector2, ChangeData> changes)
    {
        List<MazeMove> wallMoves = new List<MazeMove>();
        foreach (KeyValuePair<IntVector2, ChangeData> change in changes)
        {
            IntVector2 coord = change.Key;
            ChangeData changeData = change.Value;

            if (changeData.kindOfChange == KindOfMove.Central)
            {
                int firstIDir = changeData.iDir;
                int secondIDir = CardinalPoints.ReverseIndexOfDirection(firstIDir);

                // Get all coords needing for the change
                IntVector2 firstCoord = coord + CardinalPoints.Get(firstIDir).ToIntVector2;
                IntVector2 secondCoord = coord + CardinalPoints.Get(secondIDir).ToIntVector2;

                // Get all cells needing for the change
                MazeCell cell = Get(coord);
                MazeCell firstNeighbor = Get(firstCoord);
                MazeCell secondNeighbor = Get(secondCoord);

                // Change values for the cells
                MazeCellEdge firstEdge = cell.GetEdge(firstIDir);
                MazeCellEdge secondEdge = cell.GetEdge(secondIDir);

                firstEdge.ChangeMainCell(cell);
                secondEdge.ChangeMainCell(cell);

                firstNeighbor.SetEdge(CardinalPoints.Get(secondIDir), secondEdge); // SecondIDir = Reverse of firstIDir
                cell.SetEdge(CardinalPoints.Get(firstIDir), secondEdge);

                cell.SetEdge(CardinalPoints.Get(secondIDir), firstEdge);
                secondNeighbor.SetEdge(CardinalPoints.Get(firstIDir), firstEdge); // FirstIDir = Reverse of secondIDir

                // Move the two edge to their new position
                wallMoves.Add(new MazeMoveCentral(cell, firstEdge));
                wallMoves.Add(new MazeMoveCentral(cell, secondEdge));
            }
            else if (changeData.kindOfChange == KindOfMove.Lateral)
            {
                // Pour une autre fois
            }
        }
        return wallMoves;
    }
    public IEnumerator MoveWalls(List<MazeMove> wallMoves, float seconds, int advancement = 0)
    {
        while (advancement < 1000)
        {
            foreach (MazeMove wallMove in wallMoves)
            {
                if (wallMove is MazeMoveCentral)
                {
                    MazeMoveCentral wmc = (MazeMoveCentral) wallMove;
                    wmc.edge.GFX.transform.localPosition = Vector3.Lerp(wmc.startingPos, wmc.endPos, (advancement / 1000f) * seconds);
                }
            }

            int addingAdvancement = (int)(Time.fixedDeltaTime * 1000 / seconds);
            advancement += addingAdvancement;
            m_wallAdvancement = advancement;

            yield return new WaitForFixedUpdate();
        }

        foreach (MazeMove wallMove in wallMoves)
        {
            if (wallMove is MazeMoveCentral)
            {
                MazeMoveCentral wmc = (MazeMoveCentral)wallMove;
                wmc.edge.GFX.transform.localPosition = wmc.endPos;
                wmc.edge.FinishCentralMove();
            }
        }
        m_wallAdvancement = 1000;
    }
    public void UpdateMoveWalls(List<MazeMove> wallMoves, float seconds, int advancement = 0)
    {
        if (wallMoves == null || wallMoves.Count == 0) return;

        if (advancement < 1000)
        {
            foreach (MazeMove wallMove in wallMoves)
            {
                if (wallMove is MazeMoveCentral)
                {
                    MazeMoveCentral wmc = (MazeMoveCentral)wallMove;
                    wmc.edge.GFX.transform.localPosition = Vector3.Lerp(wmc.startingPos, wmc.endPos, (advancement / 1000f) * seconds);
                }
            }
        }
        else
        {
            foreach (MazeMove wallMove in wallMoves)
            {
                if (wallMove is MazeMoveCentral)
                {
                    MazeMoveCentral wmc = (MazeMoveCentral)wallMove;
                    wmc.edge.GFX.transform.localPosition = wmc.endPos;
                    wmc.edge.FinishCentralMove();
                }
            }
        }

        m_wallAdvancement = advancement;
    }
    #endregion

    #region TimePieces  
    public void UpdateTimePieces()
    {
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                // If there is a time piece in the cell
                if (m_mazeData.cells[x, z].hasTimePiece)
                {
                    UpdateTimePiece(m_mazeCells[x, z], x, z, PossibleValue(m_mazeData.heightMapTP[x, z] + m_pieceAdvancement / 1000f));
                }
            }
        }
    }
    public void MoveTimePieces()
    {
        m_pieceAdvancement++;
        UpdateTimePieces();
    }
    public void MoveTimePieces(int piecesAdvancement)
    {
        m_pieceAdvancement = piecesAdvancement;
        UpdateTimePieces();
    }
    protected void RemoveTimePiece(TimePiece timePiece)
    {
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (m_mazeCells[x, z].timePiece.Equals(timePiece))
                {
                    m_mazeCells[x, z].RemoveTimePiece();
                    m_mazeData.cells[x, z].RemoveTimePiece();
                }
            }
        }
    }
    protected float PossibleValue(float value)
    {
        while (value < 0)
        {
            value++;
        }
        while (value >= 1)
        {
            value--;
        }

        return value;
    }
    protected void UpdateTimePiece(MazeCell cell, int x, int z, float height)
    {
        if (!cell.hasTimePiece)
        {
            TimePiece timePiece = Instantiate(m_mazeTimePieces[m_mazeData.cells[x, z].pieceIndex].prefab, ToRealPosition(x, 0, z),
                Quaternion.Euler(new Vector3(0, m_prng.Next(360), 0)), cell.transform);
            timePiece.Initialize(this, m_mazeData.cells[x, z].pieceIndex);
            cell.SetTimePiece(timePiece);
        }

        cell.HeightTimePiece(height);
        cell.FixedRotateTimePiece();
    }
    public float PlayerCollisionWithPieces(Vector3 playerPosition)
    {
        return PlayerCollisionWithPieces(new Vector2(playerPosition.x, playerPosition.z));
    }
    public float PlayerCollisionWithPieces(Vector2 playerPosition)
    {
        IntVector2 playerCoord = ToMazeCoord(playerPosition);
        if (!IsPossibleCoord(playerCoord)) return 0;

        MazeCell cell = Get(playerCoord);

        //Debug.Log("playerCoord : " + playerCoord);
        //Debug.Log("cell.IsPlayerTouchingPiece(playerPosition) : " + cell.IsPlayerTouchingPiece(playerPosition));
        if (cell.IsPlayerTouchingPiece(playerPosition))
        {
            float value = cell.timePiece.value;

            // Remove time piece
            cell.DestroyTimePiece();
            m_mazeData.cells[playerCoord.x, playerCoord.y].RemoveTimePiece();

            return value;
        }
        else
            return 0;
    }
    #endregion

    #region VisibleMaze
    public void UpdateChunks(Vector3 playerPosition)
    {
        IntVector2 playerCoord = ToMazeCoord(playerPosition);

        if (playerCoord.y < 0) // Si le joueur est devant le labyrinthe
        {
            for (int x = 0; x < m_chunks.GetLength(0); x++)
            {
                ActiveChunk(x * m_chunkSize.x, 0);
            }
        }
        else if (playerCoord.y >= height) // Si le joueur est derrière le labyrinthe
        {
            for (int x = 0; x < m_chunks.GetLength(0); x++)
            {
                ActiveChunk(x * m_chunkSize.x, height - 1);
            }
        }
        else // Si le joueur est dans le labyrinthe
        {
            // Current
            ActiveChunk(playerCoord.x, playerCoord.y);
            // Left
            ActiveChunk(playerCoord.x - m_playerHorizontalChunkView, playerCoord.y);
            // Upper Left 
            ActiveChunk(playerCoord.x - m_playerLateralChunkView, playerCoord.y + m_playerLateralChunkView);
            // Up
            ActiveChunk(playerCoord.x, playerCoord.y + m_playerHorizontalChunkView);
            // Upper Right
            ActiveChunk(playerCoord.x + m_playerLateralChunkView, playerCoord.y + m_playerLateralChunkView);
            // Right
            ActiveChunk(playerCoord.x + m_playerHorizontalChunkView, playerCoord.y);
            // Lower Right
            ActiveChunk(playerCoord.x + m_playerLateralChunkView, playerCoord.y - m_playerLateralChunkView);
            // Low
            ActiveChunk(playerCoord.x, playerCoord.y - m_playerHorizontalChunkView);
            // Lower Left
            ActiveChunk(playerCoord.x - m_playerLateralChunkView, playerCoord.y - m_playerLateralChunkView);
        }
    }
    protected void ActiveChunk(int x, int y)
    {
        if (x < 0) x = 0;
        else if (x >= width) x = width - 1;
        if (y < 0) y = 0;
        else if (y >= height) y = height - 1;
        GetChunk(x, y).gameObject.SetActive(true);
        GetChunk(x, y).StayActive();
    }
    public void EnableAllChunks()
    {
        for (int y = 0; y < m_chunks.GetLength(1); y++)
        {
            for (int x = 0; x < m_chunks.GetLength(0); x++)
            {
                m_chunks[x, y].gameObject.SetActive(true);
                m_chunks[x, y].StayActiveForEver();
            }
        }
    }
    public void DisableAllChunks()
    {
        for (int y = 0; y < m_chunks.GetLength(1); y++)
        {
            for (int x = 0; x < m_chunks.GetLength(0); x++)
            {
                m_chunks[x, y].Disable();
            }
        }
    }
    #endregion
}
