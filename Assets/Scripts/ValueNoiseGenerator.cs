using UnityEngine;

public static class ValueNoiseGenerator
{
    public static float[,] GenerateNoiseMap(int width, int height, float magnification, int octaves, float persistence, float lacunarity, int seed)
    {
        float[,] noiseMap = new float[width, height];

        System.Random rand = new System.Random(seed);

        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rand.Next(-100000, 100000);
            float offsetY = rand.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (magnification <= 0)
        {
            magnification = 0.0001f;
        }

        // Precompute grids of random values per octave
        float[][,] grids = new float[octaves][,];
        for (int i = 0; i < octaves; i++)
        {
            int gridWidth = Mathf.CeilToInt(width / magnification * Mathf.Pow(lacunarity, i)) + 2;
            int gridHeight = Mathf.CeilToInt(height / magnification * Mathf.Pow(lacunarity, i)) + 2;

            grids[i] = GenerateValueGrid(gridWidth, gridHeight, rand);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 0f;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - (width / 2f)) / magnification * frequency + octaveOffsets[i].x;
                    float sampleY = (y - (height / 2f)) / magnification * frequency + octaveOffsets[i].y;

                    float valueNoise = ValueNoise(sampleX, sampleY, grids[i]);
                    noiseHeight += valueNoise * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                noiseMap[x, y] = Mathf.InverseLerp(-1f, 1f, noiseHeight);
            }
        }

        return noiseMap;
    }

    static float[,] GenerateValueGrid(int width, int height, System.Random rand)
    {
        float[,] grid = new float[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                grid[i, j] = (float)(rand.NextDouble() * 2 - 1);
            }
        }
        return grid;
    }

    static float SmoothStep(float t)
    {
        return t * t * (3 - 2 * t);
    }

    static float Lerp(float a, float b, float t)
    {
        return a + t * (b - a);
    }

    static float ValueNoise(float x, float y, float[,] grid)
    {
        int x0 = Mathf.FloorToInt(x);
        int x1 = x0 + 1;
        int y0 = Mathf.FloorToInt(y);
        int y1 = y0 + 1;

        x0 = Mathf.Clamp(x0, 0, grid.GetLength(0) - 1);
        x1 = Mathf.Clamp(x1, 0, grid.GetLength(0) - 1);
        y0 = Mathf.Clamp(y0, 0, grid.GetLength(1) - 1);
        y1 = Mathf.Clamp(y1, 0, grid.GetLength(1) - 1);

        float sx = x - Mathf.Floor(x);
        float sy = y - Mathf.Floor(y);

        float ix0 = Lerp(grid[x0, y0], grid[x1, y0], SmoothStep(sx));
        float ix1 = Lerp(grid[x0, y1], grid[x1, y1], SmoothStep(sx));
        float value = Lerp(ix0, ix1, SmoothStep(sy));

        return value;
    }
}
