using System.Collections.Generic;
using UnityEngine;

public class CustomValue2D
{
    private readonly int[] hashList;
    private readonly int mask;

    public CustomValue2D(int seed = 0, int hashSize = 256)
    {
        hashList = GenerateHashList(seed, hashSize);
        mask = hashList.Length - 1;
    }

    public float Noise(float x, float y)
    {


        int x0 = Mathf.FloorToInt(x);
        int y0 = Mathf.FloorToInt(y);

        float tx = x - x0;
        float ty = y - y0;

        tx = Smooth(tx);
        ty = Smooth(ty);

        int x1 = (x0 + 1) & mask;
        int y1 = (y0 + 1) & mask;
        x0 &= mask;
        y0 &= mask;

        int h00 = Hash(x0, y0);
        int h10 = Hash(x1, y0);
        int h01 = Hash(x0, y1);
        int h11 = Hash(x1, y1);

        float v00 = h00 / (float)mask;
        float v10 = h10 / (float)mask;
        float v01 = h01 / (float)mask;
        float v11 = h11 / (float)mask;

        float u = Mathf.Lerp(v00, v10, tx);
        float v = Mathf.Lerp(v01, v11, tx);
        return Mathf.Lerp(u, v, ty);
    }

    private int Hash(int x, int y)
    {
        return hashList[(hashList[x] + y) & mask];
    }

    private float Smooth(float t)
    {
        return t * t * t * (t * (t * 6f - 15f) + 10f);
    }

    private int[] GenerateHashList(int seed, int length)
    {
        System.Random rng = new System.Random(seed);
        int[] list = new int[length];

        for (int i = 0; i < length; i++)
            list[i] = i;

        for (int i = 0; i < length; i++)
        {
            int j = rng.Next(length);
            (list[i], list[j]) = (list[j], list[i]);
        }

        return list;
    }
}
