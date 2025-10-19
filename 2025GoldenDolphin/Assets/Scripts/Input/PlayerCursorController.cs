using System;
using SKCell;
using BagSystem;
using UnityEngine;

public class PlayerCursorController : MonoBehaviour
{
    // --- 上下文（Context）枚举 ---
    public enum CursorContext
    {
        None,      // 无效区域
        Inventory, // 在背包区域
        UI         // 在UI区域
    }

    // --- 引用 ---
    [Header("上下文引用")]
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private UIController uiController;

    [Header("区域边界配置")]
    [Tooltip("背包区域在通用坐标系中所占的矩形范围")]
    [SerializeField] private RectInt inventoryBounds = new RectInt(0, 0, 4, 4);
    
    [Tooltip("UI区域在通用坐标系中所占的矩形范围")]
    [SerializeField] private RectInt uiBounds = new RectInt(1, -1, 2, 1); // 示例: 一个在背包下方的2格宽UI区域

    // --- 状态 ---
    public Vector2Int _currentPosition = Vector2Int.one; // 光标当前的通用坐标
    public CursorContext _currentContext = CursorContext.None;

    // private void Start()
    // {
    //     // 设置初始位置和上下文
    //     _currentPosition = new Vector2Int(inventoryBounds.x + 1, inventoryBounds.y + 1);
    //     EnterContext(DetermineContext(_currentPosition));
    // }

    /// <summary>
    /// 核心移动方法，由 Player.cs 调用
    /// </summary>
    public void Move(Vector2Int direction)
    {
        if (_currentContext == CursorContext.UI && direction == Vector2Int.up)
        {
            Debug.Log($"[{gameObject.name}] 移动被阻止: 不允许从UI区域向上移动。");
            // (可选) 可以在这里播放一个“无效操作”的音效
            return; // 直接退出函数，不进行任何移动计算
        }
        
        Vector2Int newPosition = _currentPosition + direction;

        // 根据新位置判断新的上下文
        CursorContext newContext = DetermineContext(newPosition);
        //
        // // 如果我们当前在UI区域，并且目标位置在Inventory区域，则阻止这次移动。
        // if (_currentContext == CursorContext.UI && newContext == CursorContext.Inventory)
        // {
        //     Debug.Log($"[{gameObject.name}] 移动被阻止: 不允许从UI区域返回背包。");
        //     // 通过将 newContext 强制设为 None，我们可以利用下面已有的 "else" 逻辑来处理移动失败的情况。
        //     newContext = CursorContext.None;
        // }

        // 如果新位置是有效的（不属于None），则更新位置
        if (newContext != CursorContext.None)
        {
            _currentPosition = newPosition;
            
            // 检查上下文是否发生了变化
            if (newContext != _currentContext)
            {
                ExitContext(_currentContext); // 离开旧的上下文
                EnterContext(newContext);   // 进入新的上下文
            }
            
            // 通知当前的上下文，光标移动了
            NotifyMove();
            SKAudioManager.instance.PlaySound("move");
        }
        else
        {
            Debug.Log($"[{gameObject.name}] 移动到 {newPosition} 被阻止 (超出所有有效边界).");
        }
    }
    
    /// <summary>
    /// 核心确认方法
    /// </summary>
    public void Confirm()
    {
        Debug.Log($"[{gameObject.name}] 在上下文: {_currentContext} 的位置 {_currentPosition} 按下确认");
        switch (_currentContext)
        {
            case CursorContext.Inventory:
                // 将通用坐标转换为背包的本地坐标
                Vector2Int localInventoryPos = _currentPosition - inventoryBounds.min;
                inventoryController.OnConfirmAction(localInventoryPos);
                break;
            case CursorContext.UI:
                Vector2Int localUIPos = _currentPosition - uiBounds.min;
                uiController.OnConfirmAction(localUIPos);
                break;
        }
    }
    
    /// <summary>
    /// 核心旋转方法
    /// </summary>
    public void Rotate()
    {
        // 旋转操作只在背包上下文中有效
        if (_currentContext == CursorContext.Inventory)
        {
            inventoryController.RotateHeldItem();
        }
    }

    private CursorContext DetermineContext(Vector2Int position)
    {
        if (inventoryController != null && inventoryController.IsValidUniversalPosition(position, inventoryBounds))
        {
            return CursorContext.Inventory;
        }
        // TODO: 将区域判定逻辑改为自己的
        if (uiBounds.Contains(position))
        {
            if (inventoryController != null && inventoryController.IsHoldingItem)
            {
                // 手里有东西，不准进入UI区域，将此区域视为无效（None）
                Debug.Log("手持物品，无法进入UI区域！");
                return CursorContext.None; 
            }
            return CursorContext.UI;
        }

        return CursorContext.None;
    }

    private void EnterContext(CursorContext newContext)
    {
        _currentContext = newContext;
        Debug.Log($"[{gameObject.name}] 进入上下文: {newContext}");
        switch (newContext)
        {
            case CursorContext.Inventory:
                inventoryController.OnCursorEnter();
                NotifyMove(); // 进入时立刻通知一次位置
                break;
            case CursorContext.UI:
                uiController.OnCursorEnter();
                break;
        }
    }

    private void ExitContext(CursorContext oldContext)
    {
        Debug.Log($"[{gameObject.name}] 离开上下文: {oldContext}");
        switch (oldContext)
        {
            case CursorContext.Inventory:
                inventoryController.OnCursorExit();
                break;
            case CursorContext.UI:
                uiController.OnCursorExit();
                break;
        }
    }

    private void NotifyMove()
    {
        switch (_currentContext)
        {
            case CursorContext.Inventory:
                Vector2Int localInventoryPos = _currentPosition - inventoryBounds.min;
                inventoryController.OnCursorMove(localInventoryPos);
                break;
            case CursorContext.UI:
                Vector2Int localUIPos = _currentPosition - uiBounds.min;
                uiController.OnCursorMove(localUIPos);
                break;
        }
    }
    
    /// <summary>
    /// 立即将光标切换到UI区域的第一个元素。
    /// 如果玩家正手持物品，此操作将被阻止。
    /// </summary>
    public void SwitchToUI()
    {
        // 1. 前提条件检查：遵守“手持物品时不能进入UI”的规则
        if (inventoryController != null && inventoryController.IsHoldingItem)
        {
            Debug.LogWarning("SwitchToUI called, but action is blocked because an item is being held.");
            // (可选) 在这里播放一个“失败”或“被阻止”的音效
            // SKAudioManager.instance.PlaySound("blocked_sound");
            return; // 中断函数执行
        }

        // 2. 确定目标位置：UI区域的第一个元素通常是其左下角坐标
        Vector2Int targetPosition = uiBounds.min;

        // 检查是否已经在了，避免不必要的操作
        if (_currentContext == CursorContext.UI && _currentPosition == targetPosition)
        {
            return;
        }

        // 3. 执行上下文切换
        // 首先，离开当前的上下文（无论是Inventory还是None）
        ExitContext(_currentContext);

        // 然后，立即更新光标的内部状态
        _currentPosition = targetPosition;
        _currentContext = CursorContext.UI;

        // 接着，进入新的UI上下文
        EnterContext(_currentContext);

        // 4. 通知UI控制器光标已经移动到了具体位置，以便更新高亮等视觉效果
        NotifyMove();

        // (可选) 播放一个切换音效，使其与手动移动的感觉一致
        // SKAudioManager.instance.PlaySound("move");
    }
}