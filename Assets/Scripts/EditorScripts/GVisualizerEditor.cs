using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GVisualizer))]
public class GVisualizerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GVisualizer visualizer = (GVisualizer)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate Brick Wall Layout"))
        {
            visualizer.VisualizeBrickWall();
            Debug.Log("Brick Wall layout generated.");
        }
    }
}
