using UnityEngine;
using System.Collections.Generic;

public class MountainTerrain : MyTerrain
{
    public override void Initialize(MyMap displayInstance, Location2D terrainLocation, Location2D mazeLocation, int seed, int levelOfDetail)
    {
        if (m_isInitialized)
            return;

        base.Initialize(displayInstance, terrainLocation, mazeLocation, seed, levelOfDetail);
        VerifyAttributes();

        m_isInitialized = true;
    }

    public override void Delete()
    {
        if (!m_isInitialized)
            return;

        base.Delete();
    }

    public override void VerifyAttributes()
    {
        base.VerifyAttributes();
    }

    public override void Generate()
    {
        if (!m_isInitialized)
            throw new System.Exception(this + " n'est pas initialisé.");

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
