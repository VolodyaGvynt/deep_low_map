using UnityEngine;
using System.Collections.Generic;


public class GridTownGenerator : MonoBehaviour
{
    public int width = 20;
    public int height = 10;
    public int hSpacing = 4;
    public float vRoadChance = 0.5f;
    public RoadHelper roadHelper;
    public StructureHelper structureHelper;





    public void GenerateGridTown()
    {
        roadHelper.Clear();
        structureHelper.Clear();

        long memBefore = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();
        float timeBefore = Time.realtimeSinceStartup;

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

        float timeAfter = Time.realtimeSinceStartup;
        long memAfter = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();

        Debug.Log($"Generacja miasta L: czas = {(timeAfter - timeBefore) * 1000f} ms");
        Debug.Log($"Generacja miasta L: pamiec = {(memAfter - memBefore) / 1024f} KB");
        PrintMetrics();
    }

    void PrintMetrics()
    {
        int roadCount = roadHelper.GetRoadSegmentCount();
        var roadTypes = roadHelper.GetRoadTypeCounts();
        int buildingCount = structureHelper.GetBuildingCount();
        float occupancy = structureHelper.GetOccupancyRate(roadHelper.GetPositions());

        Debug.Log($"[GVisualizer] Liczba segmentów dróg: {roadCount}");
        foreach (var kvp in roadTypes)
        {
            Debug.Log($"[GVisualizer] {kvp.Key}: {kvp.Value}");
        }
        Debug.Log($"[GVisualizer] Liczba budynków: {buildingCount}");
        Debug.Log($"[GVisualizer] Procent zajętości: {occupancy * 100f:F2}%");
    }

}
