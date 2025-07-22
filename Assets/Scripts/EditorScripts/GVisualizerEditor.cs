using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridTownGenerator))]
public class GVisualizerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GridTownGenerator visualizer = (GridTownGenerator)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate Brick Wall Layout"))
        {
            visualizer.GenerateGridTown();
            Debug.Log("Brick Wall layout generated.");
        }
    }
}
