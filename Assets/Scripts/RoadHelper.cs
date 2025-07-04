using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class RoadHelper : MonoBehaviour
{
    public GameObject straight, corner, TSection, Cross, end;
    

    Dictionary<Vector3Int, GameObject> roadDictionary = new Dictionary<Vector3Int, GameObject>();
    HashSet<Vector3Int> fixCandidates = new HashSet<Vector3Int>();

    public List<Vector3Int> GetPositions() { 
        return roadDictionary.Keys.ToList();
    }

    public void PlaceRoad(Vector3 position, Vector3Int direction, int length)
    {
        Quaternion rotation = GetRotation(direction);

        for (int i = 0; i < length; i++)
        {
            var pos = Vector3Int.RoundToInt(position + direction * i);
            if (roadDictionary.ContainsKey(pos)) continue;
            var road = Instantiate(straight, new Vector3(pos.x, pos.y, 0), rotation, transform);
            roadDictionary.Add(pos, road);

            if (i == 0 || i == length - 1)
            {
                fixCandidates.Add(pos);
            }
        }
    }

    private Quaternion GetRotation(Vector3Int direction)
    {
        if (direction == Vector3Int.up)
            return Quaternion.Euler(0, 0, 0);
        else if (direction == Vector3Int.right)
            return Quaternion.Euler(0, 0, -90);
        else if (direction == Vector3Int.down)
            return Quaternion.Euler(0, 0, 180);
        else if (direction == Vector3Int.left)
            return Quaternion.Euler(0, 0, 90);
        else
            return Quaternion.identity;
    }

    public void FixRoads()
    {
        foreach (var pos in fixCandidates)
        {
            List<Direction> neighbourDirections = PlacementHelper.FindNeighbour(pos, roadDictionary.Keys);
            Quaternion rotation = Quaternion.identity;

            if (neighbourDirections.Count == 1)
            {
                DestroyImmediate(roadDictionary[pos]);

                if (neighbourDirections.Contains(Direction.Up))
                    rotation = Quaternion.Euler(0, 0, 180);
                else if (neighbourDirections.Contains(Direction.Left))
                    rotation = Quaternion.Euler(0, 0, -90);
                else if (neighbourDirections.Contains(Direction.Right))
                    rotation = Quaternion.Euler(0, 0, 90);

                roadDictionary[pos] = Instantiate(end, new Vector3(pos.x, pos.y, 0), rotation, transform);
            }
            else if (neighbourDirections.Count == 2)
            {
                if ((neighbourDirections.Contains(Direction.Up) && neighbourDirections.Contains(Direction.Down)) ||
                    (neighbourDirections.Contains(Direction.Left) && neighbourDirections.Contains(Direction.Right)))
                {
                    continue;
                }

                DestroyImmediate(roadDictionary[pos]);

                if (neighbourDirections.Contains(Direction.Up) && neighbourDirections.Contains(Direction.Left))
                    rotation = Quaternion.Euler(0, 0, 90);
                else if (neighbourDirections.Contains(Direction.Down) && neighbourDirections.Contains(Direction.Left))
                    rotation = Quaternion.Euler(0, 0, 180);
                else if (neighbourDirections.Contains(Direction.Down) && neighbourDirections.Contains(Direction.Right))
                    rotation = Quaternion.Euler(0, 0, -90);

                roadDictionary[pos] = Instantiate(corner, new Vector3(pos.x, pos.y, 0), rotation, transform);
            }
            else if (neighbourDirections.Count == 3)
            {
                DestroyImmediate(roadDictionary[pos]);

                
                if (neighbourDirections.Contains(Direction.Up) &&
                    neighbourDirections.Contains(Direction.Right) &&
                    neighbourDirections.Contains(Direction.Down))
                    rotation = Quaternion.Euler(0, 0, 90);
                else if (neighbourDirections.Contains(Direction.Up) &&
                         neighbourDirections.Contains(Direction.Left) &&
                         neighbourDirections.Contains(Direction.Down))
                    rotation = Quaternion.Euler(0, 0, -90);
                else if (neighbourDirections.Contains(Direction.Up) &&
                         neighbourDirections.Contains(Direction.Left) &&
                         neighbourDirections.Contains(Direction.Right))
                    rotation = Quaternion.Euler(0, 0, 180);

                roadDictionary[pos] = Instantiate(TSection, new Vector3(pos.x, pos.y, 0), rotation, transform);
            }
            else if (neighbourDirections.Count == 4)
            {
                DestroyImmediate(roadDictionary[pos]);
                roadDictionary[pos] = Instantiate(Cross, new Vector3(pos.x, pos.y, 0), rotation, transform);
            }
        }
    }

    public void Clear()
    {
        foreach (var road in roadDictionary.Values)
        {
            if (road != null)
                DestroyImmediate(road);
        }

        roadDictionary.Clear();
        fixCandidates.Clear();
    }

}
