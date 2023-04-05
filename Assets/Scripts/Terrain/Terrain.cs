using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    [Range(0, 100)]
    public int gridSize = 100;
    [Range(0, 1)]
    public float scale = .1f;
    [Range(0.1f, 10)]
    public float heightScale = 1f;
    [Range(2, 4)]
    public float lacunarity = 2f;
    [Range(0, 1)]
    public float persistence = 0.5f;
    [Range(1, 7)]
    public int octaves = 4;
    [Range(0, 1)]
    public float waterLevel = .4f;
    public Material terrainMaterial;
    public bool greyScale = false;


    void Start()
    {
        GenerateTerrain();
    }

    public void GenerateTerrain()
    {
        float[,] noiseMap = NoiseMap.GenerateNoiseMap(gridSize, scale, lacunarity, persistence, octaves);
        Cell[,] grid = Grid.GenerateGrid(noiseMap, waterLevel);
        Mesh mesh = ProceduralTerrain.GenerateMesh(noiseMap, heightScale);

        GetComponent<MeshFilter>().mesh = mesh;
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = mesh;
        }

        DrawTexture(grid);
    }

    void DrawTexture(Cell[,] grid)
    {
        Texture2D texture = TextureGenerator.GenerateTexture(grid, greyScale);

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = terrainMaterial;
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}

