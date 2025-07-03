using UnityEngine;
using System.Collections.Generic;
public class RoadHelper : MonoBehaviour
{
    public GameObject straight, corner, TSection, Cross, end;
    Dictionary<Vector3Int, GameObject> roadDictionary = new Dictionary<Vector3Int, GameObject>();
    HashSet<Vector3Int> fixCandidates = new HashSet<Vector3Int>();

    public void PlaceRoad(Vector3 position,Vector3Int direction, int length)
    {
        Quaternion rotation = GetRotation(direction);

        for (int i = 0; i < length; i++)
        {
            var pos = Vector3Int.RoundToInt(position + direction * i);
            if (roadDictionary.ContainsKey(pos)) continue;
            var road = Instantiate(straight, pos, rotation, transform);
            roadDictionary.Add(pos, road);
            if (i == 0 || i == length) { 
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
}
