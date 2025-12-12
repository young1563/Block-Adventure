using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shape", menuName = "ShapeData")]
[Serializable]
public class ShapeData : ScriptableObject
{
    [Serializable]
    public class Row
    {
        public bool[] column;
        private int _size = 0;
                
        public Row(int size)
        {
            CreateRow(size);
        }
        public void CreateRow(int size)
        {
            _size = size;
            column = new bool[_size];
            ClearRow();
        }

        public void ClearRow()
        {
            for(int i=0; i<_size; i++)
            {
                column[i] = false;
            }
        }
    }

    public int rows = 0;
    public int columns = 0;
    public Row[] board;

    public void Clear()
    {
        if (board == null || board.Length == 0)
        {
            Debug.LogWarning("보드 초기화 안됨");
            return;
        }

        for(int i=0; i< rows; i++)
        {
            board[i].ClearRow();            
        }
    }

    public void CreateNewBoard()
    {
        board = new Row[rows];

        for (int i = 0; i < rows; i++)
        {
            board[i] = new Row(columns);
        }
    }
}
