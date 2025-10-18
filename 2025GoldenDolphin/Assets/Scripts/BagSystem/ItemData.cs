using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: 添加目录
[CreateAssetMenu]
public class ItemData : ScriptableObject
{
    [Tooltip("物品的名称")]
    public string itemName;
    
    [Tooltip("物品在UI中显示的图标")]
    public Sprite icon;

    [Tooltip("定义物品的形状。使用相对于锚点(0,0)的坐标列表。例如，一个1x3的竖条可以是 (0,0), (0,1), (0,2)。")]
    public List<Vector2Int> shape = new List<Vector2Int> { Vector2Int.zero }; // 默认为1x1
}
