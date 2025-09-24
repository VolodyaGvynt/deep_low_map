using UnityEngine;
using System.Collections.Generic;

public class VoronoiBiomeHelper
{
    private List<VoronoiBiome> biomes = new List<VoronoiBiome>();
    private Dictionary<Vector2Int, VoronoiBiome> biomeMap = new Dictionary<Vector2Int, VoronoiBiome>();
    private List<(Vector2Int position, VoronoiBiome biome)> sites = new List<(Vector2Int, VoronoiBiome)>();

    private int mapWidth;
    private int mapHeight;

    public void SetBiomes(List<VoronoiBiome> availableBiomes, int width, int height, int seed, int siteCount)
    {
        if (availableBiomes == null || availableBiomes.Count == 0)
        {
            Debug.LogError("No biomes assigned to Voronoi generator!");
            return;
        }

        biomes = availableBiomes;
        mapWidth = width;
        mapHeight = height;
        biomeMap.Clear();
        sites.Clear();

        System.Random rand = new System.Random(seed);

        List<VoronoiBiome> usedBiomes = new List<VoronoiBiome>();
        foreach (var biome in biomes)
        {
            Vector2Int sitePos = new Vector2Int(rand.Next(0, mapWidth), rand.Next(0, mapHeight));
            sites.Add((sitePos, biome));
            usedBiomes.Add(biome);
        }

        int remainingSites = siteCount - usedBiomes.Count;
        for (int i = 0; i < remainingSites; i++)
        {
            Vector2Int sitePos = new Vector2Int(rand.Next(0, mapWidth), rand.Next(0, mapHeight));
            VoronoiBiome randomBiome = biomes[rand.Next(0, biomes.Count)];
            sites.Add((sitePos, randomBiome));
        }

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float minDist = float.MaxValue;
                VoronoiBiome closestBiome = null;
                Vector2Int point = new Vector2Int(x, y);

                foreach (var (sitePos, biome) in sites)
                {
                    float dist = Vector2Int.Distance(sitePos, point);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closestBiome = biome;
                    }
                }

                biomeMap[point] = closestBiome;
            }
        }
    }


    public VoronoiBiome GetBiomeAt(int x, int y)
    {
        var key = new Vector2Int(x, y);
        if (biomeMap.TryGetValue(key, out var biome))
        {
            return biome;
        }

        return null;
    }
}
