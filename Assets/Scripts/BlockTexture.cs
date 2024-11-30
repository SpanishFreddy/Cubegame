using UnityEngine;

[ExecuteInEditMode]
public class BlockTexture : MonoBehaviour
{
    MeshFilter meshFilter;
    Mesh mesh;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.sharedMesh;

        Vector2[] uv = mesh.uv;

        // Define the sections of the texture atlas
        float sideStartX = 0f;         // First part of the texture (sides)
        float topStartX = 1f / 3f;     // Second part (top)
        float bottomStartX = 2f / 3f;  // Third part (bottom)
        float sectionWidth = 1f / 3f;

        // Sides (front, back, left, right)
        uv[0] = new Vector2(sideStartX, 0f);
        uv[1] = new Vector2(sideStartX + sectionWidth, 0f);
        uv[2] = new Vector2(sideStartX, 1f);
        uv[3] = new Vector2(sideStartX + sectionWidth, 1f);
        uv[6] = uv[0];
        uv[7] = uv[1];
        uv[10] = uv[2];
        uv[11] = uv[3];
        uv[16] = uv[0];
        uv[17] = uv[2];
        uv[18] = uv[3];
        uv[19] = uv[1];
        uv[20] = uv[0];
        uv[21] = uv[2];
        uv[22] = uv[3];
        uv[23] = uv[1];

        // Top
        uv[4] = new Vector2(topStartX, 1f);
        uv[5] = new Vector2(topStartX + sectionWidth, 1f);
        uv[8] = new Vector2(topStartX, 0f);
        uv[9] = new Vector2(topStartX + sectionWidth, 0f);

        // Bottom
        uv[12] = new Vector2(bottomStartX, 1f);
        uv[13] = new Vector2(bottomStartX, 0f);
        uv[14] = new Vector2(bottomStartX + sectionWidth, 0f);
        uv[15] = new Vector2(bottomStartX + sectionWidth, 1f);

        mesh.uv = uv;
    }
}
