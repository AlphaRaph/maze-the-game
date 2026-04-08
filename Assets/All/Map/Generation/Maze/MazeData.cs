using UnityEngine;
using System.Collections.Generic;

public class MazeData
{
    public CellData[,] cells { get; private set; }
    public IntVector2 size { get; private set; }
    public int seed { get; private set; }
    private System.Random m_prng;
    //public int mazeSeed { get { return seed / 1000000; } private set { seed = value * 1000000 + piecesSeed; } }
    //public int piecesSeed { get { return seed % 1000000; } private set { seed = mazeSeed * 1000000 + value; } }
    //public int piecesMapSeed { get { return piecesSeed / 1000; } private set { piecesSeed = value * 1000 + (piecesSeed % 1000); } }
    //private System.Random m_mazePrng, m_piecesPrng;
    public int numberOfPossibilities { get; private set; }
    public float[,] heightMapTP { get; private set; }

    private float m_allDensity;

    public virtual IntVector2 DepartureCoord
    {
        get
        {
            return new IntVector2(size.x / 2, 0);
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
            return new IntVector2(size.x / 2, size.y - 1);
        }
    }

    //private List<IntVector2> m_goodWayCoords;
    //public List<IntVector2> goodWay { get { SearchGoodWay(m_playerCoord); return m_goodWayCoords; } }
    //public bool isAGoodWay { get; set; }
    //private IntVector2 m_playerCoord;

    public MazeData(IntVector2 size, int numberOfPossibilities, int seed)
    {
        this.size = size;
        this.numberOfPossibilities = numberOfPossibilities;
        cells = new CellData[size.x, size.y];

        this.seed = seed;
        m_prng = new System.Random(this.seed);
        //m_playerCoord = DepartureCoord;
    }
    private void Reset ()
    {
        for (int z = 0; z < size.y; z++)
        {
            for (int x = 0; x < size.x; x++)
            {
                cells[x, z] = new CellData();
            }
        }

        // Set the arrival and the departure
        cells[DepartureCoord.x, DepartureCoord.y].SetEdge(4, 2); // 4 = South
        cells[ArrivalCoord.x, ArrivalCoord.y].SetEdge(0, 3); // 0 = North

        // Set the good way attributes
        //m_goodWayCoords = new List<IntVector2>();
        //isAGoodWay = false;

        // Time Pieces
        heightMapTP = Noise.GenerateNoiseMap(size, seed, 1, 4, 0.5f, 2, Vector2.zero, 1);
        m_allDensity = 0f;
    }
    public void Generate()
    {
        Reset();
        Recursively(ArrivalCoord);
        AddPilars();
    }

    #region Generation
    private void Recursively(IntVector2 coord)
    {
        CellData cell = cells[coord.x, coord.y];
        cell.isActive = true;

        while (!cell.isMainInitialized)
        {
            //Debug.Log("current coord : " + coord);
            int iDir = cell.RIUMDirection(m_prng);

            //Debug.Log("cell.edges : " + cell.StringEdges());

            Direction dir = CardinalPoints.Get(iDir);
            //Debug.Log("Dir : " + dir);
            IntVector2 newCoord = coord + dir.ToIntVector2;
            if (!IsOutside(newCoord))
            {
                //Debug.Log("It's not outside !");
                CellData neighbor = cells[newCoord.x, newCoord.y];
                //Debug.Log("neighbor.edges : " + neighbor.StringEdges());

                if (neighbor.edges[CardinalPoints.ReverseIndexOfDirection(iDir)] == -1)
                {
                    if (!neighbor.isActive)
                    {
                        // Set passage and move to the new cell
                        //Debug.Log("Je crée un passage ! " + dir);
                        cell.SetEdge(iDir, 0);
                        neighbor.SetEdge(CardinalPoints.ReverseIndexOfDirection(iDir), 0);

                        Recursively(newCoord);
                        continue;
                    }
                }
                else if (neighbor.edges[CardinalPoints.ReverseIndexOfDirection(iDir)] == 0)
                {
                    throw new System.InvalidOperationException("MazeCell shouldn't have a passage with a neighbor uninitialized. " + dir);
                }
                else if (neighbor.edges[CardinalPoints.ReverseIndexOfDirection(iDir)] == 1)
                {
                    throw new System.InvalidOperationException("MazeCell shouldn't have a wall with a neighbor uninitialized. " + dir);
                }
                else if (neighbor.edges[CardinalPoints.ReverseIndexOfDirection(iDir)] == 2)
                {
                    throw new System.InvalidOperationException("MazeCell shouldn't have a departure uninitialized. " + dir);
                }
                else if (neighbor.edges[CardinalPoints.ReverseIndexOfDirection(iDir)] == 3)
                {
                    throw new System.InvalidOperationException("MazeCell shouldn't have an arrival uninitialized. " + dir);
                }

                // Create a wall for the neighbor
                //Debug.Log("Je crée un mur je ne sais pas pourquoi." + dir);
                //Debug.Log("neighbor.isActive : " + neighbor.isActive);

                neighbor.SetEdge(CardinalPoints.ReverseIndexOfDirection(iDir), 1);
            }

            //Debug.Log("Je crée un mur ! " + dir);
            cell.SetEdge(iDir, 1);
        }
    }
    #endregion

