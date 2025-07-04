using UnityEngine;
using System.Collections.Generic;
using System;

public class StructureHelper : MonoBehaviour
{
    public House[] houses;
    public GameObject[] nature;
    [Range(0, 1)]
    public float natureChance = 0.3f;
    [Range(0, 1)]
    public float emptySpotChance = 0.2f; 

    public Dictionary<Vector3Int, GameObject> structureDictionary = new Dictionary<Vector3Int, GameObject>();
    public Dictionary<Vector3Int, GameObject> natureDictionary = new Dictionary<Vector3Int, GameObject>();



    public void PlaceStructure(List<Vector3Int> pos)
    {
        Dictionary<Vector3Int, Direction> freeSpots = FindFreeSpots(pos);
        List<Vector3Int> blockedPos = new List<Vector3Int>(pos);

        foreach (var freeSpot in freeSpots)
        {
            if (blockedPos.Contains(freeSpot.Key)) continue;

            // Roll for empty spot first
            if (UnityEngine.Random.value < emptySpotChance)
            {
                // Skip placing anything here; leave spot empty
                continue;
            }

            var rotation = Quaternion.identity;
            switch (freeSpot.Value)
            {
                case Direction.Up: rotation = Quaternion.Euler(0, 0, 180); break;
                case Direction.Down: rotation = Quaternion.Euler(0, 0, 0); break;
                case Direction.Left: rotation = Quaternion.Euler(0, 0, -90); break;
                case Direction.Right: rotation = Quaternion.Euler(0, 0, 90); break;
            }

            bool placed = false;

            if (UnityEngine.Random.value < natureChance && nature.Length > 0)
            {
                var natureObject = Instantiate(nature[UnityEngine.Random.Range(0, nature.Length)], freeSpot.Key, rotation, transform);
                natureDictionary.Add(freeSpot.Key, natureObject);
                placed = true;
            }

            if (!placed)
            {
                for (int i = 0; i < houses.Length; i++)
                {
                    if (houses[i].quantity != -1 && houses[i].CanPlace())
                    {
                        var house = Instantiate(houses[i].GetPrefab(), freeSpot.Key, rotation, transform);
                        structureDictionary.Add(freeSpot.Key, house);
                        placed = true;
                        break;
                    }
                }
            }

            if (!placed)
            {
                for (int i = 0; i < houses.Length; i++)
                {
                    if (houses[i].quantity == -1)
                    {
                        var house = Instantiate(houses[i].GetPrefab(), freeSpot.Key, rotation, transform);
                        structureDictionary.Add(freeSpot.Key, house);
                        break;
                    }
                }
            }
        }
    }


    Dictionary<Vector3Int, Direction> FindFreeSpots(List<Vector3Int> positions)
    {
        Dictionary<Vector3Int, Direction> freeSpots = new Dictionary<Vector3Int, Direction>();
        HashSet<Vector3Int> positionSet = new HashSet<Vector3Int>(positions);

        foreach (var position in positions)
        {
            List<Direction> neighbourDirections = PlacementHelper.FindNeighbour(position, positionSet);
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                if (neighbourDirections.Contains(direction) == false)
                {
                    var newPos = position + PlacementHelper.GetOffset(direction);
                    if (freeSpots.ContainsKey(newPos))
                    {
                        continue;
                    }

                    freeSpots.Add(newPos, PlacementHelper.OppositeDirection(direction));
                }
            }
        }

        return freeSpots;

    }


    public void Clear()
    {
        foreach (var house in houses)
        {
            house.Reset();
        }
        foreach (var structure in structureDictionary)
        {
            DestroyImmediate(structure.Value);
        }
        foreach (var structure in natureDictionary) {
            DestroyImmediate(structure.Value);
        }

        structureDictionary.Clear();
        natureDictionary.Clear();
    }
}
