using UnityEngine;

public static class NoiseStats
{
    public static void PrintElevationStats(float[,] map)
    {
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        int count = width * height;

        float min = float.MaxValue;
        float max = float.MinValue;
        float sum = 0f;
        float sumSq = 0f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float val = map[x, y];
                if (val < min) min = val;
                if (val > max) max = val;
                sum += val;
                sumSq += val * val;
            }
        }

        float mean = sum / count;
        float variance = (sumSq / count) - (mean * mean);
        float stdDev = Mathf.Sqrt(variance);

        Debug.Log($"Map stats:\nMin: {min:F4}, Max: {max:F4}, Mean: {mean:F4}, Std Dev: {stdDev:F4}");
    }

    public static void PrintHistogram(float[,] map, int bins = 10)
    {
        int[] histogram = new int[bins];
        int width = map.GetLength(0);
        int height = map.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int index = Mathf.Clamp((int)(map[x, y] * bins), 0, bins - 1);
                histogram[index]++;
            }
        }

        for (int i = 0; i < bins; i++)
        {
            float rangeStart = (float)i / bins;
            float rangeEnd = (float)(i + 1) / bins;
            Debug.Log($"[{rangeStart:F1} - {rangeEnd:F1}]: {histogram[i]} values");
        }
    }

}
