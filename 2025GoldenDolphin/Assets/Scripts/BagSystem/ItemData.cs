using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: 添加目录
[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    public int width = 1;
    public int height = 1;
    // public bool[,] shape;

    public Sprite itemIcon;

    [System.Serializable]
    public class ShapeRow
    {
        public bool[] columns;
        
        public ShapeRow(int length)
        {
            columns = new bool[length];
            for (int i = 0; i < length; i++)
            {
                columns[i] = true;
            }
        }
    }
    
    public ShapeRow[] shape;

    public void InitializeShape()
    {
        shape = new ShapeRow[height];
        for (int i = 0; i < height; i++)
        {
            shape[i] = new ShapeRow(width);
        }
    }
    private void OnValidate()
    {
        if (shape == null || shape.Length != height)
        {
            InitializeShape();
        }
        else
        {
            for (int i = 0; i < shape.Length; i++)
            {
                if (shape[i].columns == null || shape[i].columns.Length != width)
                {
                    shape[i].columns = new bool[width];
                    for (int j = 0; j < width; j++)
                    {
                        shape[i].columns[j] = true;
                    }
                }
            }
        }
    }

    public bool[,] GetShapeArray()
    {
        bool[,] result = new bool[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                result[x, y] = shape[y].columns[x];
            }
        }

        return result;
    }

    public bool[,] GetRotatedShape(bool rotated)
    {
        if (!rotated)
        {
            return GetShapeArray();
        }
        bool[,] original = GetShapeArray();
        bool[,] rotatedShape = new bool[height, width];
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                rotatedShape[height - 1 - y, x] = original[x, y];
            }
        }
        
        return rotatedShape;
    }
}
