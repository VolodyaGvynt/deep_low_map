using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LVisualizer))]
public class SimpleVisualizerEditor : Editor {
    public override void OnInspectorGUI() {
        LVisualizer visualizer = (LVisualizer)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate & Visualize L-System")) {
            if (visualizer.lsystem != null) {
                string sequence = visualizer.lsystem.GenerateSequence();
                visualizer.Visualize(sequence);
                Debug.Log("Visualized Sequence: " + sequence);
            } else {
                Debug.LogWarning("LSystemGenerator referenc is missing.");
            }
        }
    }
}