    #region Pilars
    private void AddPilars()
    {
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                CellData cell = cells[x, y];
                IntVector2 coord = new IntVector2(x, y);

                if (x == 0)
                {
                    cell.SetEdge(7, SearchTheGoodPilar(coord, 7)); // North West
                }
                if (y == 0)
                {
                    cell.SetEdge(3, SearchTheGoodPilar(coord, 3)); // South East
                }
                if (x == 0 && y == 0)
                {
                    cell.SetEdge(5, SearchTheGoodPilar(coord, 5)); // South West
                }

                cell.SetEdge(1, SearchTheGoodPilar(coord, 1)); // North East
            }
        }
    }
    private int SearchTheGoodPilar(IntVector2 coord, int iDir)
    {
        CellData cell = cells[coord.x, coord.y];

        // Calculate the number of walls next to the pilar
        int numberOfWalls = 0;
        bool[] wallDirections = new bool[4] { false, false, false, false };


        IntVector2 newCoord = coord + CardinalPoints.Get(iDir - 1).ToIntVector2;
        if (!IsOutside(newCoord) && cells[newCoord.x, newCoord.y].edges[IndexOfDirection(iDir + 1)] == 1)
        {
            numberOfWalls++;
            wallDirections[0] = true; // North
        }


        newCoord = coord + CardinalPoints.Get(iDir + 1).ToIntVector2;
        if (!IsOutside(newCoord) && cells[newCoord.x, newCoord.y].edges[IndexOfDirection(iDir - 1)] == 1)
        {
            numberOfWalls++;
            wallDirections[1] = true; // East
        }


        if (cell.edges[IndexOfDirection(iDir + 1)] == 1)
        {
            numberOfWalls++;
            wallDirections[2] = true; // South
        }


        if (cell.edges[IndexOfDirection(iDir - 1)] == 1)
        {
            numberOfWalls++;
            wallDirections[3] = true; // West
        }

        // Calculate the direction of the pilar
        int iDirPilar = 0;
        if (numberOfWalls < 4)
        {
            for (int i = 0; i < wallDirections.Length; i++)
            {
                if (!wallDirections[i] && wallDirections[IndexOfDirection((i + 1) * 2) / 2])
                {
                    iDirPilar = i * 2;
                }
            }
        }

        iDirPilar = IndexOfDirection(iDirPilar);


        if (numberOfWalls == 0)
        {
            return IndexOfPilar(5, iDirPilar);
        }
        else if (numberOfWalls == 1)
        {
            return IndexOfPilar(4, iDirPilar);
        }
        else if (numberOfWalls == 2)
        {
            for (int i = 0; i < wallDirections.Length; i++)
            {
                if (wallDirections[i] && wallDirections[IndexOfDirection((i + 1) * 2) / 2])
                {
                    return IndexOfPilar(2, iDirPilar);
                }
            }

            return IndexOfPilar(3, iDirPilar);
        }
        else if (numberOfWalls == 3)
        {
            return IndexOfPilar(1, iDirPilar);
        }
        else if (numberOfWalls == 4)
        {
            return IndexOfPilar(0, iDirPilar);
        }

        throw new System.InvalidOperationException("Ceci est impossible, un pilier ne peut pas être à coté de plus de 4 murs");
    }
    #endregion

    #region Utils
    private int IndexOfDirection(int index)
    {
        while (index > 7)
            index -= 8;
        while (index < 0)
            index += 8;
        return index;
    }
    private int IndexOfPilar(int type, int iDir)
    {
        int index;
        switch (type)
        {
            case 0:
                index = 2;
                break;
            case 1:
                index = 3;
                break;
            case 2:
                index = 5;
                break;
            case 3:
                index = 7;
                break;
            case 4:
                index = 11;
                break;
            case 5:
                index = 13;
                break;
            default:
                throw new System.InvalidOperationException("Ce type de pilier n'existe pas : " + type);
        }

        switch(iDir)
        {
            case 0:
                return index * 17;
            case 2:
                return index * 19;
            case 4:
                return index * 23;
            case 6:
                return index * 29;
            default:
                throw new System.InvalidOperationException("Cette index de direction n'existe pas : " + iDir);
        }
    }
    private bool IsOutside(IntVector2 coord)
    {
        return coord.x < 0 || coord.x >= size.x || coord.y < 0 || coord.y >= size.y;
    }
    #endregion

    #region GoodWay
    public bool SearchGoodWay (IntVector2 playerCoord)
    {
        List<IntVector2> goodWay = new List<IntVector2>();
        return SearchGoodWay(playerCoord, out goodWay);
    }
    public bool SearchGoodWay (IntVector2 playerCoord, out List<IntVector2> goodWay)
    {
        // Set the necessary variables
        goodWay = new List<IntVector2>();
        bool isAGoodWay = false;

        for (int z = 0; z < size.y; z++)
        {
            for (int x = 0; x < size.x; x++)
            {
                cells[x, z].isNotGoodWay = false;
                cells[x, z].isSearchingGoodWay = false;
            }
        }

        // Search the good way
        SearchGoodWayRecursively(IntVector2.NegativeOne, playerCoord, ref goodWay, ref isAGoodWay);
        goodWay.Reverse();

        if (!isAGoodWay)
            goodWay = new List<IntVector2>();

        return isAGoodWay;
    }
    private void SearchGoodWayRecursively(IntVector2 comingCoord, IntVector2 coord, ref List<IntVector2> goodWay, ref bool isAGoodWay)
    {
        CellData cell = cells[coord.x, coord.y];
        cell.isSearchingGoodWay = true;
        int badNeighbors = 0;

        if (comingCoord == IntVector2.NegativeOne)
        {
            badNeighbors--;
        }
        if (coord == ArrivalCoord)
        {
            goodWay.Add(coord);
            cell.isNotGoodWay = false;
            isAGoodWay = true;
            return;
        }

        for (int i = 0; i < 8; i += 2)
        {
            IntVector2 newCoord = coord + CardinalPoints.Get(i).ToIntVector2;

            if (cell.edges[i] == 1 || cell.edges[i] == -1 || IsOutside(newCoord))
            {
                badNeighbors++;
                continue;
            }

            CellData neighbor = cells[newCoord.x, newCoord.y];

            if (neighbor.isSearchingGoodWay)
            {
                if (newCoord != comingCoord)
                    badNeighbors++;
            }
            else
            {
                SearchGoodWayRecursively(coord, newCoord, ref goodWay, ref isAGoodWay);

                if (neighbor.isNotGoodWay)
                {
                    badNeighbors++;
                }
            }
        }

        if (badNeighbors < 3)
        {
            goodWay.Add(coord);
            cell.isNotGoodWay = false;
        }
        else
        {
            cell.isNotGoodWay = true;
        }
    }
    #endregion

    #region Changes
    public Dictionary<IntVector2, ChangeData> ChangeWalls(int numberOfChanges, IntVector2 playerCoord)
    {
        Dictionary<IntVector2, ChangeData> changes = new Dictionary<IntVector2, ChangeData>();

        int goodWayLoopMax = 100, goodWayLoopCpt = 0;
        int changeLoopMax = 100, changeLoopCpt = 0;

        do
        {
            foreach (KeyValuePair<IntVector2, ChangeData> change in changes)
            {
                // Undo last changes
                Change(change);
            }
            changes.Clear();

            if (goodWayLoopCpt >= goodWayLoopMax)
                return null;

            changeLoopCpt = 0;
            for (int i = 0; i < numberOfChanges; i++)
            {
                KeyValuePair<IntVector2, ChangeData> change;

                do
                {
                    if (changeLoopCpt >= changeLoopMax)
                        goto ChangeLoopMax;

                    change = new KeyValuePair<IntVector2, ChangeData>(new IntVector2(m_prng.Next(size.x), m_prng.Next(size.y)),
                        new ChangeData(m_prng.Next(0, 4) * 2, KindOfMove.Central));

                    changeLoopCpt++;
                } while (changes.ContainsKey(change.Key) || (SearchGoodWay(playerCoord) && IsChangeNextToPlayer(change, playerCoord)) || !Change(change));

                changes.Add(change.Key, change.Value);
                //Debug.Log("change : " + change.Key + " " + CardinalPoints.Get(change.Value.iDir));
            }

            ChangeLoopMax: ;
            

            //DebugMaze();
            //DebugGoodWay();

            goodWayLoopCpt++;

        } while (!SearchGoodWay(playerCoord));

        return changes;
    }
    private bool Change(KeyValuePair<IntVector2, ChangeData> change)
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

            // Verify if the change is possible
            if (IsOutside(firstCoord) || IsOutside(secondCoord) || !CardinalPoints.IsMainIndexOfCardinalPoint(firstIDir) ||
                !CardinalPoints.IsMainIndexOfCardinalPoint(secondIDir))
            {
                return false;
            }

            // Get all cells needing for the change
            CellData cell = cells[coord.x, coord.y];
            CellData firstNeighbor = cells[firstCoord.x, firstCoord.y];
            CellData secondNeighbor = cells[secondCoord.x, secondCoord.y];

            // Change values for the cells
            int firstValue = cell.edges[firstIDir];
            int secondValue = cell.edges[secondIDir];

            if (firstValue == secondValue) // If the edges are the same it's not worth making the change
                return false;

            firstNeighbor.SetEdge(secondIDir, secondValue); // SecondIDir = Reverse of firstIDir
            cell.SetEdge(firstIDir, secondValue);

            cell.SetEdge(secondIDir, firstValue);
            secondNeighbor.SetEdge(firstIDir, firstValue); // FirstIDir = Reverse of secondIDir
        }
        else if (changeData.kindOfChange == KindOfMove.Lateral)
        {
            // Pour une autre fois
        }

        return true;
    }
    private bool IsChangeNextToPlayer(KeyValuePair<IntVector2, ChangeData> change, IntVector2 playerCoord)
    {
        Location2D playerLocation = new Location2D(playerCoord - IntVector2.One, playerCoord + IntVector2.One);
        return playerLocation.Contain(change.Key);
    }
    #endregion

    #region Debug
    public void DebugMaze()
    {
        for (int z = 0; z < size.y; z++)
        {
            for (int x = 0; x < size.x; x++)
            {
                Debug.Log(new IntVector2(x, z) + " = " + cells[x, z].StringEdges());
            }
        }
    }
    public void DebugGoodWay(List<IntVector2> goodWay)
    {
        if (goodWay.Count != 0)
        {
            string debug = " { ";
            for (int i = 0; i < goodWay.Count - 1; i++)
            {
                debug += goodWay[i] + ", ";
            }
            debug += goodWay[goodWay.Count - 1] + " }";
            Debug.Log("good way : " + debug);
        }
        else
        {
            Debug.Log("Il n'y a pas de bon chemin.");
        }
    }
    #endregion

    #region TimePieces
    public void AddTimePieces(float density, int pieceIndex) // density : between 0 and 1
    {
        if (m_allDensity >= 1)
        {
            Debug.LogWarning("allDensity >= 1");
            return;
        }
        int maxSkips = (int)((1 - m_allDensity) / density);
        m_allDensity += density;
        int skips = m_prng.Next(maxSkips);
        for (int z = 0; z < size.y; z++)
        {
            for (int x = 0; x < size.x; x++)
            {
                if (!cells[x, z].hasTimePiece)
                {
                    if (skips <= 0)
                    {
                        cells[x, z].AddTmePiece(pieceIndex);
                        skips = m_prng.Next(maxSkips);
                    }
                    skips--;
                }
            }
        }
    }
    #endregion
}

