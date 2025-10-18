using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    private RectTransform rectTransform;
    public ItemData itemData;
    public Image itemIcon;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void init(ItemData itemData)
    {
        this.itemData = itemData;
        this.itemIcon.sprite = this.itemData.icon;
        UpdateVisuals();
    }
    
    /// <summary>
    /// 根据当前数据（形状和旋转）更新UI元素的尺寸和旋转
    /// </summary>
    private void UpdateVisuals()
    {
        // 1. 计算旋转后形状的尺寸（单位：格子）
        Vector2Int shapeDimensions = GetShapeDimensions(GetRotatedShape());

        // 2. 计算最终的UI像素尺寸
        Vector2 newSize = new Vector2(shapeDimensions.x * Constants.cellSize, shapeDimensions.y * Constants.cellSize);

        // 3. 应用新的尺寸
        rectTransform.sizeDelta = newSize;

        // 4. 应用旋转
        // 使用z轴旋转来匹配2D UI的旋转
        rectTransform.localRotation = Quaternion.Euler(0, 0, -90 * currentRotation);
    }

    /// <summary>
    /// 计算给定形状列表的边界框尺寸（宽度和高度，单位：格子）
    /// </summary>
    private Vector2Int GetShapeDimensions(List<Vector2Int> shape)
    {
        if (shape == null || shape.Count == 0)
        {
            return Vector2Int.zero;
        }

        // 初始化边界
        int minX = shape[0].x;
        int maxX = shape[0].x;
        int minY = shape[0].y;
        int maxY = shape[0].y;

        // 遍历所有点，找到最大和最小的x, y坐标
        foreach (var point in shape)
        {
            minX = Mathf.Min(minX, point.x);
            maxX = Mathf.Max(maxX, point.x);
            minY = Mathf.Min(minY, point.y);
            maxY = Mathf.Max(maxY, point.y);
        }

        // 宽度和高度是最大值减最小值再加1（因为(0,0)到(0,2)是3个格子高）
        int width = maxX - minX + 1;
        int height = maxY - minY + 1;

        return new Vector2Int(width, height);
    }
    
    // 物品在背包网格中的锚点坐标
    public Vector2Int anchorGridPosition;
    
    // 0: 0°, 1: 90°, 2: 180°, 3: 270°
    public int currentRotation = 0;

    /// <summary>
    /// 获取物品旋转后所有占用的格子坐标（相对于锚点）
    /// </summary>
    public List<Vector2Int> GetRotatedShape()
    {
        var rotatedShape = new List<Vector2Int>();
        foreach (var point in itemData.shape)
        {
            switch (currentRotation)
            {
                case 0: // 0°
                    rotatedShape.Add(point);
                    break;
                case 1: // 90° clockwise
                    rotatedShape.Add(new Vector2Int(point.y, -point.x));
                    break;
                case 2: // 180°
                    rotatedShape.Add(new Vector2Int(-point.x, -point.y));
                    break;
                case 3: // 270° clockwise
                    rotatedShape.Add(new Vector2Int(-point.y, point.x));
                    break;
            }
        }
        return rotatedShape;
    }

    /// <summary>
    /// 获取物品在整个背包网格中实际占用的所有格子坐标
    /// </summary>
    public List<Vector2Int> GetOccupiedGridPositions()
    {
        var occupiedPositions = new List<Vector2Int>();
        var rotatedShape = GetRotatedShape();
        foreach (var point in rotatedShape)
        {
            occupiedPositions.Add(anchorGridPosition + point);
        }
        return occupiedPositions;
    }

    public void Rotate()
    {
        currentRotation = (currentRotation + 1) % 4;
        // 你可以在这里添加旋转模型的视觉表现代码
        transform.Rotate(0, 0, -90);
    }
}