

public class DesertTerrain : MyTerrain
{
    public override void Initialize(MyMap map, Location2D terrainLocation, Location2D mazeLocation, int seed, int levelOfDetail)
    {
        if (m_isInitialized)
            return;

        base.Initialize(map, terrainLocation, mazeLocation, seed, levelOfDetail);
        VerifyAttributes();

        m_isInitialized = true;
    }

    public override void VerifyAttributes()
    {
        base.VerifyAttributes();
    }

    public override void Generate()
    {
        // Generate Height Map
        m_heightMap = GenerateHeightMap();

        // Create Texture
        CreateTexture();

        // Create mesh
        CreateMesh();

        // Create Elements
        CreateElements();
    }
}
