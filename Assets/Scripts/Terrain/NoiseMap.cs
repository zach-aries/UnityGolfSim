using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMap : MonoBehaviour
{
    private static float PerlinNoise(float x, float y, float lacunarity, float persistence, int octaves)
    {
        float noiseValue = 0f;
        float frequency = 1f;
        float amplitude = 1f;

        for (int i = 0; i < octaves; i++)
        {
            float perlinValue = Mathf.PerlinNoise(x * frequency, y * frequency);
            noiseValue += perlinValue * amplitude;
            frequency *= lacunarity;
            amplitude *= persistence;
        }

        return noiseValue;
    }

    public static float[,] GenerateNoiseMap(int size, float scale, float lacunarity, float persistence, int octaves)
    {
        size = size + 1;
        // float scale = size / baseScale;
        float[,] noiseMap = new float[size, size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = PerlinNoise(x * scale, y * scale, lacunarity, persistence, octaves);
                noiseMap[x, y] = noiseValue;
            }
        }

        float halfSize = size / 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distanceFromCenter = Mathf.Sqrt((x - halfSize) * (x - halfSize) + (y - halfSize) * (y - halfSize));
                float interpolationValue = Mathf.Pow(distanceFromCenter / halfSize, 2.5f);
                float falloffValue = Mathf.SmoothStep(1f, 0f, interpolationValue);


                noiseMap[x, y] = noiseMap[x, y] * falloffValue;
            }
        }

        return noiseMap;
    }
}
