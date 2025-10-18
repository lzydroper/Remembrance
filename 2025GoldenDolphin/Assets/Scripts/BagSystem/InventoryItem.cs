using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    public ItemData itemData;
    
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