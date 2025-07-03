using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(menuName = "Town Rule")]
public class Rule : ScriptableObject {
    public string letter;
    [SerializeField]
    private string[] results = null;

    [SerializeField] 
    private bool random = false;

    public string GetResult() {
        if (random) {
            int rand = UnityEngine.Random.Range(0, results.Length);
            return results[rand];
        }
        return results[0];
    }
}
