using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(TileMapGenerator))]
public class TileMapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TileMapGenerator mapGen = (TileMapGenerator)target;

        EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
        mapGen.noiseType = (NoiseType)EditorGUILayout.EnumPopup("Noise Type", mapGen.noiseType);
        mapGen.mapWidth = EditorGUILayout.IntField("Map Width", mapGen.mapWidth);
        mapGen.mapHeight = EditorGUILayout.IntField("Map Height", mapGen.mapHeight);
        mapGen.magnification = EditorGUILayout.FloatField("Magnification", mapGen.magnification);
        mapGen.seed = EditorGUILayout.IntField("Seed", mapGen.seed);

        if (mapGen.noiseType == NoiseType.Perlin)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Perlin Noise Settings", EditorStyles.boldLabel);
            mapGen.perlinOctaves = EditorGUILayout.IntField("Perlin Octaves", mapGen.perlinOctaves);
            mapGen.perlinPersistence = EditorGUILayout.FloatField("Perlin Persistence", mapGen.perlinPersistence);
            mapGen.perlinLacunarity = EditorGUILayout.FloatField("Perlin Lacunarity", mapGen.perlinLacunarity);
        }

        if (mapGen.noiseType == NoiseType.Value)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Value Noise Settings", EditorStyles.boldLabel);
            mapGen.valueFrequency = EditorGUILayout.FloatField("Value Frequency", mapGen.valueFrequency);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Tile Layers (Threshold-Based)", EditorStyles.boldLabel);

        int newSize = EditorGUILayout.IntField("Layer Count", mapGen.tileLayers.Count);
        while (newSize > mapGen.tileLayers.Count)
            mapGen.tileLayers.Add(new TileLayer());
        while (newSize < mapGen.tileLayers.Count)
            mapGen.tileLayers.RemoveAt(mapGen.tileLayers.Count - 1);

        for (int i = 0; i < mapGen.tileLayers.Count; i++)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            mapGen.tileLayers[i].prefab = (GameObject)EditorGUILayout.ObjectField($"Tile {i} Prefab", mapGen.tileLayers[i].prefab, typeof(GameObject), false);
            mapGen.tileLayers[i].threshold = EditorGUILayout.Slider($"Threshold ?", mapGen.tileLayers[i].threshold, 0f, 1f);
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Biome Settings", EditorStyles.boldLabel);

        mapGen.useBiomes = EditorGUILayout.Toggle("Use Biomes", mapGen.useBiomes);

        if (mapGen.useBiomes)
        {
            int newBiomeSize = EditorGUILayout.IntField("Biome Count", mapGen.biomes.Count);
            while (newBiomeSize > mapGen.biomes.Count)
                mapGen.biomes.Add(new Biome());
            while (newBiomeSize < mapGen.biomes.Count)
                mapGen.biomes.RemoveAt(mapGen.biomes.Count - 1);

            for (int i = 0; i < mapGen.biomes.Count; i++)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                mapGen.biomes[i].name = EditorGUILayout.TextField($"Biome {i} Name", mapGen.biomes[i].name);
                mapGen.biomes[i].prefab = (GameObject)EditorGUILayout.ObjectField("Tile Prefab", mapGen.biomes[i].prefab, typeof(GameObject), false);
                mapGen.biomes[i].minTemperature = EditorGUILayout.Slider("Min Temperature", mapGen.biomes[i].minTemperature, 0f, 1f);
                mapGen.biomes[i].maxTemperature = EditorGUILayout.Slider("Max Temperature", mapGen.biomes[i].maxTemperature, 0f, 1f);
                mapGen.biomes[i].minMoisture = EditorGUILayout.Slider("Min Moisture", mapGen.biomes[i].minMoisture, 0f, 1f);
                mapGen.biomes[i].maxMoisture = EditorGUILayout.Slider("Max Moisture", mapGen.biomes[i].maxMoisture, 0f, 1f);
                EditorGUILayout.EndVertical();
            }
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Generate Map"))
        {
            mapGen.GenerateMapInEditor();
        }

        if (GUILayout.Button("Clear Map"))
        {
            mapGen.ClearMap();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(mapGen);
        }
    }
}
