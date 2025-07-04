using System;
using UnityEngine;

public class PerlinNoise
{
    private int[] permutation;

    public PerlinNoise(int seed)
    {
        System.Random rand = new System.Random(seed);
        permutation = new int[512];
        int[] p = new int[256];

        for (int i = 0; i < 256; i++)
            p[i] = i;

        for (int i = 255; i > 0; i--)
        {
            int swapIndex = rand.Next(i + 1);
            int temp = p[i];
            p[i] = p[swapIndex];
            p[swapIndex] = temp;
        }

        for (int i = 0; i < 512; i++)
            permutation[i] = p[i & 255];
    }

    private static float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);

    private static float Lerp(float a, float b, float t) => a + t * (b - a);

    private static float Grad(int hash, float x, float y)
    {
        int h = hash & 7;
        float u = h < 4 ? x : y;
        float v = h < 4 ? y : x;
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }

    public float Noise(float x, float y)
    {
        int X = (int)Mathf.Floor(x) & 255;
        int Y = (int)Mathf.Floor(y) & 255;

        float xf = x - Mathf.Floor(x);
        float yf = y - Mathf.Floor(y);

        float u = Fade(xf);
        float v = Fade(yf);

        int aa = permutation[permutation[X] + Y];
        int ab = permutation[permutation[X] + Y + 1];
        int ba = permutation[permutation[X + 1] + Y];
        int bb = permutation[permutation[X + 1] + Y + 1];

        float x1 = Lerp(Grad(aa, xf, yf), Grad(ba, xf - 1, yf), u);
        float x2 = Lerp(Grad(ab, xf, yf - 1), Grad(bb, xf - 1, yf - 1), u);

        return Lerp(x1, x2, v);
    }
}
