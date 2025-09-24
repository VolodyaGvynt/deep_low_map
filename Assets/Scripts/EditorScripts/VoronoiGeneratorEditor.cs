using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VoronoiGenerator))]
public class VoronoiGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        VoronoiGenerator generator = (VoronoiGenerator)target;

        EditorGUILayout.LabelField("Voronoi Settings", EditorStyles.boldLabel);
        generator.mapWidth = EditorGUILayout.IntField("Map Width", generator.mapWidth);
        generator.mapHeight = EditorGUILayout.IntField("Map Height", generator.mapHeight);
        generator.siteCount = EditorGUILayout.IntField("Site Count", generator.siteCount);
        generator.seed = EditorGUILayout.IntField("Seed", generator.seed);
        generator.autoUpdate = EditorGUILayout.Toggle("Auto Update", generator.autoUpdate);

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Voronoi"))
        {
            generator.GenerateVoronoiTexture();
        }

        if (GUILayout.Button("Save Voronoi PNG"))
        {
            generator.SaveVoronoiTexture();
        }

        if (GUI.changed && generator.autoUpdate)
        {
            generator.GenerateVoronoiTexture();
        }

        // Ensure the object is marked dirty for scene saving
        if (GUI.changed)
        {
            EditorUtility.SetDirty(generator);
        }
    }
}
