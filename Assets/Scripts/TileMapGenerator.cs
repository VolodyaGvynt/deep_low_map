using System.Collections.Generic;
using UnityEngine;

public class TileMapGenerator : MonoBehaviour
{
    public NoiseType noiseType = NoiseType.Perlin;

    public int mapWidth = 100;
    public int mapHeight = 100;
    public float magnification = 7f;

    

    public int perlinOctaves = 4;
    public float perlinPersistence = 0.5f;
    public float perlinLacunarity = 2f;

    public float valueFrequency = 1f;
    public int seed = 0;

    public bool useBiomes = false;

    public List<TileLayer> tileLayers;
    public List<Biome> biomes;

    private Dictionary<int, GameObject> tileGroups;

    public void GenerateMapInEditor()
    {
        ClearMap();
        CreateTileGroups();

        float[,] elevationMap = NoiseGenerator.GenerateNoiseMap(
            mapWidth, mapHeight, magnification,
            perlinOctaves, perlinPersistence, perlinLacunarity,
            seed, noiseType, valueFrequency);

        if (useBiomes)
        {
            float[,] temperatureMap = NoiseGenerator.GenerateNoiseMap(
                mapWidth, mapHeight, magnification,
                perlinOctaves, perlinPersistence, perlinLacunarity,
                seed + 1000, noiseType, valueFrequency);

            float[,] moistureMap = NoiseGenerator.GenerateNoiseMap(
                mapWidth, mapHeight, magnification,
                perlinOctaves, perlinPersistence, perlinLacunarity,
                seed + 2000, noiseType, valueFrequency);

            GenerateBiomeMap(elevationMap, temperatureMap, moistureMap);
        }
        else
        {
            GenerateStandardMap(elevationMap);
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

    void GenerateBiomeMap(float[,] elevationMap, float[,] temperatureMap, float[,] moistureMap)
    {
        int width = elevationMap.GetLength(0);
        int height = elevationMap.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float temp = temperatureMap[x, y];
                float moist = moistureMap[x, y];

                Biome biome = GetBiomeFromClimate(temp, moist);
                if (biome != null && biome.prefab != null)
                {
                    GameObject tile = Instantiate(biome.prefab, this.transform);
                    tile.name = $"Biome_{biome.name}_x[{x}]_y[{y}]";
                    tile.transform.localPosition = new Vector3(x, y, 0);
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

    Biome GetBiomeFromClimate(float temperature, float moisture)
    {
        foreach (var biome in biomes)
        {
            if (temperature >= biome.minTemperature && temperature <= biome.maxTemperature &&
                moisture >= biome.minMoisture && moisture <= biome.maxMoisture)
            {
                return biome;
            }
            
           
            Debug.LogWarning($"Biome {biome.name} no match for temp {temperature:F2}, moisture {moisture:F2}");
            
        }
        return null;
    }



    public void ClearMap()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
