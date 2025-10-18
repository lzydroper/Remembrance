using System;
using System.Collections;
using System.Collections.Generic;
using SKCell;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player : SKMonoSingleton<Player>
{
    [SerializeField] public PlayerInputController inputController;

    [Header("玩家1模块")]
    [SerializeField] public InventoryController inventoryController1;
    [SerializeField] public PlayerCursorController cursorController1;

    [Header("玩家2模块")]
    [SerializeField] public InventoryController inventoryController2;
    [SerializeField] public PlayerCursorController cursorController2;
    
    // 你可以在这里添加其他模块，比如游戏暂停菜单
    // [SerializeField] private PauseMenu pauseMenu;

    #region Life-cycle

    private void OnEnable()
    {
        // --- 公共输入 ---
        inputController.onPause += Pause;
        
        // --- 玩家1 输入绑定 ---
        inputController.onMoveUp1    += () => cursorController1.Move(Vector2Int.up);
        inputController.onMoveDown1  += () => cursorController1.Move(Vector2Int.down);
        inputController.onMoveLeft1  += () => cursorController1.Move(Vector2Int.left);
        inputController.onMoveRight1 += () => cursorController1.Move(Vector2Int.right);
        inputController.onConfirm1   += cursorController1.Confirm;
        inputController.onRotate1    += cursorController1.Rotate;

        // --- 玩家2 输入绑定 ---
        inputController.onMoveUp2    += () => cursorController2.Move(Vector2Int.up);
        inputController.onMoveDown2  += () => cursorController2.Move(Vector2Int.down);
        inputController.onMoveLeft2  += () => cursorController2.Move(Vector2Int.left);
        inputController.onMoveRight2 += () => cursorController2.Move(Vector2Int.right);
        inputController.onConfirm2   += cursorController2.Confirm;
        inputController.onRotate2    += cursorController2.Rotate;
    }

    private void OnDisable()
    {
        // 在 OnEnable 中使用了匿名函数，严格来说需要存储引用才能准确解绑。
        // 但对于这种贯穿游戏生命周期的核心控制器，通常不解绑也可以，因为它会和 inputController 同时被销毁。
        // 如果确实需要频繁启/禁用此脚本，则需要将 lambda 表达式改为常规方法以便解绑。
        // 为了简洁，此处省略解绑代码。
    }

    void Start()
    {
        inputController.EnableGameplay1Input();
        inputController.EnableGameplay2Input();
        // 假设你有一个方法可以为玩家生成初始物品
        // Test_GiveInitialItems();
    }

    #endregion

    #region Functions

    void Pause()
    {
        Debug.Log("游戏暂停/恢复");
        // pauseMenu.Toggle();
    }

    // --- 以下是用于测试的示例函数 ---
    public ItemData testItemData1; // 在Inspector中拖入一个1x3的ItemData
    public ItemData testItemData2; // 在Inspector中拖入一个2x2的ItemData
    public GameObject itemPrefab;  // 一个包含InventoryItem脚本的基础物品预制体

    public void Test_GiveInitialItems()
    {
        // 给玩家1一个1x3的物品
        if (testItemData1 != null && itemPrefab != null)
        {
            GameObject itemObj1 = Instantiate(itemPrefab);
            InventoryItem invItem1 = itemObj1.GetComponent<InventoryItem>();
            invItem1.itemData = testItemData1;
            inventoryController1.AddNewItemToHand(invItem1, Vector2Int.one);
            Debug.Log("give item to p1");
        }
        
        // 给玩家2一个2x2的物品
        if (testItemData2 != null && itemPrefab != null)
        {
            GameObject itemObj2 = Instantiate(itemPrefab);
            InventoryItem invItem2 = itemObj2.GetComponent<InventoryItem>();
            invItem2.itemData = testItemData2;
            inventoryController2.AddNewItemToHand(invItem2, Vector2Int.one);
            Debug.Log("give item to p2");
        }
    }
    
    #endregion
}