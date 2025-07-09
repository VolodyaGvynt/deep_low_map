using UnityEngine;

[System.Serializable]
public class Biome
{
    public string name;
    public GameObject prefab;

    [Range(0f, 1f)]
    public float minTemperature;
    [Range(0f, 1f)]
    public float maxTemperature;

    [Range(0f, 1f)]
    public float minMoisture;
    [Range(0f, 1f)]
    public float maxMoisture;
}
