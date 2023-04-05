using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureGenerator : MonoBehaviour
{
    private static Texture2D TextureFromColourMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        // texture.filterMode = FilterMode.Trilinear;

        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    public static Texture2D GenerateTexture(Cell[,] grid, bool greyScale = false)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        Color[] colorMap = new Color[width * height];
        Gradient gradient = new Gradient();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Cell cell = grid[x, y];
                Color color = greyScale ? Color.Lerp(Color.black, Color.white, cell.height) : cell.color;
                colorMap[y * width + x] = color;
            }
        }


        return TextureFromColourMap(colorMap, width, height);
    }
}
