using System;
using SKCell;
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
        Vector2Int newPosition = _currentPosition + direction;

        // 根据新位置判断新的上下文
        CursorContext newContext = DetermineContext(newPosition);

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
                // TODO: 调用你的UI控制器的确认方法
                // Vector2Int localUIPos = _currentPosition - uiBounds.min;
                // uiController.OnConfirmAction(localUIPos);
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
        if (uiBounds.Contains(position)) return CursorContext.UI;
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
                // TODO: 调用你的UI控制器的 OnCursorEnter 方法
                // uiController.OnCursorEnter();
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
                // TODO: 调用你的UI控制器的 OnCursorExit 方法
                // uiController.OnCursorExit();
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
                 // TODO: 调用你的UI控制器的 OnCursorMove 方法
                Vector2Int localUIPos = _currentPosition - uiBounds.min;
                // uiController.OnCursorMove(localUIPos);
                break;
        }
    }
}