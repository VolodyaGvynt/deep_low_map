using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LTownGenerator))]
public class SimpleVisualizerEditor : Editor {
    public override void OnInspectorGUI() {
        LTownGenerator visualizer = (LTownGenerator)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate & Visualize L-System")) {
            if (visualizer.lsystem != null) {
                string sequence = visualizer.lsystem.GenerateSequence();
                visualizer.GenerateLTown(sequence);
                Debug.Log("Visualized Sequence: " + sequence);
            } else {
                Debug.LogWarning("LSystemGenerator referenc is missing.");
            }
        }
    }
}

