using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class VoronoiGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int mapWidth = 100;
    public int mapHeight = 100;
    public int siteCount = 10;
    public int seed = 0;
    public bool autoUpdate = true;

    [Header("Export Settings")]
    public int exportScale = 20;

    private Texture2D voronoiTexture;
    private List<Vector2Int> sites;
    private List<Color> siteColors;


    public void GenerateVoronoiTexture()
    {
        voronoiTexture = new Texture2D(mapWidth, mapHeight);
        voronoiTexture.filterMode = FilterMode.Point;

        sites = new List<Vector2Int>();
        siteColors = new List<Color>();

        System.Random rand = new System.Random(seed);

        sites = new List<Vector2Int>()
        {
            new Vector2Int(1,1),
            new Vector2Int(3,1),
            new Vector2Int(2,4)
        };

        siteColors = new List<Color>()
        {
            Color.red,  
            Color.green,
            Color.blue 
        };

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector2Int point = new Vector2Int(x, y);
                int closestIndex = GetClosestSiteIndex(point);
                voronoiTexture.SetPixel(x, y, siteColors[closestIndex]);
            }
        }

        foreach (var site in sites)
        {
            if (site.x >= 0 && site.x < mapWidth && site.y >= 0 && site.y < mapHeight)
            {
                voronoiTexture.SetPixel(site.x, site.y, Color.black);
            }
        }

        voronoiTexture.Apply();
    }


    private int GetClosestSiteIndex(Vector2Int point)
    {
        float minDist = float.MaxValue;
        int closestIndex = 0;

        for (int i = 0; i < sites.Count; i++)
        {
            float dist = Vector2Int.Distance(sites[i], point);
            if (dist < minDist)
            {
                minDist = dist;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    public void SaveVoronoiTexture()
    {
        if (voronoiTexture == null)
        {
            Debug.LogWarning("Voronoi texture is null. Generate it before saving.");
            return;
        }

        int exportWidth = mapWidth * exportScale;
        int exportHeight = mapHeight * exportScale;
        Texture2D scaledTexture = new Texture2D(exportWidth, exportHeight);
        scaledTexture.filterMode = FilterMode.Point;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Color c = voronoiTexture.GetPixel(x, y);

                for (int dx = 0; dx < exportScale; dx++)
                {
                    for (int dy = 0; dy < exportScale; dy++)
                    {
                        scaledTexture.SetPixel(x * exportScale + dx, y * exportScale + dy, c);
                    }
                }
            }
        }

        scaledTexture.Apply();

        byte[] pngData = scaledTexture.EncodeToPNG();
        if (pngData == null)
        {
            Debug.LogError("Failed to encode texture to PNG.");
            return;
        }

        string folderPath = System.IO.Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures),
            "VoronoiMaps");

        if (!System.IO.Directory.Exists(folderPath))
            System.IO.Directory.CreateDirectory(folderPath);

        string filePath = System.IO.Path.Combine(folderPath, $"Voronoi_{System.DateTime.Now:yyyyMMdd_HHmmss}.png");

        System.IO.File.WriteAllBytes(filePath, pngData);

        Debug.Log($"Voronoi texture saved to: {filePath}");
    }


    private void OnDrawGizmos()
    {
        if (voronoiTexture == null && autoUpdate)
            GenerateVoronoiTexture();

        if (voronoiTexture != null)
        {
            Vector3 pos = transform.position;
            Rect rect = new Rect(pos.x, pos.y, mapWidth, mapHeight);
            Gizmos.DrawGUITexture(rect, voronoiTexture);
        }
    }
}
