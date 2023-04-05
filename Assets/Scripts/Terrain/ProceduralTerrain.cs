using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProceduralTerrain : MonoBehaviour
{
    public static Mesh GenerateMesh(float[,] noiseMap, float heightScale)
    {
        int size = noiseMap.GetLength(0) - 1;
        Vector3[] vertices = new Vector3[(size + 1) * (size + 1)];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[size * size * 6];

        for (int y = 0; y <= size; y++)
        {
            for (int x = 0; x <= size; x++)
            {
                float height = noiseMap[x, y] * heightScale;

                Vector3 vertex = new Vector3(x, height, y);
                vertices[y * (size + 1) + x] = vertex;
                uvs[y * (size + 1) + x] = new Vector2((float)x / size, (float)y / size);
            }
        }

        int triangleIndex = 0;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int vertexIndex = y * (size + 1) + x;

                triangles[triangleIndex + 0] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + size + 1;
                triangles[triangleIndex + 2] = vertexIndex + 1;

                triangles[triangleIndex + 3] = vertexIndex + 1;
                triangles[triangleIndex + 4] = vertexIndex + size + 1;
                triangles[triangleIndex + 5] = vertexIndex + size + 2;

                triangleIndex += 6;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

}
