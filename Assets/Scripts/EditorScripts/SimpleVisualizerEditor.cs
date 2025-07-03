using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SimpleVisualizer))]
public class SimpleVisualizerEditor : Editor {
    public override void OnInspectorGUI() {
        SimpleVisualizer visualizer = (SimpleVisualizer)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate & Visualize L-System")) {
            if (visualizer.lsystem != null) {
                string sequence = visualizer.lsystem.GenerateSequence();
                visualizer.VisualiseSequence(sequence);
                Debug.Log("Visualized Sequence: " + sequence);
            } else {
                Debug.LogWarning("LSystemGenerator reference is missing.");
            }
        }
    }
}