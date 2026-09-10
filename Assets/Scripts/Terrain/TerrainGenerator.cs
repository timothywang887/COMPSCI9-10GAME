using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class TerrainGenerator : MonoBehaviour 
{
    [Header("Dimensions")]
    [SerializeField] int xSize = 20;
    [SerializeField] int zSize = 20;
    [SerializeField] int xOffset = 0;
    [SerializeField] int zOffset = 0;

    [Header("Noise Settings")]
    [SerializeField] string generationSeed = "Seed";
    [Range(1, 6)] [SerializeField] int noiseLayers = 3;
    [SerializeField] float noiseScale = 0.03f;
    [SerializeField] float heightMultiplier = 7f;
    [Range(0f, 1f)] [SerializeField] float persistence = 0.4f;
    [SerializeField] float lacunarity = 2.5f;

    [Header("Texture Cell Settings (Anti-Repeating)")]
    [SerializeField] float textureFrequency = 4.0f; 
    [Range(0f, 0.3f)] [SerializeField] float hueVariance = 0.04f;
    [Range(0f, 0.5f)] [SerializeField] float saturationVariance = 0.1f;
    [Range(0f, 0.5f)] [SerializeField] float brightnessVariance = 0.15f;

    [Header("Stylization")]
    [SerializeField] Gradient terrainGradient;
    [SerializeField] Material mat;

    private Mesh mesh;
    private Texture2D gradientTexture;
    private MeshCollider meshCollider;

    private float trueMinHeight = 0f;
    private float trueMaxHeight = 1f;

    void Start() 
    {
        InitializeComponents();
        GenerateAndSync();
    }

    private void OnValidate()
    {
        InitializeComponents();
        GenerateAndSync();
    }

    private void InitializeComponents()
    {
        meshCollider = GetComponent<MeshCollider>();
        int groundLayerIndex = LayerMask.NameToLayer("Ground");
        if (groundLayerIndex != -1) gameObject.layer = groundLayerIndex;

        if (mesh == null)
        {
            mesh = new Mesh();
            mesh.name = "ProceduralTerrain";
            GetComponent<MeshFilter>().mesh = mesh;
        }

        if (gradientTexture == null)
        {
            gradientTexture = new Texture2D(1, 100);
            gradientTexture.wrapMode = TextureWrapMode.Clamp;
        }
    }

    private void GenerateAndSync()
    {
        GenerateTerrainStructure();
        GradientToTexture();
        UpdateShaderProperties();
    }

    private void GradientToTexture() 
    {
        if (gradientTexture == null || terrainGradient == null) return;

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
        if (mat == null || mesh == null) return;
        
        mat.SetTexture("_TerrainGradient", gradientTexture);
        mat.SetFloat("_MinTerrainHeight", trueMinHeight);
        mat.SetFloat("_MaxTerrainHeight", trueMaxHeight);
        
        int seedHash = generationSeed.GetHashCode() % 50000;
        // Generate diverse coordinates to break all axes of orientation repetition
        mat.SetVector("_SeedOffset", new Vector4(seedHash * 0.13f, seedHash * 0.71f, seedHash * 0.43f, seedHash));

        mat.SetFloat("_TexFrequency", textureFrequency);
        mat.SetFloat("_HueRand", hueVariance);
        mat.SetFloat("_SatRand", saturationVariance);
        mat.SetFloat("_ValRand", brightnessVariance);
    }

    private void GenerateTerrainStructure() 
    {
        int seedHash = generationSeed.GetHashCode() % 50000;
        Vector3[] gridVertices = new Vector3[(xSize + 1) * (zSize + 1)];
        int i = 0;

        trueMinHeight = float.MaxValue;
        trueMaxHeight = float.MinValue;

        for(int z = 0; z <= zSize; z++) 
        {
            for(int x = 0; x <= xSize; x++) 
            {
                float amplitude = 1f;
                float frequency = 1f;
                float yPos = 0f;

                for (int l = 0; l < noiseLayers; l++)
                {
                    float xCoord = (x + xOffset + seedHash) * noiseScale * frequency;
                    float zCoord = (z + zOffset + seedHash) * noiseScale * frequency;
                    
                    yPos += Mathf.PerlinNoise(xCoord, zCoord) * amplitude;
                    
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                float finalHeight = yPos * heightMultiplier;
                gridVertices[i] = new Vector3(x, finalHeight, z);

                if (finalHeight < trueMinHeight) trueMinHeight = finalHeight;
                if (finalHeight > trueMaxHeight) trueMaxHeight = finalHeight;

                i++;
            }
        }

        Vector3[] flatVertices = new Vector3[xSize * zSize * 6];
        int[] flatTriangles = new int[xSize * zSize * 6];
        int currentTriangleIndex = 0;

        for(int z = 0; z < zSize; z++) 
        {
            for(int x = 0; x < xSize; x++) 
            {
                int vertexRow0 = x + (z * (xSize + 1));
                int vertexRow1 = x + ((z + 1) * (xSize + 1));

                Vector3 tl = gridVertices[vertexRow1];
                Vector3 tr = gridVertices[vertexRow1 + 1];
                Vector3 bl = gridVertices[vertexRow0];
                Vector3 br = gridVertices[vertexRow0 + 1];

                flatVertices[currentTriangleIndex + 0] = bl;
                flatVertices[currentTriangleIndex + 1] = tl;
                flatVertices[currentTriangleIndex + 2] = tr;

                flatVertices[currentTriangleIndex + 3] = tr;
                flatVertices[currentTriangleIndex + 4] = br;
                flatVertices[currentTriangleIndex + 5] = bl;

                for(int t = 0; t < 6; t++)
                {
                    flatTriangles[currentTriangleIndex + t] = currentTriangleIndex + t;
                }

                currentTriangleIndex += 6;
            }
        }

        mesh.Clear();

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        
        mesh.vertices = flatVertices;
        mesh.triangles = flatTriangles;
        mesh.RecalculateNormals();

        if (meshCollider != null)
        {
            meshCollider.sharedMesh = null; 
            meshCollider.sharedMesh = mesh;
        }
    }
}
