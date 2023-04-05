using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Terrain))]
public class TerrainInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Terrain terrain = (Terrain)target;

        if (GUILayout.Button("Generate Mesh"))
        {
            terrain.GenerateTerrain();
        }
    }
}
