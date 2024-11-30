using UnityEngine;

public class VoxelGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int chunkWidth = 16;  // Number of blocks along the X axis
    public int chunkLength = 16; // Number of blocks along the Z axis
    public int chunkHeight = 8;  // Maximum height of the terrain

    [Header("Block Settings")]
    public GameObject[] blockPrefabs; // Array of block prefabs for different layers

    void Start()
    {
        GenerateLayeredTerrain();
    }

    void GenerateLayeredTerrain()
    {
        for (int x = 0; x < chunkWidth; x++)
        {
            for (int z = 0; z < chunkLength; z++)
            {
                for (int y = 0; y < chunkHeight; y++)
                {
                    GameObject blockToInstantiate = GetBlockForLayer(chunkHeight - 1 - y);
                    if (blockToInstantiate != null)
                    {
                        Vector3 position = new Vector3(x, y, z);
                        Instantiate(blockToInstantiate, position, Quaternion.identity, transform);
                    }
                }
            }
        }
    }

    GameObject GetBlockForLayer(int layer)
    {
        // Determine the block type for each layer
        if (layer == 0)
        {
            return blockPrefabs.Length > 0 ? blockPrefabs[0] : null; // Topmost layer
        }
        else if (layer <= 3)
        {
            return blockPrefabs.Length > 1 ? blockPrefabs[1] : null; // Next three layers
        }
        else
        {
            return blockPrefabs.Length > 2 ? blockPrefabs[2] : null; // Remaining layers
        }
    }
}
