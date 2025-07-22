using UnityEngine;
using System;

public static class NoiseGenerator
{
    public static float[,] GenerateNoiseMap(
        int width,
        int height,
        float magnification,
        int octaves,
        float persistence,
        float lacunarity,
        int seed,
        NoiseType noiseType)
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
            magnification = 0.0001f;

        CustomPerlin2D customPerlin = new CustomPerlin2D(seed);
        CustomValue2D customValue = new CustomValue2D(seed);

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

                    float rawNoise = 0f;
                    if (noiseType == NoiseType.Perlin)
                    {
                        rawNoise = Mathf.PerlinNoise(sampleX, sampleY) * 2f - 1f;
                    }
                    else if (noiseType == NoiseType.Value)
                    {
                        rawNoise = customValue.Noise(sampleX, sampleY) * 2f - 1f;
                    }

                    noiseHeight += rawNoise * amplitude;
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                noiseMap[x, y] = Mathf.InverseLerp(-1f, 1f, noiseHeight);
            }
        }

        return noiseMap;
    }
}
