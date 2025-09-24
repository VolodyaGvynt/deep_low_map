using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class TileMapGenerator : MonoBehaviour
{
    public NoiseType noiseType = NoiseType.Perlin;
    public BiomeType biomeType = BiomeType.None;
    public TownType townType = TownType.None;


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
    public List<VoronoiBiome> voronoiBiomes = new List<VoronoiBiome>();
    public List<TileLayer> tileLayers = new List<TileLayer>();
    public List<ClimateBiome> biomes = new List<ClimateBiome>();


    private Dictionary<int, GameObject> tileGroups;
    private GameObject biomeGroup;
    private float[,] lastGeneratedMap;

    private ClimateBiomeHelper biomeHelper;
    private VoronoiBiomeHelper voronoiGenerator;

    public LTownGenerator lTownGenerator;
    public GridTownGenerator gridTownGenerator;

    private Dictionary<string, List<Vector2Int>> biomeTileMap = new();

    public void GenerateMapInEditor()
    {
        ClearMap();
        CreateTileGroups();
        biomeTileMap.Clear();

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
                    GenerateClimateMap(elevationMap, tempMap, moistMap);
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

        if (townType == TownType.LSystem)
        {
            SpawnTownInBiome("Snow");
        }
        else if (townType == TownType.GridBased)
        {
            SpawnTownInBiome("Snow");
        }
        ;
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

    void GenerateClimateMap(float[,] elevationMap, float[,] temperatureMap, float[,] moistureMap)
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
                if (biome != null && biome.prefab != null)
                {
                    GameObject tile = Instantiate(biome.prefab, biomeGroup.transform);
                    tile.name = $"Biome_{biome.name}_x[{x}]_y[{y}]";
                    tile.transform.localPosition = new Vector3(x, y, -0.4f);

                    if (!biomeTileMap.ContainsKey(biome.name))
                        biomeTileMap[biome.name] = new List<Vector2Int>();

                    biomeTileMap[biome.name].Add(new Vector2Int(x, y));
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

                            if (!biomeTileMap.ContainsKey(biome.name))
                                biomeTileMap[biome.name] = new List<Vector2Int>();

                            biomeTileMap[biome.name].Add(new Vector2Int(x, y));
                        }
                    }
                }
                //PrintBiomeStatistics();
                break;

            case BiomeType.Voronoi:
                var generator = new VoronoiBiomeHelper();
                int adjustedSites = Mathf.Max(5, Mathf.RoundToInt(mapWidth * mapHeight / (20f * magnification)));
                //UnityEngine.Debug.Log($"Voronoi Site Count: {adjustedSites}");
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

                            if (!biomeTileMap.ContainsKey(biome.name))
                                biomeTileMap[biome.name] = new List<Vector2Int>();

                            biomeTileMap[biome.name].Add(new Vector2Int(x, y));
                        }
                    }
                }
                //PrintBiomeStatistics();
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

                    if (!biomeTileMap.ContainsKey(biome.name))
                        biomeTileMap[biome.name] = new List<Vector2Int>();

                    biomeTileMap[biome.name].Add(new Vector2Int(x, y));
                }
                else
                {
                    int tileID = GetTileIdFromNoise(elev);
                    CreateBiomeTile(tileID, x, y);
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
        tile.transform.localPosition = new Vector3(x, y, -0.6f);
    }

    void CreateBiomeTile(int id, int x, int y)
    {
        GameObject tile = Instantiate(tileLayers[id].prefab, tileGroups[id].transform);
        tile.name = $"Tile_x[{x}]_y[{y}]";
        tile.transform.localPosition = new Vector3(x, y, 0);
    }

    public void SpawnTownInBiome(string biomeName)
    {
        if (!biomeTileMap.ContainsKey(biomeName) || biomeTileMap[biomeName].Count == 0)
        {
            UnityEngine.Debug.LogWarning($"Biome '{biomeName}' not found or empty — cannot place town.");
            return;
        }

        List<Vector2Int> tiles = biomeTileMap[biomeName];

        Vector2 average = Vector2.zero;
        foreach (var tile in tiles)
        {
            average += new Vector2(tile.x, tile.y);
        }
        average /= tiles.Count;

        Vector2Int center = new Vector2Int(Mathf.RoundToInt(average.x), Mathf.RoundToInt(average.y));
        Vector3 worldPos = new Vector3(center.x, center.y, 0);

        GameObject townRoot = new GameObject($"{biomeName}_Town");
        townRoot.transform.position = worldPos;

        if (townType == TownType.LSystem && lTownGenerator != null)
        {
            var lGen = Instantiate(lTownGenerator, worldPos, Quaternion.identity);
            lGen.name = $"{biomeName}_LTown";
            lGen.GenerateLTown(lGen.lsystem.GenerateSequence());
        }
        else if (townType == TownType.GridBased && gridTownGenerator != null)
        {
            var gGen = Instantiate(gridTownGenerator, worldPos, Quaternion.identity);
            gGen.name = $"{biomeName}_GridTown";
            gGen.GenerateGridTown();
        }
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

    public void PrintBiomeStatistics()
    {
        UnityEngine.Debug.Log("=== Biome Statistics ===");
        foreach (var kvp in biomeTileMap)
        {
            string name = kvp.Key;
            int count = kvp.Value.Count;
            float percentage = (count / (float)(mapWidth * mapHeight)) * 100f;
            UnityEngine.Debug.Log($"{name}: {count} tiles ({percentage:F2}%)");
        }
    }

    public (long timeMs, long memoryBytes) GenerateBiomeMapAndMeasurePerformance(BiomeType biomeType)
    {
        float[,] elevationMap = NoiseGenerator.GenerateNoiseMap(
            mapWidth, mapHeight, magnification, octaves, persistence, lacunarity, seed, noiseType);

        float[,] tempMap = null;
        float[,] moistMap = null;

        if (biomeType == BiomeType.ClimateBased)
        {
            biomeHelper = new ClimateBiomeHelper(biomes, seaLevel, mountainLevel);
            tempMap = NoiseGenerator.GenerateNoiseMap(
                mapWidth, mapHeight, magnification, octaves, persistence, lacunarity, seed + 1000, noiseType);
            moistMap = NoiseGenerator.GenerateNoiseMap(
                mapWidth, mapHeight, magnification, octaves, persistence, lacunarity, seed + 2000, noiseType);
        }

        ClearMap();
        CreateTileGroups();
        biomeTileMap.Clear();

        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
        System.GC.Collect();

        long memoryBefore = System.GC.GetTotalMemory(true);
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        GenerateBiomeMap(elevationMap, tempMap, moistMap, biomeType);

        stopwatch.Stop();
        long memoryAfter = System.GC.GetTotalMemory(false);

        return (stopwatch.ElapsedMilliseconds, memoryAfter - memoryBefore);
    }

    public void MeasureAverageBiomePerformance(BiomeType biomeType)
    {
        long totalTime = 0;
        long totalMemory = 0;

        for (int i = 0; i < testIterations; i++)
        {
            var result = GenerateBiomeMapAndMeasurePerformance(biomeType);
            totalTime += result.timeMs;
            totalMemory += result.memoryBytes;
        }

        float avgTime = totalTime / (float)testIterations;
        float avgMemoryKB = totalMemory / (float)testIterations / 1024f;

        UnityEngine.Debug.Log($"=== {biomeType} Biome Generation Performance ===");
        UnityEngine.Debug.Log($"Average Time: {avgTime:F2} ms");
        UnityEngine.Debug.Log($"Average Memory: {avgMemoryKB:F2} KB");
    }




    public void ClearMap()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
