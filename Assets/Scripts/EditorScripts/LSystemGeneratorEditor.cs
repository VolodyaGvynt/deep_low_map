using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LSystemGenerator))]
public class LSystemGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        LSystemGenerator lGen = (LSystemGenerator)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate Sentence")) {
            string result = lGen.GenerateSequence();
            Debug.Log("Generated Sentence: " + result);
        }
        

    }
}