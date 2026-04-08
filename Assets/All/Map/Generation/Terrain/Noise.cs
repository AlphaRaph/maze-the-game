using UnityEngine;

public class Noise
{
    public static float[,] GenerateNoiseMap(IntVector2 mapSize, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, int levelOfDetail)
    {
        float[,] noiseMap = new float[mapSize.x, mapSize.y];

        // Random number generator
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapSize.x / 2f;
        float halfHeight = mapSize.y / 2f;

        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / (scale * levelOfDetail) * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / (scale * levelOfDetail) * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                // Return a value beetween 0 and 1 
                // ex : min = 700, max = 300 and perlinValue = 200 -> return 0.9f
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }

    public static float[,] GenerateNoiseMapWithCurve(IntVector2 mapSize, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, int levelOfDetail)
    {
        float[,] noiseMap = new float[mapSize.x, mapSize.y];

        // Random number generator
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapSize.x / 2f;
        float halfHeight = mapSize.y / 2f;

        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / (scale * levelOfDetail) * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / (scale * levelOfDetail) * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                // Return a value beetween -1 and 1 
                // ex : min = 700, max = 300 and perlinValue = 200 -> return 0.9f
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }

    public static float[,] InverseLerpHeightMap(float [,] _heightMap)
    {
        // Get the min and the max value of the map
        float minValue = Mathf.Infinity;
        float maxValue = Mathf.NegativeInfinity;

        for (int z = 0; z < _heightMap.GetLength(1); z++)
        {
            for (int x = 0; x < _heightMap.GetLength(0); x++)
            {
                minValue = Mathf.Min(_heightMap[x, z], minValue);
                maxValue = Mathf.Max(_heightMap[x, z], maxValue);
            }
        }

        // Transform the height map
        float[,] heightMap = new float[_heightMap.GetLength(0), _heightMap.GetLength(1)];

        for (int z = 0; z < _heightMap.GetLength(1); z++)
        {
            for (int x = 0; x < _heightMap.GetLength(0); x++)
            {
                heightMap[x, z] = Mathf.InverseLerp(minValue, maxValue, _heightMap[x, z]);
            }
        }

        return heightMap;
    }

    public static float[,] HeightMapFromTerrainCurve(float[,] _heightMap, Location2D emptySpace, AnimationCurve terrainCurve)
    {
        int width = _heightMap.GetLength(0);
        int height = _heightMap.GetLength(1);
        float centerX = (width - 1f) / 2f;
        float centerZ = (height - 1f) / 2f;

        float[,] heightMap = new float[width, height];

        float maxDst = Mathf.Max(centerZ, centerX);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float mazeDstLerp = Mathf.InverseLerp(0, maxDst, emptySpace.Distance(x, y));
                heightMap[x, y] = _heightMap[x, y] * terrainCurve.Evaluate(mazeDstLerp);
            }
        }

        return heightMap;
        //return InverseLerpHeightMap(heightMap);
    }
}
