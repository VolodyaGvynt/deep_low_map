using System.Collections.Generic;
using UnityEngine;

public class TileMapGenerator : MonoBehaviour {
    public int mapWidth = 100;
    public int mapHeight = 100;
    public float magnification = 7f;
    public int octaves = 4;
    public float persistence = 0.5f;
    public float lacunarity = 2f;
    public int seed = 0;
    

    public GameObject prefabWater;
    public GameObject prefabSand;
    public GameObject prefabGrass;
    public GameObject prefabStone;
    public GameObject prefabSnow;

    Dictionary<int, GameObject> tileset;
    Dictionary<int, GameObject> tileGroups;
    GameObject structGroup;
    
    public LSystemGenerator lSystemGenerator;
    public SimpleVisualizer structVisualizer;
    public bool generateStructures = true;

    public void GenerateMapInEditor() {
        ClearExistingMap();
        CreateTileset();
        CreateTileGroups();

        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(mapWidth, mapHeight, magnification,  octaves, persistence, lacunarity, seed);
        GenerateMap(noiseMap);

        if (generateStructures) {
            GenerateStructures();
        }
    }

    public void ClearExistingMap()
    {
#if UNITY_EDITOR
        if (structVisualizer?.roadHelper != null)
        {
            structVisualizer.roadHelper.ClearRoads();
        }

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            
            if (child.gameObject == lSystemGenerator?.gameObject || 
                child.gameObject == structVisualizer?.gameObject)
                continue;
                
            DestroyImmediate(child.gameObject);
        }

        if (tileset != null) tileset.Clear();
        if (tileGroups != null) tileGroups.Clear();
        structGroup = null;
#endif
    }


    void CreateTileset() {
        tileset = new Dictionary<int, GameObject>();
        tileset.Add(0, prefabWater);
        tileset.Add(1, prefabSand);
        tileset.Add(2, prefabGrass);
        tileset.Add(3, prefabStone);
        tileset.Add(4, prefabSnow);
    }

    void CreateTileGroups() {
        tileGroups = new Dictionary<int, GameObject>();

        foreach (var pair in tileset) {
            GameObject group = new GameObject(pair.Value.name);
            group.transform.parent = this.transform;
            tileGroups[pair.Key] = group;
        }

        structGroup = new GameObject("Roads");
        structGroup.transform.parent = this.transform;
    }

    void GenerateMap(float[,] noiseMap) {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                int tileID = GetTileIdFromNoise(noiseMap[x, y]);
                CreateTile(tileID, x, y);
            }
        }
    }
    
    private void GenerateStructures() {
        structVisualizer.roadHelper.SetPosition(mapWidth / 2f, mapHeight / 2f);
        string roadSequence = lSystemGenerator.GenerateSequence();
        structVisualizer.VisualiseSequence(roadSequence);
    }

    int GetTileIdFromNoise(float noiseValue) {
        float scaled = noiseValue * tileset.Count;
        if (scaled == tileset.Count) scaled = tileset.Count - 1;
        return Mathf.FloorToInt(scaled);
    }

    void CreateTile(int id, int x, int y) {
        GameObject tile = Instantiate(tileset[id], tileGroups[id].transform);
        tile.name = $"Tile_x[{x}]_y[{y}]";
        tile.transform.localPosition = new Vector3(x, y, 0);
    }
    
    }
