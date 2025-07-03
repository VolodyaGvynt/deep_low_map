using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileMapGenerator))]
public class TileMapGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        TileMapGenerator mapGen = (TileMapGenerator)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate Map")) {
            mapGen.GenerateMapInEditor();
        }
        
        
    }
}