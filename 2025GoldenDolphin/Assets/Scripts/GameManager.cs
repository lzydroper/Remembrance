using System.Collections;
using DG.Tweening;
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
    WaitConfirm,
    FixCal,
    RoundEnd,
}
// 光标状态
public enum CursorState
{
    None,
    Menu,
    SelectItem,
    FinishedSelect,
    PlaceItem,
    WaitConfirm,
    FinishConfirm,
}
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
    public CursorState P1CursorState { get; private set; } = CursorState.None;
    public CursorState P2CursorState { get; private set; } = CursorState.None;
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
            
            // 放置物品阶段
            
            // 等待玩家选择“再等等”或“修复”
            
            // 修复结算，在这里播放相应的动画
            
            // 回合结束时机
            
            CurrentRound++;
        }
        yield return null;
    }
    
    #region 私有功能函数

    private void RoundStartAct()
    {
        CurrentRoundState = RoundState.RoundStart;
        // 进入回合开始阶段时，禁用玩家输入
        inputManager.DisableAllInputs();
    }
    
    private void SelectItemAct()
    {
        CurrentRoundState = RoundState.SelectItem;
        // 进入选择物品阶段时，恢复玩家输入
        inputManager.EnableAllInputs();
    }

    #endregion

    #region 子系统引用

    [Header("子系统引用")]
    [SerializeField] private InputManager inputManager;

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