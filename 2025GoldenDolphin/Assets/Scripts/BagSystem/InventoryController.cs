using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BagSystem;
using SKCell; // [NEW] 用于构建详细的debug信息
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    // --- 网格设置 ---
    public readonly int gridSizeX = 4;
    public readonly int gridSizeY = 4;

    // [NEW] Inspector中可配置的参数
    [Header("Configuration")]
    [Tooltip("背包网格中每个格子的世界单位大小")]
    [SerializeField] private float cellSize = 1f;
    [Tooltip("光标的初始网格坐标")]
    [SerializeField] private Vector2Int initialCursorPosition = new Vector2Int(1, 1);

    [Header("Visuals & Scene References")]
    // [SerializeField] private InventoryItem inventoryItemPrefab;
    [Tooltip("用于表示光标位置的Transform")]
    [SerializeField] private Transform cursorVisual;
    [Tooltip("所有属于此背包的物品实例的父对象")]
    [SerializeField] private Transform itemContainer;

    // --- 状态变量 ---
    private InventoryItem[,] grid;
    private List<InventoryItem> placedItems = new List<InventoryItem>();
    // private Vector2Int cursorPosition;
    private InventoryItem heldItem = null;
    // private bool isCursorInUI = false;
    // [NEW] 记录光标进入UI区域前的位置
    // private Vector2Int lastValidGridPosition;
    

    // --- 事件 ---
    public event Action onCursorEnterUI = delegate { };
    public event Action onCursorLeaveUI = delegate { };
    // public event Action<InventoryItem, bool> OnPlacementValidityChanged = delegate { };

    private void Awake()
    {
        grid = new InventoryItem[gridSizeX, gridSizeY];
        // // 确保初始位置不是无效的角落
        // if (!IsGridPositionAllowedForCursor(initialCursorPosition))
        // {
        //     Debug.LogWarning($"初始光标位置 {initialCursorPosition} 无效，已重置为 (1, 1)");
        //     initialCursorPosition = new Vector2Int(1, 1);
        // }
        // cursorPosition = initialCursorPosition;
        // lastValidGridPosition = cursorPosition; // [NEW] 初始化记录
    }

    // private void Start()
    // {
    //     // [NEW] 游戏开始时，根据初始位置更新一次光标视觉效果
    //     UpdateCursorVisualPosition();
    // }

    #region Public Control API
    
    public void OnCursorEnter()
    {
        Debug.Log($"[{gameObject.name}] 光标已进入我的区域。");
        if (cursorVisual) cursorVisual.gameObject.SetActive(true);
    }
    
    public void OnCursorExit()
    {
        Debug.Log($"[{gameObject.name}] 光标已离开我的区域。");
        if (cursorVisual) cursorVisual.gameObject.SetActive(false);
    }
    
    public void OnCursorMove(Vector2Int localGridPosition)
    {
        // 更新光标的视觉位置
        if (cursorVisual) cursorVisual.position = GridToWorldPosition(localGridPosition);
        
        // 如果正拿着物品，更新物品的状态和位置
        if (heldItem != null)
        {
            heldItem.anchorGridPosition = localGridPosition;
            UpdateHeldItemVisualState();
        }
    }
    
    public void OnConfirmAction(Vector2Int localGridPosition)
    {
        if (heldItem != null) // 如果手上有物品，尝试放置
        {
            // 注意: 检查有效性时，物品的位置已经是正确的 (在OnCursorMove里更新了)
            if (CheckPlacementValidity(heldItem))
            {
                PlaceHeldItem();
            }
            else
            {
                Debug.LogWarning($"[{gameObject.name}] 无法放置物品。");
            }
        }
        // 不准拾取T_T
        // else // 如果手上没物品，尝试拾取
        // {
        //     InventoryItem itemToPickUp = grid[localGridPosition.x, localGridPosition.y];
        //     if (itemToPickUp != null)
        //     {
        //         PickUpItem(itemToPickUp);
        //     }
        // }
    }
    
    public void RotateHeldItem()
    {
        if (heldItem != null)
        {
            heldItem.Rotate();
            UpdateHeldItemVisualState();
        }
    }

    // public void MoveCursor(Vector2Int direction)
    // {
    //     Vector2Int newPos = cursorPosition + direction;
    //
    //     if (newPos.y < 0)
    //     {
    //         if (!isCursorInUI)
    //         {
    //             isCursorInUI = true;
    //             lastValidGridPosition = cursorPosition;
    //             onCursorEnterUI.Invoke();
    //             if(cursorVisual) cursorVisual.gameObject.SetActive(false); // [NEW] 进入UI时隐藏光标
    //             Debug.Log($"[{gameObject.name}] 光标进入UI区域，记录位置 {lastValidGridPosition}");
    //         }
    //         return;
    //     }
    //
    //     if (isCursorInUI && newPos.y >= 0)
    //     {
    //         isCursorInUI = false;
    //         onCursorLeaveUI.Invoke();
    //         if(cursorVisual) cursorVisual.gameObject.SetActive(true); // [NEW] 离开UI时显示光标
    //         // [MODIFIED] 恢复到之前记录的位置，而不是使用newPos
    //         cursorPosition = lastValidGridPosition;
    //         Debug.Log($"[{gameObject.name}] 光标离开UI区域，恢复到 {cursorPosition}");
    //         
    //         // 因为位置可能已改变，需要更新视觉
    //         UpdateCursorVisualPosition();
    //         if (heldItem != null)
    //         {
    //             heldItem.anchorGridPosition = cursorPosition;
    //             UpdateHeldItemVisualState();
    //         }
    //         return; // 结束本次移动逻辑
    //     }
    //     
    //     if (!IsGridPositionAllowedForCursor(newPos))
    //     {
    //         Debug.Log($"[{gameObject.name}] 移动被边界阻挡: 试图移动到 {newPos}");
    //         return;
    //     }
    //
    //     cursorPosition = newPos;
    //     Debug.Log($"[{gameObject.name}] 光标移动到: {cursorPosition}");
    //     
    //     UpdateCursorVisualPosition(); // [NEW] 更新光标视觉
    //
    //     if (heldItem != null)
    //     {
    //         heldItem.anchorGridPosition = cursorPosition;
    //         UpdateHeldItemVisualState(); // [MODIFIED] 这个函数现在也会更新位置
    //     }
    // }
    //
    // public void RotateHeldItem()
    // {
    //     if (heldItem != null)
    //     {
    //         heldItem.Rotate();
    //         Debug.Log($"[{gameObject.name}] 物品 '{heldItem.itemData.itemName}' 已旋转. 新旋转状态: {heldItem.currentRotation}");
    //         UpdateHeldItemVisualState();
    //     }
    //     else
    //     {
    //         Debug.Log($"[{gameObject.name}] 尝试旋转，但手中没有物品。");
    //     }
    // }
    //
    // public void ConfirmAction()
    // {
    //     if (isCursorInUI)
    //     {
    //         Debug.Log($"[{gameObject.name}] 在UI区域按下确认，背包无操作。");
    //         return;
    //     }
    //
    //     if (heldItem != null)
    //     {
    //         Debug.Log($"[{gameObject.name}] 尝试放置物品 '{heldItem.itemData.itemName}' 在 {cursorPosition}");
    //         if (CheckPlacementValidity(heldItem, cursorPosition))
    //         {
    //             PlaceHeldItem();
    //         }
    //         else
    //         {
    //             Debug.LogWarning($"[{gameObject.name}] 无法放置：位置不合法或已被占用。");
    //         }
    //     }
    //     else
    //     {
    //         InventoryItem itemToPickUp = grid[cursorPosition.x, cursorPosition.y];
    //         if (itemToPickUp != null)
    //         {
    //             Debug.Log($"[{gameObject.name}] 尝试在 {cursorPosition} 拾取物品。");
    //             PickUpItem(itemToPickUp);
    //         }
    //         else
    //         {
    //             Debug.Log($"[{gameObject.name}] 在 {cursorPosition} 按下确认，但该位置没有物品。");
    //         }
    //     }
    // }

    /// <summary>
    /// 添加一个新物品到玩家手中
    /// </summary>
    /// <param name="newItemInstance">待添加物品prefab</param>
    /// <param name="anchorPosition"></param>
    public void AddNewItemToHand(InventoryItem newItemInstance, Vector2Int anchorPosition)
    {
        // RemoveHeldItem();
        heldItem = newItemInstance;
        if(itemContainer != null) heldItem.transform.SetParent(itemContainer);
        else heldItem.transform.SetParent(this.transform);

        // 重置初始状态
        heldItem.anchorGridPosition = anchorPosition;
        UpdateHeldItemVisualState();
        OnCursorMove(Vector2Int.one);
        Debug.Log($"添加物品{newItemInstance.name}");
    }
    public void AddNewItemToHand(ItemData itemData)
    {
        int index = itemdb.items.IndexOf(itemData);
        InventoryItem newItemInstance = Instantiate<GameObject>(itemdb.prefabs[index]).GetComponentInChildren<InventoryItem>();
        // newItemInstance.init(itemData);
        AddNewItemToHand(newItemInstance, Vector2Int.one);
    }
    
    // 强制清除手持物品，并强制烹饪，返回烹饪结果
    // 时间到后由actionPhase调用UI的相关接口来调用这个函数
    public ItemData RemoveHeldItem()
    {
        if (heldItem != null)
        {
            Destroy(heldItem.gameObject);
            return CookIt();
        }
        return null;
    }

    /// <summary>
    /// 删除一个特定物品，需持有该物品的引用，组合物品时使用
    /// </summary>
    /// <param name="item"></param>
    public void DeleteItem(InventoryItem item)
    {
        if (placedItems.Contains(item))
        {
            RemoveItemFromGrid(item);
            placedItems.Remove(item);
            Destroy(item.gameObject);
            Debug.Log($"[{gameObject.name}] 物品 '{item.itemData.itemName}' 已被删除");
        }
    }

    #endregion

    #region Private Logic & Visuals

    /// [NEW] 将网格坐标转换为世界坐标
    private Vector3 GridToWorldPosition(Vector2Int gridPos)
    {
        // 1. 计算出目标格子相对于网格中心的坐标（以格子为单位）
        //    例如，在一个4x4网格中，中心是(2, 2)。格子(0,0)相对于中心就是(-2, -2)。
        // 2. 我们需要定位到格子的中心，所以给每个轴加上0.5。
        //    格子(0,0)的中心就变成了相对于网格中心(-1.5, -1.5)个单位。
        // 3. 最后将这个格子单位的偏移量乘以cellSize，得到世界单位的偏移量。
        float xOffset = (gridPos.x - gridSizeX / 2.0f + 0.5f) * cellSize;
        float yOffset = (gridPos.y - gridSizeY / 2.0f + 0.5f) * cellSize;

        // 4. 将计算出的偏移量应用到GameObject的中心位置上。
        return transform.position + new Vector3(xOffset, yOffset, 0);
    }
    
    // private void UpdateCursorVisualPosition()
    // {
    //     if (cursorVisual != null)
    //     {
    //         cursorVisual.position = GridToWorldPosition(cursorPosition);
    //     }
    // }

    private void PlaceHeldItem()
    {
        Debug.Log($"[{gameObject.name}] 成功放置物品 '{heldItem.itemData.itemName}' 在 {heldItem.anchorGridPosition}");
        
        placedItems.Add(heldItem);
        foreach (var pos in heldItem.GetOccupiedGridPositions())
        {
            grid[pos.x, pos.y] = heldItem;
        }
        // 放置后更新状态
        UpdateHeldItemVisualState(false);

        // 放置后，物品的位置已经是正确的，我们只需要清空手牌
        heldItem = null;
        // UpdateResultReview();
    }

    // private void UpdateResultReview()
    // {
    //     ItemData itemData = CheckCraftingRecipe();
    //     if (itemData is not null)
    //     {
    //         resultReview.init(itemData);
    //     }
    // }

    private void PickUpItem(InventoryItem itemToPickUp)
    {
        Debug.Log($"[{gameObject.name}] 成功拿起物品 '{itemToPickUp.itemData.itemName}'");
        
        RemoveItemFromGrid(itemToPickUp);
        placedItems.Remove(itemToPickUp);
        
        heldItem = itemToPickUp;
        // cursorPosition = heldItem.anchorGridPosition;

        // UpdateCursorVisualPosition();
        UpdateHeldItemVisualState();
        // UpdateResultReview();
    }

    private void RemoveItemFromGrid(InventoryItem item)
    {
        foreach (var pos in item.GetOccupiedGridPositions())
        {
            if (IsWithinBounds(pos) && grid[pos.x, pos.y] == item)
            {
                grid[pos.x, pos.y] = null;
            }
        }
    }

    // [MODIFIED] 增强了Debug信息
    private bool CheckPlacementValidity(InventoryItem item)
    {
        var occupiedPositions = item.GetOccupiedGridPositions();
        foreach (var pos in occupiedPositions)
        {
            if (!IsGridPositionAllowedForCursor(pos))
            {
                // [NEW] 详细的失败原因
                if (!IsWithinBounds(pos))
                    Debug.Log($"[{gameObject.name}] 放置无效: 部分超出边界 at {pos}");
                else
                    Debug.Log($"[{gameObject.name}] 放置无效: 部分位于禁用的角落 at {pos}");
                return false;
            }
            if (grid[pos.x, pos.y] != null)
            {
                Debug.Log($"[{gameObject.name}] 放置无效: 与物品 '{grid[pos.x, pos.y].itemData.itemName}' 重叠 at {pos}");
                return false;
            }
        }
        return true;
    }
    
    // [MODIFIED] 这个函数现在同时负责更新颜色和位置
    private void UpdateHeldItemVisualState(bool selected = true)
    {
        if (heldItem == null) return;
        
        // 更新位置
        heldItem.transform.position = GridToWorldPosition(heldItem.anchorGridPosition);
        
        // 更新有效性状态（颜色等）
        bool isValid = CheckPlacementValidity(heldItem);
        // OnPlacementValidityChanged.Invoke(heldItem, isValid);
        
        // 示例：直接改变颜色来反馈是否可放置
        heldItem.SetSprite(true, selected);
        var renderer = heldItem.GetComponentInChildren<Image>();
        if (renderer != null)
        {
            renderer.color = isValid ? Color.white : Color.red;
        }
    }
    
    public bool IsValidUniversalPosition(Vector2Int universalPosition, RectInt inventoryBounds)
    {
        // 1. 首先检查这个通用坐标是否在背包的矩形边界内
        if (!inventoryBounds.Contains(universalPosition))
        {
            return false;
        }

        // 2. 如果在边界内，将其转换为背包的本地坐标
        Vector2Int localPosition = universalPosition - inventoryBounds.min;

        // 3. 使用我们之前写好的内部逻辑来检查这个本地坐标是否是角落
        return IsGridPositionAllowed(localPosition); 
    }
    
    private bool IsGridPositionAllowed(Vector2Int pos) { 
        if (!IsWithinBounds(pos)) return false;
        // 四个角的本地坐标是 (0,0), (0,3), (3,0), (3,3)
        if ((pos.x == 0 || pos.x == gridSizeX - 1) && (pos.y == 0 || pos.y == gridSizeY - 1)) return false;
        return true;
    }
    
    private bool IsGridPositionAllowedForCursor(Vector2Int pos)
    {
        // 检查1：是否在边界内
        if (!IsWithinBounds(pos)) return false;
        
        // 检查2：是否为四个角
        if ((pos.x == 0 || pos.x == gridSizeX - 1) && (pos.y == 0 || pos.y == gridSizeY - 1)) return false;

        return true;
    }

    private bool IsWithinBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridSizeX && pos.y >= 0 && pos.y < gridSizeY;
    }

    #endregion

    [SerializeField] private Itemdb itemdb;
    [SerializeField] private ItemData shit;
    public ItemData CookIt()
    {
        List<ItemData> itemsInBag = placedItems.Select(item => item.itemData).ToList();
        if (itemsInBag.Count == 0)
            return null;

        // 2. 遍历数据库中的每一个配方
        foreach (var recipe in itemdb.recipes)
        {
            // 3. 检查数量是否一致。这是一个快速的优化，如果数量都对不上，肯定不是这个配方。
            if (itemsInBag.Count != recipe.ingredients.Count)
            {
                continue; // 跳过这个配方，检查下一个
            }

            // 4. 【核心】处理无序匹配：将两边的列表都进行排序，然后比较。
            //    我们通过物品名称(itemName)来排序，确保排序结果的唯一性和稳定性。
            var sortedItemsInBag = itemsInBag.Select(data => data.itemName).OrderBy(name => name).ToList();
            var sortedRecipeIngredients = recipe.ingredients.Select(data => data.itemName).OrderBy(name => name).ToList();
            
            // 5. 使用 LINQ 的 SequenceEqual 来比较两个排好序的列表是否完全相同。
            if (sortedItemsInBag.SequenceEqual(sortedRecipeIngredients))
            {
                // 找到了匹配的配方！
                Debug.Log($"合成匹配成功！配方：{string.Join(", ", sortedRecipeIngredients)} -> {recipe.result.itemName}");
                return recipe.result;
            }
        }

        // 遍历完所有配方都没有找到匹配的
        Debug.Log("未找到匹配的合成配方。");
        // 有消耗物品，清除背包内放置物品
        placedItems.Clear();
        return shit;
    }
}