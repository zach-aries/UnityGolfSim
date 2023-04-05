using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// color pallettes
// https://colorhunt.co/palette/6096b493bfcfbdcdd6eee9da
// https://colorhunt.co/palette/f5f0bbc4dfaa90c8ac73a9ad

public class Cell
{
    public float height;
    public Color color;

    private Color rough = new Color(144f / 255f, 200f / 255f, 172f / 255f); // rgb(144, 200, 172)
    private Color ocean = new Color(96f / 255f, 150f / 255f, 180f / 255f); // rgb(96, 150, 180)
    private Color sand = new Color(238f / 255f, 233f / 255f, 218f / 255f); // rgb(238, 233, 218)

    public Cell(float height, float waterLevel)
    {
        this.height = height;

        if (height < waterLevel)
            this.color = sand;
        else
            this.color = rough;
    }
}