using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineIndicator : MonoBehaviour
{
    public int[,] line_data = new int[9, 9] // 그리드 전체맵
    {
        {  0,  1,  2,   3,  4,  5,   6,  7,  8 },
        {  9, 10, 11,  12, 13, 14,  15, 16, 17 },
        { 18, 19, 20,  21, 22, 23,  24, 25, 26 },

        { 27, 28, 29,  30, 31, 32,  33, 34, 35 },
        { 36, 37, 38,  39, 40, 41,  42, 43, 44 },
        { 45, 46, 47,  48, 49, 50,  51, 52, 53 },
                                    
        { 54, 55, 56,  57, 58, 59,  60, 61, 62 },
        { 63, 64, 65,  66, 67, 68,  69, 70, 71 },
        { 72, 73, 74,  75, 76, 77,  78, 79, 80 }
    };

    public int[,] square_data = new int[9, 9] // 3X3 블록맵
    {
        // Block 0 (좌상단)
        { 0, 1, 2, 9, 10, 11, 18, 19, 20 },

        // Block 1 (상단 가운데)
        { 3, 4, 5, 12, 13, 14, 21, 22, 23 },

        // Block 2 (상단 오른쪽)
        { 6, 7, 8, 15, 16, 17, 24, 25, 26 },

        // Block 3 (중간 왼쪽)
        { 27, 28, 29, 36, 37, 38, 45, 46, 47 },

        // Block 4 (중간 가운데)
        { 30, 31, 32, 39, 40, 41, 48, 49, 50 },

        // Block 5 (중간 오른쪽)
        { 33, 34, 35, 42, 43, 44, 51, 52, 53 },

        // Block 6 (하단 왼쪽)
        { 54, 55, 56, 63, 64, 65, 72, 73, 74 },

        // Block 7 (하단 가운데)
        { 57, 58, 59, 66, 67, 68, 75, 76, 77 },

        // Block 8 (하단 오른쪽)
        { 60, 61, 62, 69, 70, 71, 78, 79, 80 }
    };

    [HideInInspector]
    public int[] columnIndexes = new int[9] // 세로줄 인덱스
    {
        0,1,2,3,4,5,6,7,8
    };

    private (int, int) GetSquarePosition(int square_index) // 칸 번호 -> 좌표 변환
    {
        int row = square_index / 9;
        int col = square_index & 9;
       
        return (row, col);
    }

    public int[] GetVerticalLine(int square_index) //세로즐(y)
    {
        int[] line = new int[9];

        int square_position_col = GetSquarePosition(square_index).Item2;

        for(int i = 0; i<9; i++)
        {
            line[i] = line_data[i, square_position_col];
        }
        return line;
    }

    public int GetGridSquareIndex(int sqaure)
    {
        for(int row = 0; row<9; row++)
        {
            for(int column = 0; column <9; column++)
            {
                if(square_data[row, column] == sqaure)
                {
                    return row;
                }
            }
        }
        return -1;
    }
}
