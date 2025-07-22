using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class TileMapGenerator : MonoBehaviour
{
    public NoiseType noiseType = NoiseType.Perlin;
    public BiomeType biomeType = BiomeType.None;

    public int mapWidth = 100;
    public int mapHeight = 100;
    public float magnification = 7f;

    public int octaves = 4;
    public float persistence = 0.5f;
    public float lacunarity = 2f;

    public int seed = 0;
    public int testIterations = 10;

    public float seaLevel = 0.3f;
    public float mountainLevel = 0.8f;
    public bool biomeMap = false;

    public int voronoiSiteCount = 5;
    public List<VoronoiBiome> voronoiBiomes;
    public List<TileLayer> tileLayers;
    public List<ClimateBiome> biomes;

    private Dictionary<int, GameObject> tileGroups;
    private GameObject biomeGroup;
    private float[,] lastGeneratedMap;

    private ClimateBiomeHelper biomeHelper;
    private VoronoiBiomeHelper voronoiGenerator;

    public void GenerateMapInEditor()
    {
        ClearMap();
        CreateTileGroups();

        float[,] elevationMap = NoiseGenerator.GenerateNoiseMap(
            mapWidth, mapHeight, magnification,
            octaves, persistence, lacunarity,
            seed, noiseType);

        NoiseStats.PrintElevationStats(elevationMap);
        NoiseStats.PrintHistogram(elevationMap, 10);

        switch (biomeType)
        {
            case BiomeType.ClimateBased:
                biomeHelper = new ClimateBiomeHelper(biomes, seaLevel, mountainLevel);

                float[,] tempMap = NoiseGenerator.GenerateNoiseMap(
                    mapWidth, mapHeight, magnification,
                    octaves, persistence, lacunarity,
                    seed + 1000, noiseType);

                float[,] moistMap = NoiseGenerator.GenerateNoiseMap(
                    mapWidth, mapHeight, magnification,
                    octaves, persistence, lacunarity,
                    seed + 2000, noiseType);

                if (biomeMap)
                    GenerateBiomeMap(elevationMap, tempMap, moistMap, BiomeType.ClimateBased);
                else
                    GenerateCombinedMap(elevationMap, tempMap, moistMap);
                break;

            case BiomeType.Voronoi:
                if (biomeMap)
                    GenerateBiomeMap(elevationMap, null, null, BiomeType.Voronoi);
                else
                    GenerateVoronoiMap(elevationMap);
                break;

            default:
                GenerateStandardMap(elevationMap);
                break;
        }
    }

    void CreateTileGroups()
    {
        tileGroups = new Dictionary<int, GameObject>();
        for (int i = 0; i < tileLayers.Count; i++)
        {
            GameObject prefab = tileLayers[i].prefab;
            GameObject group = new GameObject(prefab != null ? prefab.name : $"Layer_{i}");
            group.transform.parent = this.transform;
            tileGroups[i] = group;
        }

        biomeGroup = new GameObject("Biomes");
        biomeGroup.transform.parent = this.transform;
    }

    void GenerateStandardMap(float[,] elevationMap)
    {
        int width = elevationMap.GetLength(0);
        int height = elevationMap.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int tileID = GetTileIdFromNoise(elevationMap[x, y]);
                CreateTile(tileID, x, y);
            }
        }
    }

    void GenerateCombinedMap(float[,] elevationMap, float[,] temperatureMap, float[,] moistureMap)
    {
        int width = elevationMap.GetLength(0);
        int height = elevationMap.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float elev = elevationMap[x, y];
                float temp = temperatureMap[x, y];
                float moist = moistureMap[x, y];

                var biome = biomeHelper.GetBiome(elev, temp, moist);
                if (biome != null)
                {
                    GameObject tile = Instantiate(biome.prefab, biomeGroup.transform);
                    tile.name = $"Biome_{biome.name}_x[{x}]_y[{y}]";
                    tile.transform.localPosition = new Vector3(x, y, 0);
                }
                else
                {
                    int tileID = GetTileIdFromNoise(elev);
                    CreateTile(tileID, x, y);
                }
            }
        }
    }

    void GenerateBiomeMap(float[,] elevationMap, float[,] temperatureMap, float[,] moistureMap, BiomeType biomeType)
    {
        switch (biomeType)
        {
            case BiomeType.ClimateBased:
                for (int x = 0; x < mapWidth; x++)
                {
                    for (int y = 0; y < mapHeight; y++)
                    {
                        float temp = temperatureMap[x, y];
                        float moist = moistureMap[x, y];

                        ClimateBiome biome = biomeHelper.GetBiome(temp, moist);
                        if (biome != null && biome.prefab != null)
                        {
                            GameObject tile = Instantiate(biome.prefab, this.transform);
                            tile.name = $"Biome_{biome.name}_x[{x}]_y[{y}]";
                            tile.transform.localPosition = new Vector3(x, y, 0);
                        }
                    }
                }
                break;

            case BiomeType.Voronoi:
                var generator = new VoronoiBiomeHelper();
                int adjustedSites = Mathf.Max(5, Mathf.RoundToInt(mapWidth * mapHeight / (20f * magnification)));
                UnityEngine.Debug.Log($"Voronoi Site Count: {adjustedSites}");
                generator.SetBiomes(voronoiBiomes, mapWidth, mapHeight, seed, adjustedSites);

                for (int x = 0; x < mapWidth; x++)
                {
                    for (int y = 0; y < mapHeight; y++)
                    {
                        var biome = generator.GetBiomeAt(x, y);
                        if (biome != null && biome.prefab != null)
                        {
                            GameObject tile = Instantiate(biome.prefab, this.transform);
                            tile.name = $"Voronoi_{biome.name}_x[{x}]_y[{y}]";
                            tile.transform.localPosition = new Vector3(x, y, 0);
                        }
                    }
                }
                break;
        }
    }

    void GenerateVoronoiMap(float[,] elevationMap)
    {
        if (voronoiGenerator == null)
            voronoiGenerator = new VoronoiBiomeHelper();

        int adjustedSites = Mathf.Max(5, Mathf.RoundToInt(mapWidth * mapHeight / (20f * magnification)));
        voronoiGenerator.SetBiomes(voronoiBiomes, mapWidth, mapHeight, seed, adjustedSites);
        UnityEngine.Debug.Log($"Voronoi Site Count: {adjustedSites}");

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float elev = elevationMap[x, y];

                if (elev < seaLevel || elev > mountainLevel)
                {
                    int tileID = GetTileIdFromNoise(elev);
                    CreateTile(tileID, x, y);
                    continue;
                }

                var biome = voronoiGenerator.GetBiomeAt(x, y);
                if (biome != null)
                {
                    GameObject tile = Instantiate(biome.prefab, biomeGroup.transform);
                    tile.name = $"VoronoiBiome_{biome.name}_x[{x}]_y[{y}]";
                    tile.transform.localPosition = new Vector3(x, y, 0);
                }
                else
                {
                    int tileID = GetTileIdFromNoise(elev);
                    CreateTile(tileID, x, y);
                }
            }
        }
    }

    int GetTileIdFromNoise(float noiseValue)
    {
        for (int i = 0; i < tileLayers.Count; i++)
        {
            if (noiseValue <= tileLayers[i].threshold)
                return i;
        }

        return tileLayers.Count - 1;
    }

    void CreateTile(int id, int x, int y)
    {
        GameObject tile = Instantiate(tileLayers[id].prefab, tileGroups[id].transform);
        tile.name = $"Tile_x[{x}]_y[{y}]";
        tile.transform.localPosition = new Vector3(x, y, 0);
    }

    public (long timeMs, long memoryBytes) GenerateMapAndMeasurePerformance()
    {
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
        System.GC.Collect();

        long memoryBefore = System.GC.GetTotalMemory(true);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        lastGeneratedMap = NoiseGenerator.GenerateNoiseMap(
            mapWidth, mapHeight, magnification,
            octaves, persistence, lacunarity,
            seed, noiseType);

        stopwatch.Stop();
        long memoryAfter = System.GC.GetTotalMemory(false);

        return (stopwatch.ElapsedMilliseconds, memoryAfter - memoryBefore);
    }

    public void MeasureAveragePerformance(NoiseType noiseType)
    {
        long totalTime = 0;
        long totalMemory = 0;

        for (int i = 0; i < testIterations; i++)
        {
            var result = GenerateMapAndMeasurePerformance();
            totalTime += result.timeMs;
            totalMemory += result.memoryBytes;
        }

        float avgTime = totalTime / (float)testIterations;
        float avgMemoryKB = totalMemory / (float)testIterations / 1024f;

        UnityEngine.Debug.Log($"{noiseType} average generation time over {testIterations} runs: {avgTime} ms");
        UnityEngine.Debug.Log($"{noiseType} average memory used over {testIterations} runs: {avgMemoryKB:F2} KB");
    }

    public void ClearMap()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
