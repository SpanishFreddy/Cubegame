using UnityEngine;

public class VoxelGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int chunkWidth = 16;  // Number of blocks along the X axis
    public int chunkLength = 16; // Number of blocks along the Z axis
    public int chunkHeight = 8;  // Maximum height of the terrain
    public float noiseScale = 10f; // Scale for Perlin noise

    [Header("Block Settings")]
    public GameObject blockPrefab; // Assign the block prefab in the Inspector

    void Start()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        for (int x = 0; x < chunkWidth; x++)
        {
            for (int z = 0; z < chunkLength; z++)
            {
                // Use Perlin noise to generate a height map
                float yHeight = Mathf.PerlinNoise(x / noiseScale, z / noiseScale) * chunkHeight;

                // Round to nearest integer for discrete block levels
                int height = Mathf.RoundToInt(yHeight);

                for (int y = 0; y < height; y++)
                {
                    Vector3 position = new Vector3(x, y, z);
                    Instantiate(blockPrefab, position, Quaternion.identity, transform);
                }
            }
        }
    }
}
