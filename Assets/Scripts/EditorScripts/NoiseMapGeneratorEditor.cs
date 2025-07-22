using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NoiseMapGenerator))]
public class NoiseMapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NoiseMapGenerator gen = (NoiseMapGenerator)target;

        EditorGUILayout.LabelField("Noise Map Settings", EditorStyles.boldLabel);
        gen.noiseType = (NoiseType)EditorGUILayout.EnumPopup("Noise Type", gen.noiseType);
        gen.mapWidth = EditorGUILayout.IntField("Map Width", gen.mapWidth);
        gen.mapHeight = EditorGUILayout.IntField("Map Height", gen.mapHeight);
        gen.magnification = EditorGUILayout.FloatField("Magnification", gen.magnification);
        gen.seed = EditorGUILayout.IntField("Seed", gen.seed);

        
            EditorGUILayout.Space();
            gen.octaves = EditorGUILayout.IntField("Octaves", gen.octaves);
            gen.persistence = EditorGUILayout.FloatField("Persistence", gen.persistence);
            gen.lacunarity = EditorGUILayout.FloatField("Lacunarity", gen.lacunarity);
        

        

        gen.autoUpdate = EditorGUILayout.Toggle("Auto Update", gen.autoUpdate);

        if (GUILayout.Button("Generate Noise Map"))
        {
            gen.GenerateNoiseTexture();
        }

        if (GUILayout.Button("Save Image"))
        {
            gen.SaveNoiseTexture();
        }


        if (GUI.changed && gen.autoUpdate)
        {
            gen.GenerateNoiseTexture();
            EditorUtility.SetDirty(gen);
        }
    }
}
