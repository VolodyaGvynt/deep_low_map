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
            var road = Instantiate(straight, new Vector3(pos.x, pos.y, -0.5f), rotation, transform);
            roadDictionary.Add(pos, road);

            fixCandidates.Add(pos);
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
            List<Direction> neighbours = PlacementHelper.FindNeighbour(pos, roadDictionary.Keys);
            Quaternion rotation = Quaternion.identity;

            if (roadDictionary[pos] != null)
                DestroyImmediate(roadDictionary[pos]);

            bool up = neighbours.Contains(Direction.Up);
            bool down = neighbours.Contains(Direction.Down);
            bool left = neighbours.Contains(Direction.Left);
            bool right = neighbours.Contains(Direction.Right);

            int count = neighbours.Count;

            if (count == 1)
            {
                if (up) rotation = Quaternion.Euler(0, 0, 180);
                else if (down) rotation = Quaternion.Euler(0, 0, 0);
                else if (left) rotation = Quaternion.Euler(0, 0, -90);
                else if (right) rotation = Quaternion.Euler(0, 0, 90);

                roadDictionary[pos] = Instantiate(end, new Vector3(pos.x, pos.y, -0.5f), rotation, transform);
            }
            else if (count == 2)
            {
                if ((up && down) || (left && right))
                {
                    rotation = (up && down) ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, -90);
                    roadDictionary[pos] = Instantiate(straight, new Vector3(pos.x, pos.y, -0.5f), rotation, transform);
                }
                else
                {
                    if (up && right) rotation = Quaternion.Euler(0, 0, 0);
                    else if (right && down) rotation = Quaternion.Euler(0, 0, -90);
                    else if (down && left) rotation = Quaternion.Euler(0, 0, 180);
                    else if (left && up) rotation = Quaternion.Euler(0, 0, 90);

                    roadDictionary[pos] = Instantiate(corner, new Vector3(pos.x, pos.y, - 0.5f), rotation, transform);
                }
            }
            else if (count == 3)
            {
                if (!up) rotation = Quaternion.Euler(0, 0, 0);
                else if (!right) rotation = Quaternion.Euler(0, 0, -90);
                else if (!down) rotation = Quaternion.Euler(0, 0, 180);
                else if (!left) rotation = Quaternion.Euler(0, 0, 90);

                roadDictionary[pos] = Instantiate(TSection, new Vector3(pos.x, pos.y, -0.5f), rotation, transform);
            }
            else if (count == 4)
            {
                roadDictionary[pos] = Instantiate(Cross, new Vector3(pos.x, pos.y, -0.5f), rotation, transform);
            }
        }
    }

    public int GetRoadSegmentCount()
    {
        return roadDictionary.Count;
    }

    public Dictionary<string, int> GetRoadTypeCounts()
    {
        Dictionary<string, int> counts = new Dictionary<string, int>()
    {
        {"Straight", 0},
        {"Corner", 0},
        {"TSection", 0},
        {"Cross", 0},
        {"End", 0}
    };

        foreach (var kvp in roadDictionary)
        {
            GameObject roadObj = kvp.Value;
            if (roadObj == null) continue;

            if (roadObj.name.StartsWith(straight.name)) counts["Straight"]++;
            else if (roadObj.name.StartsWith(corner.name)) counts["Corner"]++;
            else if (roadObj.name.StartsWith(TSection.name)) counts["TSection"]++;
            else if (roadObj.name.StartsWith(Cross.name)) counts["Cross"]++;
            else if (roadObj.name.StartsWith(end.name)) counts["End"]++;
        }

        return counts;
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
