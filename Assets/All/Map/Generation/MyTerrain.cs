using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public abstract class MyTerrain : MonoBehaviour
{
    public enum TerrainPlace
    {
        TERRAIN, // All the terrain
        EMPTY_SPACE,
        MAZE_AND_SPAWN_FINISH_LOCATION,
        MAZE,
        NOTHING
    }

    [System.Serializable]
    public struct TerrainElement
    {
        [SerializeField]
        private string m_name;
        public string name { get { return m_name; } }
        [SerializeField]
        private List<GameObject> m_prefabs;
        public List<GameObject> prefabs { get { return m_prefabs; } }
        [SerializeField]
        private float m_intensity;
        public float intensity { get { return m_intensity; } }
        [SerializeField]
        private TerrainPlace m_notIn;
        public TerrainPlace notIn { get { return m_notIn; } }
    }

    public Location2D[] ToLocations(TerrainPlace place)
    {
        if (place == TerrainPlace.TERRAIN)
        {
            return new Location2D[1] { m_terrainLocation };
        }
        else if (place == TerrainPlace.EMPTY_SPACE)
        {
            return new Location2D[1] { m_emptySpace };
        }
        else if (place == TerrainPlace.MAZE_AND_SPAWN_FINISH_LOCATION)
        {
            return new Location2D[3] { m_mazeLocation, SpawnLocation, FinishLocation };
        }
        else if (place == TerrainPlace.MAZE)
        {
            return new Location2D[1] { m_mazeLocation };
        }
        else
        {
            return new Location2D[0];
        }
    }

    // Settings
    [SerializeField]
    protected float m_scaleMultiplicator = 3;
    [SerializeField]
    protected IntVector2 m_emptySpaceSize;
    protected Location2D m_emptySpace;
    //[SerializeField]
    //protected IntVector2 m_terrainAddingSize;
    [SerializeField]
    protected float m_meshHeightMultiplier = 8;
    [SerializeField]
    protected AnimationCurve m_terrainCurve;
    [SerializeField]
    protected TerrainType[] m_regions;
    [SerializeField]
    protected int m_noiseScale = 8;
    [SerializeField]
    protected int m_octaves = 4;
    [SerializeField]
    protected float m_persistance = 0.5f;
    [SerializeField]
    protected float m_lacunarity = 2f;
    [SerializeField]
    protected Vector2 m_offset = new Vector2(0, 0);
    [SerializeField]
    protected AnimationCurve m_meshHeightCurve;
    [SerializeField]
    protected List<TerrainElement> m_elements;
    [SerializeField]
    private Color m_wayColour;

    // References
    protected Renderer m_textureRenderer;
    protected MeshFilter m_meshFilter;
    protected MeshRenderer m_meshRenderer;
    protected MeshCollider m_meshCollider;

    // Attributes
    protected MyMap m_map;
    protected Location2D m_mazeLocation;
    protected Location2D m_terrainLocation;
    public IntVector2 size { get { return m_terrainLocation.Size; } }
    public int width { get { return m_terrainLocation.Size.x; } }
    public int height { get { return m_terrainLocation.Size.y; } }
    protected int m_seed = 0;
    protected System.Random m_prng;
    protected int m_levelOfDetail = 5;
    protected float[,] m_heightMap = null;
    protected List<GameObject> m_elementInstances;

    protected bool m_isInitialized = false;

    public Location2D SpawnLocation
    {
        get
        {
            return m_map.SpawnLocation;
        }
    }

    public Location2D FinishLocation
    {
        get
        {
            return m_map.FinishLocation;
        }
    }

    public virtual void Initialize(MyMap map, Location2D terrainLocation, Location2D mazeLocation, int seed, int levelOfDetail)
    {
        if (m_isInitialized)
            return;

        m_seed = seed;
        m_levelOfDetail = levelOfDetail;
        m_map = map;

        m_prng = new System.Random(seed);
        m_textureRenderer = GetComponent<Renderer>();
        m_meshFilter = GetComponent<MeshFilter>();
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_meshCollider = GetComponent<MeshCollider>();


        m_terrainLocation = terrainLocation;
        m_mazeLocation = mazeLocation;
        m_emptySpace = m_mazeLocation + (m_emptySpaceSize * 2 * levelOfDetail);

        //Debug.Log("mazeLocation : " + m_mazeLocation);
        //Debug.Log("mazeSize : " + m_mazeLocation.Size);
        //Debug.Log("terrainLocation : " + m_terrainLocation);
        //Debug.Log("mazeLocation : " + m_mazeLocation);
        //Debug.Log("emptySpace : " + m_emptySpace);
        //Debug.Log("spawnLocation : " + SpawnLocation);
        //Debug.Log("mazeSpawnLocation : " + m_map.MazeInstance.SpawnLocation);
        //Debug.Log("departureCoord : " + m_map.MazeInstance.DepartureCoord);
        //Debug.Log("finishLocation : " + FinishLocation);
    }

    public abstract void Generate();

    public virtual void VerifyAttributes()
    {
        if (m_mazeLocation.Size.x < 0 || m_mazeLocation.Size.y < 0)
        {
            throw new System.Exception("Le labyrinthe est trop petit.");
        }
        if (m_mazeLocation.Size.x >= 255 || m_mazeLocation.Size.y >= 255)
        {
            throw new System.Exception("Le labyrinthe est trop grand.");
        }
        //if (m_mazeLocation.Size.x >= 256 || m_mazeLocation.Size.y >= 256)
        //{
        //    m_terrainAddingSize = new IntVector2(254 - m_mazeLocation.Size.x, 254 - m_mazeLocation.Size.y);
        //    m_terrainLocation = new Location2D(IntVector2.Zero, m_mazeLocation.Size + m_terrainAddingSize);
        //}

        while (m_levelOfDetail * width >= 256 || m_levelOfDetail * height >= 256)
        {
            m_levelOfDetail -= 1;
        }
            
        if (m_levelOfDetail < 1)
        {
            m_levelOfDetail = 1;
        }
    }

    public virtual void Delete()
    {
        if (!m_isInitialized)
            return;

        m_meshFilter.sharedMesh = new Mesh();
        m_meshFilter.sharedMesh = new Mesh();

        m_isInitialized = false;
    }

    public Vector3 ToRealPosition(int x, float y, int z)
    {
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;
        return new Vector3(-(topLeftX + x) / m_levelOfDetail, m_meshHeightCurve.Evaluate(y) * m_meshHeightMultiplier, -(topLeftZ - z) / m_levelOfDetail) * m_scaleMultiplicator;
    }

    public virtual float[,] GenerateHeightMap()
    {
        if (!m_isInitialized)
            throw new System.Exception(this + " n'est pas initialisé.");

        float[,] noiseMap = Noise.GenerateNoiseMap(size, m_seed, m_noiseScale, m_octaves, m_persistance, m_lacunarity, m_offset, m_levelOfDetail);
        float centerX = (width - 1f) / 2f; // Here if I want to have a mapSize
        float centerZ = (height - 1f) / 2f; // Here if I want to have a mapSize

        float[,] heightMap = new float[width, height];

        float maxDst = Mathf.Max(centerZ, centerX);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float mazeDstLerp = Mathf.InverseLerp(0, maxDst, m_emptySpace.Distance(m_terrainLocation.firstCoord.x + x, m_terrainLocation.firstCoord.y + y));
                heightMap[x, y] = noiseMap[x, y] * m_terrainCurve.Evaluate(mazeDstLerp);
            }
        }

        return heightMap;
    }

    public virtual void CreateTexture()
    {
        if (!m_isInitialized)
            throw new System.Exception(this + " n'est pas initialisé.");

        if (m_heightMap == null)
            throw new System.Exception("m_heightMap est null");

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (m_wayColour != null && (m_mazeLocation.Contain(x, y) || SpawnLocation.Contain(x, y) || FinishLocation.Contain(x, y)))
                {
                    colourMap[y * width + x] = m_wayColour;
                }
                else
                {
                    float currentHeight = m_heightMap[x, y];
                    for (int i = 0; i < m_regions.Length; i++)
                    {
                        if (currentHeight <= m_regions[i].height)
                        {
                            colourMap[y * width + x] = m_regions[i].colour;
                            break;
                        }
                    }
                }
            }
        }

        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colourMap);
        texture.Apply();
        m_textureRenderer.sharedMaterial.mainTexture = texture;
    }

    public virtual void CreateMesh()
    {
        if (!m_isInitialized)
            throw new System.Exception(this + " n'est pas initialisé.");

        if (m_heightMap == null)
            throw new System.Exception("m_heightMap est null");

        Vector3[] vertices = new Vector3[width * height];
        int[] triangles = new int[(width - 1) * (height - 1) * 6];
        Vector2[] uv = new Vector2[width * height];

        int vertexIndex = 0, triangleIndex = 0;

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                vertices[vertexIndex] = ToRealPosition(x, m_heightMap[x, z], z);
                uv[vertexIndex] = new Vector2(x / (float)width, z / (float)height);

                if (x < width - 1 && z < height - 1)
                {
                    triangles[triangleIndex] = vertexIndex;
                    triangles[triangleIndex + 1] = vertexIndex + width + 1;
                    triangles[triangleIndex + 2] = vertexIndex + width;
                    triangles[triangleIndex + 3] = vertexIndex + width + 1;
                    triangles[triangleIndex + 4] = vertexIndex;
                    triangles[triangleIndex + 5] = vertexIndex + 1;
                    triangleIndex += 6;
                }
                vertexIndex++;
            }
        }

        m_meshFilter.sharedMesh.vertices = vertices;
        m_meshFilter.sharedMesh.triangles = triangles;
        m_meshFilter.sharedMesh.uv = uv;
        m_meshCollider.sharedMesh = m_meshFilter.sharedMesh;
    }

    protected void CreateElements()
    {
        if (m_heightMap == null)
            throw new System.Exception("m_heightMap est null");

        if (m_elements == null || m_elements.Count == 0)
            return;

        m_elementInstances = new List<GameObject>();

        for (int z = 0; z < height; ++z)
        {
            for (int x = 0; x < width; ++x)
            {
                float y = m_heightMap[x, z];
                if (y <= 0.2f) // In the plains
                {
                    double random = m_prng.NextDouble();
                    foreach (TerrainElement element in m_elements)
                    {
                        if (random <= element.intensity && !ToLocations(element.notIn).Contain(x, z))
                        {
                            GameObject elementInstance = Instantiate(element.prefabs[m_prng.Next(0, element.prefabs.Count)], ToRealPosition(x, y, z), Quaternion.Euler(0, m_prng.Next(0, 360), 0), transform);
                            //elementInstance.gameObject.isStatic = true;
                            m_elementInstances.Add(elementInstance);
                            break;
                        }
                    }
                }
            }
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}
