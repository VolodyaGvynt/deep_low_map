using UnityEngine;

[ExecuteAlways]
public class NoiseMapGenerator : MonoBehaviour
{
    public NoiseType noiseType = NoiseType.Perlin;

    public int mapWidth = 100;
    public int mapHeight = 100;
    public float magnification = 10f;
    public int seed = 0;

    public int octaves = 4;
    public float persistence = 0.5f;
    public float lacunarity = 2f;

    
    public bool autoUpdate = true;

    private Texture2D noiseTexture;

    public void GenerateNoiseTexture()
    {
        float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(
            mapWidth, mapHeight, magnification,
            octaves, persistence, lacunarity,
            seed, noiseType);

        noiseTexture = new Texture2D(mapWidth, mapHeight);
        noiseTexture.filterMode = FilterMode.Point;

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float v = noiseMap[x, y];
                Color c = new Color(v, v, v);
                noiseTexture.SetPixel(x, y, c);
            }
        }

        noiseTexture.Apply();
    }

    public void SaveNoiseTexture()
    {
        if (noiseTexture == null)
        {
            Debug.LogWarning("Noise texture is null. Generate it before saving.");
            return;
        }

        byte[] pngData = noiseTexture.EncodeToPNG();
        if (pngData == null)
        {
            Debug.LogError("Failed to encode texture to PNG.");
            return;
        }

        string folderPath = System.IO.Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures),
            "NoiseMaps"
        );

        if (!System.IO.Directory.Exists(folderPath))
            System.IO.Directory.CreateDirectory(folderPath);

        string filePath = System.IO.Path.Combine(folderPath, $"Noise_{System.DateTime.Now:yyyyMMdd_HHmmss}.png");

        System.IO.File.WriteAllBytes(filePath, pngData);

        Debug.Log($"Noise texture saved to: {filePath}");
    }


    private void OnDrawGizmos()
    {
        if (noiseTexture == null)
            GenerateNoiseTexture();

        Vector3 position = transform.position;
        Rect rect = new Rect(position.x, position.y, mapWidth, mapHeight);
        Gizmos.DrawGUITexture(rect, noiseTexture);
    }
}
