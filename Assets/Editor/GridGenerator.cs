using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GridGenerator : EditorWindow
{
    private GameMode gameMode;
    private GameObject objPrefabs;
    private  List<Texture2D> textures = new List<Texture2D>(10);
    private int row;
    private int column;
    private Vector2 ScrollPosition;
    //private int selected = 0;
    private int textureSize;

    [MenuItem("Reza/GridGenerator")]
    public static void OpenWindow()
    {
        GetWindow<GridGenerator>();
    }

    private void OnGUI()
    {
        objPrefabs = EditorGUILayout.ObjectField("Objects Prefabs",
            objPrefabs, typeof(GameObject), true) as GameObject; 
        row = EditorGUILayout.IntField("Row", row);
        column = EditorGUILayout.IntField("Column", column);

        if (GUILayout.Button("Generate"))
        {
            GenerateGrid();
        }
    }



    private void GenerateGrid()
    {
        Debug.Log("Grid generator Called!");

        GameObject obj = GameObject.FindGameObjectWithTag("GameMode");

        if (!obj)
        {
            gameMode = new GameObject("GameMode").AddComponent<GameMode>();
            gameMode.tag = "GameMode";
        }
        else
        {
            gameMode = obj.GetComponent<GameMode>();
        }

        gameMode.SetupGrid(row, column);
    }

}