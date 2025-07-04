using UnityEngine;
using System;

[Serializable]
public class House
{
    [SerializeField]
    private GameObject[] prefabs;
    public int quantity;
    public int placedQuantity;

    public GameObject GetPrefab()
    {
        placedQuantity++;
        if (prefabs.Length > 1) { 
            var randomIndex = UnityEngine.Random.Range(0, prefabs.Length);
            return prefabs[randomIndex];
        }
        return prefabs[0];
    }

    public bool CanPlace()
    {
        return placedQuantity < quantity;
    }


    public void Reset()
    {
        placedQuantity = 0;
    }
}
