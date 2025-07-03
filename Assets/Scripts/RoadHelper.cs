using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RoadHelper : MonoBehaviour {
    public GameObject roadStraight;
    public GameObject roadCorner;
    public GameObject roadTSection;
    public GameObject roadXSection;
    public GameObject roadEnd;

    public float roadZOffset = -0.1f;
    Dictionary<Vector3Int, GameObject> roadDictionary = new Dictionary<Vector3Int, GameObject>();
    private HashSet<Vector3Int> fixRoadCanditates = new HashSet<Vector3Int>();
    private Vector3 roadStartPos = Vector3.zero;

    public void PlaceRoads(Vector3 startPos, Vector3Int direction, int length) {
        Vector3 adjustedStartPos = startPos + roadStartPos;
        Quaternion rotation = GetRotation(direction);

        for (int i = 0; i < length; i++) {
            Vector3 roadWorldPos = adjustedStartPos + (Vector3)direction * i;
            Vector3Int roadPos = Vector3Int.RoundToInt(roadWorldPos);
            
            if (roadDictionary.ContainsKey(roadPos))
            {
                continue;
            }
            
            Vector3 finalWorldPos = new Vector3(roadPos.x, roadPos.y, roadZOffset);
            
            GameObject road = Instantiate(roadStraight, finalWorldPos, rotation, transform);
            road.name = $"Road_x[{roadPos.x}]_y[{roadPos.y}]";
            
            roadDictionary.Add(roadPos, road);
            
            if (i == 0 || i == length - 1) 
            {
                fixRoadCanditates.Add(roadPos);
            }
        }
    }

    private Quaternion GetRotation(Vector3Int direction) {
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

    public void SetPosition(float x, float y) {
        roadStartPos = new Vector3(x, y, 0);
    }
    
    public void FixRoad() {
        //todo
    }

    public void ClearRoads()
    {
#if UNITY_EDITOR
        foreach (var road in roadDictionary.Values)
        {
            if (road != null)
            {
                DestroyImmediate(road);
            }
        }
        
        roadDictionary.Clear();
        fixRoadCanditates.Clear();
#endif
    }
}
