using UnityEngine;
using System.Collections.Generic;


public class GVisualizer : MonoBehaviour
{
    public int width = 20;
    public int height = 10;
    public int hSpacing = 4;
    public float vRoadChance = 0.5f;
    public RoadHelper roadHelper;
    public StructureHelper structureHelper;





    public void VisualizeBrickWall()
    {
        roadHelper.Clear();
        structureHelper.Clear();

        List<Vector3Int> roadPositions = new List<Vector3Int>();

        for (int y = 0; y <= height; y += hSpacing)
        {
            for (int x = 0; x <= width; x++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                roadHelper.PlaceRoad(pos, Vector3Int.right, 1);
                roadPositions.Add(pos);
            }
        }

        for (int y = 0; y <= height - hSpacing; y += hSpacing)
        {
            int x = 0;
            while (x <= width)
            {
                if (UnityEngine.Random.value <= vRoadChance)
                {
                    for (int yOffset = 1; yOffset < hSpacing; yOffset++)
                    {
                        Vector3Int pos = new Vector3Int(x, y + yOffset, 0);
                        roadHelper.PlaceRoad(pos, Vector3Int.up, 1);
                        roadPositions.Add(pos);
                    }
                    x += 3;
                }
                else
                {
                    x++;
                }
            }
        }


        roadHelper.FixRoads();
        structureHelper.PlaceStructure(roadHelper.GetPositions());
    }

}
