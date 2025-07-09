using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomPerlin2D
{
    private readonly int[] permutationTable;
    private static readonly Vector2[] gradients2D = new Vector2[]
    {
        new Vector2(1,0),
        new Vector2(-1,0),
        new Vector2(0,1),
        new Vector2(0,-1),
        new Vector2(1,1).normalized,
        new Vector2(-1,1).normalized,
        new Vector2(1,-1).normalized,
        new Vector2(-1,-1).normalized
    };

    private const int tableSize = 256;

    public CustomPerlin2D(int seed = 0)
    {
        permutationTable = GeneratePermutationTable(seed);
    }

    private int[] GeneratePermutationTable(int seed)
    {
        int[] p = new int[tableSize];
        for (int i = 0; i < tableSize; i++)
            p[i] = i;

        System.Random rng = new System.Random(seed);

        for (int i = tableSize - 1; i > 0; i--)
        {
            int swapIndex = rng.Next(i + 1);
            int temp = p[i];
            p[i] = p[swapIndex];
            p[swapIndex] = temp;
        }

        return p;
    }

    private static float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private static float Lerp(float a, float b, float t)
    {
        return a + t * (b - a);
    }

    private float Gradient(int hash, float x, float y)
    {
        int gradientIndex = hash & (gradients2D.Length - 1);
        Vector2 grad = gradients2D[gradientIndex];
        return grad.x * x + grad.y * y;
    }

    public float Noise(float x, float y)
    {
        int X = Mathf.FloorToInt(x) & 255;
        int Y = Mathf.FloorToInt(y) & 255;

        float xf = x - Mathf.Floor(x);
        float yf = y - Mathf.Floor(y);

        float u = Fade(xf);
        float v = Fade(yf);

        int aa = permutationTable[(permutationTable[X] + Y) & 255];
        int ab = permutationTable[(permutationTable[X] + Y + 1) & 255];
        int ba = permutationTable[(permutationTable[(X + 1) & 255] + Y) & 255];
        int bb = permutationTable[(permutationTable[(X + 1) & 255] + Y + 1) & 255];

        float gradAA = Gradient(aa, xf, yf);
        float gradBA = Gradient(ba, xf - 1, yf);
        float gradAB = Gradient(ab, xf, yf - 1);
        float gradBB = Gradient(bb, xf - 1, yf - 1);

        float lerpX1 = Lerp(gradAA, gradBA, u);
        float lerpX2 = Lerp(gradAB, gradBB, u);
        float result = Lerp(lerpX1, lerpX2, v);

        return (result + 1) / 2f;
    }
}
