using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public static Cell[,] GenerateGrid(float[,] noiseMap, float waterLevel)
    {
        int size = noiseMap.GetLength(0) - 1;
        Cell[,] grid = new Cell[size, size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = noiseMap[x, y];
                Cell cell = new Cell(noiseValue, waterLevel);
                grid[x, y] = cell;
            }
        }

        return grid;
    }
}
