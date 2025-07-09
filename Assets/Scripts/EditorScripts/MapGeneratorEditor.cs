using UnityEngine;
using UnityEditor;

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
        EditorGUILayout.LabelField("Tile Prefabs", EditorStyles.boldLabel);
        mapGen.prefabWater = (GameObject)EditorGUILayout.ObjectField("Water Prefab", mapGen.prefabWater, typeof(GameObject), false);
        mapGen.prefabSand = (GameObject)EditorGUILayout.ObjectField("Sand Prefab", mapGen.prefabSand, typeof(GameObject), false);
        mapGen.prefabGrass = (GameObject)EditorGUILayout.ObjectField("Grass Prefab", mapGen.prefabGrass, typeof(GameObject), false);
        mapGen.prefabStone = (GameObject)EditorGUILayout.ObjectField("Stone Prefab", mapGen.prefabStone, typeof(GameObject), false);
        mapGen.prefabSnow = (GameObject)EditorGUILayout.ObjectField("Snow Prefab", mapGen.prefabSnow, typeof(GameObject), false);

        EditorGUILayout.Space();
        if (GUILayout.Button("Generate Map"))
        {
            mapGen.GenerateMapInEditor();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(mapGen);
        }
    }
}