public class CellData
{
    private int[] m_edges; // null = -1 ; passage = 0 ; wall = 1 ; departure = 2 ; arrival = 3 ; pilar1 = 4 ; pilar2 = 5; pilar3 = 6; pilar4 = 7; pilar5 = 8;
    public int[] edges { get { return m_edges; } }
    private int m_mainInitializedEdgeCount;
    private int m_wallCount;
    public int wallCount { get { return m_wallCount; } }
    public bool isActive { get; set; }
    public bool isNotGoodWay { get; set; }
    public bool isSearchingGoodWay { get; set; }
    public bool hasTimePiece { get; private set; }
    public int pieceIndex { get; private set; }

    public CellData()
    {
        m_edges = new int[8] { -1, -1, -1, -1, -1, -1, -1, -1 };
        m_mainInitializedEdgeCount = 0;
        isActive = false;
        isNotGoodWay = false;
        isSearchingGoodWay = false;
        hasTimePiece = false;
        pieceIndex = -1;
    }

    public int RIUMDirection (System.Random prng) // Random Index of Uninitialized Main Direction
    {
        int skips = prng.Next(0, CardinalPoints.MainCount - m_mainInitializedEdgeCount);
        for (int i = 0; i < CardinalPoints.Count; i += 2)
        {
            if (m_edges[i] == -1)
            {
                if (skips == 0)
                {
                    return i;
                }
                skips--;
            }
        }
        throw new System.InvalidOperationException("MazeCell has no uninitialized directions left.");
    }

