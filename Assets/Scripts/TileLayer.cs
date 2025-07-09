using UnityEngine;


[System.Serializable]
public class TileLayer
{
    public GameObject prefab;
    [Range(0f, 1f)]
    public float threshold; 
}
