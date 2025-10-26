using System.Collections;
using System.Collections.Generic;
using Control;
using DG.Tweening;
using NewBagSystem;
using SKCell;
using UnityEngine;

#region 枚举变量

// 游戏状态
public enum GameState
{
    None,
    MainMenu,
    Playing,
    Ending,
}
// 回合状态
public enum RoundState
{
    None,
    RoundStart,
    SelectItem,
    PlaceItem,
    // WaitConfirm,
    FixCal,
    RoundEnd,
}
// 光标状态
// public enum CursorState
// {
//     None,
//     Menu,
//     SelectItem,
//     FinishedSelect,
//     PlaceItem,
//     WaitConfirm,
//     FinishConfirm,
// }
public enum PlayerID
{
    Player1,
    Player2,
}

#endregion

public class GameManager : SKMonoSingleton<GameManager>
{
    public int TotalRound { get; private set; } = 10;
    public int CurrentRound { get; private set; } = 1;
    public RoundState CurrentRoundState { get; private set; } = RoundState.None;
    // public CursorState P1CursorState { get; private set; } = CursorState.None;
    // public CursorState P2CursorState { get; private set; } = CursorState.None;

    public List<BasicItemData> SelectResult = new(2) { null, null };
    // 合成结果
    public List<Recipe> CookResult = new(2) { null, null };
    [SerializeField] private List<SKText> ScoreText = new() { null, null };

    public void StartGame()
    {
        StartCoroutine(StartGameCoroutine());
    }

    IEnumerator StartGameCoroutine()
    {
        // 播放开始动画
        // 开启游戏循环
        StartGameLoop();
        yield return null;
    }

    public void StartGameLoop()
    {
        StartCoroutine(GameLoop());
    }
    
    IEnumerator GameLoop()
    {
        while (CurrentRound <= TotalRound)
        {
            // ====  回合开始时机  ====
            // 阶段进入操作
            RoundStartAct();
            // 播放开始回合的动画
            yield return StartRoundAni();
            // TODO:计算触发时机在回合开始的效果
            yield return StartGameEffect();
            
            // ====  轮流选择物品  ====
            // 阶段进入操作
            SelectItemAct();
            yield return selectItemControl.Flow();
            
            // ====  放置物品阶段  ====
            // 阶段进入操作
            PlaceItemAct();
            // ====  等待玩家选择“再等等”或“修复”  =====
            yield return placeItemControl.Flow();
            
            // 修复结算，在这里播放相应的动画
            // 阶段进入操作
            FixCalAct();
            yield return fixCalControl.Flow();
            
            // 回合结束时机
            // 阶段进入操作
            RoundEndAct();
            
            CurrentRound++;
        }
        yield return null;
    }

    #region 公共回调函数

    // public void SetCursorState(int playerID, CursorState state)
    // {
    //     if (playerID == 0)
    //     {
    //         P1CursorState = state;
    //     }
    //     else if (playerID == 1)
    //     {
    //         P2CursorState = state;
    //     }
    // }

    public void SetSelectItem(int playerID, BasicItemData result)
    {
        SelectResult[playerID] = result;
    }

    public void SetCookResult(int playerID, Recipe result)
    {
        CookResult[playerID] = result;
    }

    #endregion
    
    #region 私有功能函数

    private void RoundStartAct()
    {
        CurrentRoundState = RoundState.RoundStart;
        // P1CursorState = CursorState.None;
        // P2CursorState = CursorState.None;
        // 进入回合开始阶段时，禁用玩家输入
        InputManager.instance.DisableAllInputs();
    }
    
    private void SelectItemAct()
    {
        CurrentRoundState = RoundState.SelectItem;
        SetSelectItem(0, null);
        SetSelectItem(1, null);
        // 进入选择物品阶段时，恢复玩家输入
        // inputManager.EnableAllInputs();
        // 输入控制在内部修改
    }

    private void PlaceItemAct()
    {
        CurrentRoundState = RoundState.PlaceItem;
        SetCookResult(0, null);
        SetCookResult(1, null);
    }

    private void FixCalAct()
    {
        CurrentRoundState = RoundState.FixCal;
        // 禁用输入
        InputManager.instance.DisableAllInputs();
    }

    private void RoundEndAct()
    {
        CurrentRoundState = RoundState.RoundEnd;
        // 计算分数
    }

    #endregion

    #region 子系统引用

    [Header("子系统引用")]
    // [SerializeField] private InputManager inputManager;
    [SerializeField] private SelectItemControl selectItemControl;
    [SerializeField] private PlaceItemControl placeItemControl;
    [SerializeField] private FixCalControl fixCalControl;

    #endregion
    
    #region UIPanel的引用

    [SerializeField] private GameObject gameUI;

    #endregion

    #region 回合开始动画
    
    [Header("回合开始动画相关")]
    [SerializeField] private SKText roundStartText;
    [SerializeField] private Vector3 roundStartTextToSize = new Vector3(1, 1, 1);
    [SerializeField] private float roundStartTextAniDuration = 1f;
    [SerializeField] private float roundStartWaitDuration = 1f;

    IEnumerator StartRoundAni()
    {
        // 计算应当显示的文字
        int reaminRounds = TotalRound - CurrentRound;
        if (reaminRounds > 3)
        {
            roundStartText.text = "第" + CurrentRound + "回合！";
        }
        else
        {
            roundStartText.text = "剩余" + reaminRounds + "回合！";
        }
        // 显示动画
        // 关闭游戏主UI
        gameUI.SetActive(false);
        // 进入动画
        yield return roundStartText.gameObject.transform.
            DOScale(roundStartTextToSize, roundStartTextAniDuration).WaitForCompletion();
        // 等待
        yield return new WaitForSeconds(roundStartWaitDuration);
        // 退出动画
        yield return roundStartText.gameObject.transform.
            DOScale(Vector3.zero, roundStartTextAniDuration).WaitForCompletion();
        // 恢复游戏主UI
        gameUI.SetActive(true);
    }

    #endregion
    
    #region 待实现

    IEnumerator StartGameEffect()
    {
        yield return null;
    }
    
    #endregion
}