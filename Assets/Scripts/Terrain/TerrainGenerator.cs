using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainGenerator : MonoBehaviour 
{
    [SerializeField] int xSize = 10;
    [SerializeField] int zSize = 10;
    [SerializeField] int xOffset;
    [SerializeField] int zOffset;
    [SerializeField] float noiseScale = 0.03f;
    [SerializeField] float heightMultiplier = 7;
    [SerializeField] Gradient terrainGradient;
    [SerializeField] Material mat;

    private Mesh mesh;
    private Texture2D gradientTexture;

    void Start() 
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        
        // Initialize texture once instead of every frame
        gradientTexture = new Texture2D(1, 100);
        gradientTexture.wrapMode = TextureWrapMode.Clamp;

        GenerateTerrain();
        GradientToTexture();
        UpdateShaderProperties();
    }

    private void GradientToTexture() 
    {
        Color[] pixelColors = new Color[100];
        for(int i = 0; i < 100; i++) 
        {
            pixelColors[i] = terrainGradient.Evaluate((float)i / 100f);
        }
        gradientTexture.SetPixels(pixelColors);
        gradientTexture.Apply();
    }

    private void UpdateShaderProperties()
    {
        if (mat == null) return;

        float minTerrainHeight = mesh.bounds.min.y + transform.position.y - 0.1f;
        float maxTerrainHeight = mesh.bounds.max.y + transform.position.y + 0.1f;

        // Note: Ensure your custom shader property names exactly match these strings (e.g., "_TerrainGradient")
        mat.SetTexture("_TerrainGradient", gradientTexture);
        mat.SetFloat("_MinTerrainHeight", minTerrainHeight);
        mat.SetFloat("_MaxTerrainHeight", maxTerrainHeight);
    }

    private void GenerateTerrain() 
    {
        // VERTICES
        Vector3[] vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        int i = 0;
        for(int z = 0; z <= zSize; z++) 
        {
            for(int x = 0; x <= xSize; x++) 
            {
                float yPos = Mathf.PerlinNoise((x + xOffset) * noiseScale, (z + zOffset) * noiseScale) * heightMultiplier;
                vertices[i] = new Vector3(x, yPos, z);
                i++;
            }
        }

        // TRIANGLES
        int[] triangles = new int[xSize * zSize * 6];
        int triangleIndex = 0;

        for(int z = 0; z < zSize; z++) 
        {
            for(int x = 0; x < xSize; x++) 
            {
                // Current row vertex index logic
                int currentVertex = x + (z * (xSize + 1));

                triangles[triangleIndex + 0] = currentVertex + 0;
                triangles[triangleIndex + 1] = currentVertex + xSize + 1;
                triangles[triangleIndex + 2] = currentVertex + 1;
                triangles[triangleIndex + 3] = currentVertex + 1;
                triangles[triangleIndex + 4] = currentVertex + xSize + 1;
                triangles[triangleIndex + 5] = currentVertex + xSize + 2;

                triangleIndex += 6;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
