using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;

        base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
        {
            mapGen.CreateNewMap(mapGen.MazeSize, mapGen.Seed, mapGen.LevelOfDetail, mapGen.PieceDensity, mapGen.pieceAdvancement);
        }
        else if (GUILayout.Button("Update"))
        {
            mapGen.UpdateMap(mapGen.MazeSize, mapGen.Seed, mapGen.LevelOfDetail);
        }
        else if (GUILayout.Button("Delete"))
        {
            mapGen.DeleteMap();
        }
        //else if (GUILayout.Button("Move walls"))
        //{
        //    mapGen.MoveWalls(1);
        //}
    }
}