    public bool isMainInitialized
    {
        get
        {
            return m_edges[0] != -1 && m_edges[2] != -1 && m_edges[4] != -1 && m_edges[6] != -1;
        }
    }

    public void SetEdge(int i, int value)
    {
        if (m_edges[i] == -1)
        {
            if (i % 2 == 0)
                 m_mainInitializedEdgeCount++;

            if (value == 1)
            {
                m_wallCount++;
            }
        }
        //else
        //{
        //    throw new System.InvalidOperationException("Hopopop ! Il y a déjà un edge ici !");
        //}

        m_edges[i] = value;
    }

    public void AddTmePiece(int pieceIndex)
    {
        hasTimePiece = true;
        this.pieceIndex = pieceIndex;
    }
    public void RemoveTimePiece()
    {
        hasTimePiece = false;
        pieceIndex = -1;
    }

    public string StringEdges()
    {
        string debug = "{ ";
        for (int i = 0; i < CardinalPoints.Count - 1; i++)
        {
            debug += edges[i] + ", ";
        }
        debug += edges[7] + " }";

        return debug;
    }
}

public enum KindOfMove
{
    Central,
    Lateral
}

public class ChangeData
{
    public int iDir { get; set; }
    public KindOfMove kindOfChange { get; set; }

    public ChangeData(int iDir, KindOfMove kindOfChange)
    {
        this.iDir = iDir;
        this.kindOfChange = kindOfChange;
    }
}
