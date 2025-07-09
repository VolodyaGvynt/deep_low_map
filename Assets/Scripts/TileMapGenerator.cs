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

    public GameObject prefabWater;
    public GameObject prefabSand;
    public GameObject prefabGrass;
    public GameObject prefabStone;
    public GameObject prefabSnow;

    Dictionary<int, GameObject> tileset;
    Dictionary<int, GameObject> tileGroups;

    public void GenerateMapInEditor()
    {
        CreateTileset();
        CreateTileGroups();

        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(
            mapWidth, mapHeight, magnification,
            perlinOctaves, perlinPersistence, perlinLacunarity,
            seed, noiseType, valueFrequency);

        GenerateMap(noiseMap);
    }

    void CreateTileset()
    {
        tileset = new Dictionary<int, GameObject> {
            {0, prefabWater},
            {1, prefabSand},
            {2, prefabGrass},
            {3, prefabStone},
            {4, prefabSnow}
        };
    }

    void CreateTileGroups()
    {
        tileGroups = new Dictionary<int, GameObject>();
        foreach (var pair in tileset)
        {
            GameObject group = new GameObject(pair.Value.name);
            group.transform.parent = this.transform;
            tileGroups[pair.Key] = group;
        }
    }

    void GenerateMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int tileID = GetTileIdFromNoise(noiseMap[x, y]);
                CreateTile(tileID, x, y);
            }
        }
    }

    int GetTileIdFromNoise(float noiseValue)
    {
        float scaled = noiseValue * tileset.Count;
        if (scaled == tileset.Count) scaled = tileset.Count - 1;
        return Mathf.FloorToInt(scaled);
    }

    void CreateTile(int id, int x, int y)
    {
        GameObject tile = Instantiate(tileset[id], tileGroups[id].transform);
        tile.name = $"Tile_x[{x}]_y[{y}]";
        tile.transform.localPosition = new Vector3(x, y, 0);
    }
}
