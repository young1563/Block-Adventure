using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShapeData), false)]
[CanEditMultipleObjects]
[Serializable]
public class ShapeDataDrawer : Editor
{
    private ShapeData ShapeDataInstance => target as ShapeData;

    //인스펙터 창을 원하는 UI로 다시 그리는 함수
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ClearBoardButton();
        EditorGUILayout.Space();

        DrawColumnsInputFields();
        EditorGUILayout.Space();

        if(ShapeDataInstance.board != null && ShapeDataInstance.columns > 0 && ShapeDataInstance.rows > 0)
        {
            DrawBoardTable();
        }

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(ShapeDataInstance);
        }

    }
    private void ClearBoardButton()
    {
        if (GUILayout.Button("Clear Board"))
        {
            ShapeDataInstance.Clear();
        }

    }
    private void DrawColumnsInputFields()
    {
        int columnsTemp = ShapeDataInstance.columns;
        int rowsTemp = ShapeDataInstance.rows;

        ShapeDataInstance.columns = EditorGUILayout.IntField("Columns", ShapeDataInstance.columns);
        ShapeDataInstance.rows = EditorGUILayout.IntField("Rows", ShapeDataInstance.rows);

        if (ShapeDataInstance.columns != columnsTemp || ShapeDataInstance.rows != rowsTemp &&
            ShapeDataInstance.columns > 0 && ShapeDataInstance.rows > 0)
        {
            ShapeDataInstance.CreateNewBoard();
        }

    }
    private void DrawBoardTable()
    {
        GUIStyle tableStyle = new GUIStyle("box");
        tableStyle.margin.left = 32;

        GUIStyle headerColumnStyle = new GUIStyle();
        headerColumnStyle.fixedWidth = 65;
        headerColumnStyle.alignment = TextAnchor.MiddleCenter;

        GUIStyle rowStyle = new GUIStyle();
        rowStyle.fixedHeight = 25;
        rowStyle.alignment = TextAnchor.MiddleCenter;

        GUIStyle dataFieldStyle = new GUIStyle(EditorStyles.miniButtonMid);
        dataFieldStyle.normal.background = Texture2D.grayTexture;
        dataFieldStyle.onNormal.background = Texture2D.whiteTexture;

        for(int row = 0; row<ShapeDataInstance.rows; row++)
        {
            EditorGUILayout.BeginHorizontal(headerColumnStyle);
            
            for(int column =0; column<ShapeDataInstance.columns; column++)
            {
                EditorGUILayout.BeginHorizontal(rowStyle);
                bool data = EditorGUILayout.Toggle(ShapeDataInstance.board[row].column[column], dataFieldStyle);
                ShapeDataInstance.board[row].column[column] = data;
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndHorizontal();
        }

    }
}
