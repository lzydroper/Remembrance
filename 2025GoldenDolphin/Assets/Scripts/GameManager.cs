using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Control;
using DG.Tweening;
using NewBagSystem;
using SKCell;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
// 管理从按下游戏开始到游戏完全结束，的整个流程
public class GameManager : SKMonoSingleton<GameManager>
{
    public int TotalRound { get; private set; } = 10;
    public int CurrentRound { get; private set; } = 1;
    // public RoundState CurrentRoundState { get; private set; } = RoundState.None;
    // public CursorState P1CursorState { get; private set; } = CursorState.None;
    // public CursorState P2CursorState { get; private set; } = CursorState.None;

    public List<BasicItemData> SelectResult = new(2) { null, null };
    // 合成结果
    public List<Recipe> CookResult = new(2) { null, null };

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

    [ContextMenu("StartGameLoop")]
    public void StartGameLoop()
    {
        StartCoroutine(GameLoop());
    }
    
    IEnumerator GameLoop()
    {
        // 游戏开始初始化
        GameStaAct();
        // 从主菜单按下开始游戏按钮到主界面的动画
        yield return GameStaAni();
        while (CurrentRound <= TotalRound)
        {
            // ====  回合开始时机  ====
            // 阶段进入操作
            RoundStartAct();
            // 播放开始回合的动画
            yield return StartRoundAni();
            // TODO:计算触发时机在回合开始的效果
            yield return RoundStaEffect();
            
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
            // TODO:触发回合结束的效果
            yield return RoundEndEffect();
            // 回合结束的动画
            yield return EndRoundAni();
            
            CurrentRound++;
        }

        GameEndAct();
        // 游戏结束，播放根据最后分数，播放结束动画
        yield return GameEndAni();
        // TODO:播放完以后跳出再来一局、回主界面
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

    private void GameStaAct()
    {
        // 初始化分数文字为0
        foreach (var text in scoreText)
        {
            if (text) text.text = "0";
        }
        // 关闭endPanel
        endPanel.SetActive(false);
    }

    private void GameEndAct()
    {
        // 开启endPanel
        endPanel.SetActive(true);
    }

    private void RoundStartAct()
    {
        // CurrentRoundState = RoundState.RoundStart;
        // P1CursorState = CursorState.None;
        // P2CursorState = CursorState.None;
        // 进入回合开始阶段时，禁用玩家输入
        InputManager.instance.DisableAllInputs();
    }
    
    private void SelectItemAct()
    {
        // CurrentRoundState = RoundState.SelectItem;
        SetSelectItem(0, null);
        SetSelectItem(1, null);
        // 进入选择物品阶段时，恢复玩家输入
        // inputManager.EnableAllInputs();
        // 输入控制在内部修改
    }

    private void PlaceItemAct()
    {
        // CurrentRoundState = RoundState.PlaceItem;
        SetCookResult(0, null);
        SetCookResult(1, null);
    }

    private void FixCalAct()
    {
        // CurrentRoundState = RoundState.FixCal;
        // 禁用输入
        InputManager.instance.DisableAllInputs();
        // 关闭UI
        gameUI.SetActive(false);
    }

    private void RoundEndAct()
    {
        // CurrentRoundState = RoundState.RoundEnd;
        // 开启UI
        gameUI.SetActive(true);
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
    [SerializeField] private GameObject endPanel;

    #endregion

    #region 动画
    
    [Header("回合开始动画相关")]
    [SerializeField] private SKText roundStartText;
    [SerializeField] private Vector3 roundStartTextToSize = new Vector3(1, 1, 1);
    [SerializeField] private float roundStartTextAniDuration = 1f;
    [SerializeField] private float roundStartWaitDuration = 1f;

    IEnumerator StartRoundAni()
    {
        // 计算应当显示的文字
        int remainRounds = TotalRound - CurrentRound;
        if (remainRounds > 3)
        {
            roundStartText.text = "第" + CurrentRound + "回合！";
        }
        else
        {
            roundStartText.text = "剩余" + remainRounds + "回合！";
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
    
    [Header("回合结束动画相关")]
    [SerializeField] private List<SKText> scoreText = new() { null, null };
    [SerializeField] private List<int> scores = new() { 0, 0 };
    [SerializeField] private float scoreChangAniDuration = 1.5f;
    IEnumerator EndRoundAni()
    {
        // 目前只有分数变化动画和进度条变化动画
        // 进度条动画
        StartCoroutine(ProgressCoroutine());    // 有点屎的代码调用，Orz
        float elapsedTime = 0f;
        int[] scoreDiffs = { 0, 0 };
        if (CookResult[0])
        {
            scoreDiffs[0] = (CookResult[0].type == RecipeType.Good) ? CookResult[0].score : -CookResult[0].score;
        }
        if (CookResult[1])
        {
            scoreDiffs[1] = (CookResult[1].type == RecipeType.Bad) ? CookResult[1].score : -CookResult[1].score;
        }
        // 如果没有分数变化，直接设置最终值并退出
        if (scoreDiffs.All(num => num == 0))
        {
            yield break;
        }

        int[] currentDisplayScore = { 0, 0 };
        while (elapsedTime < scoreChangAniDuration)
        {
            // 计算当前时间进度
            float progress = elapsedTime / scoreChangAniDuration;
        
            // 根据进度计算当前应该达到的分数
            currentDisplayScore[0] = scores[0] + (int)(scoreDiffs[0] * progress);
            currentDisplayScore[1] = scores[1] + (int)(scoreDiffs[1] * progress);

            // 更新UI文本
            scoreText[0].text = currentDisplayScore[0].ToString();
            scoreText[1].text = currentDisplayScore[0].ToString();
        
            // 等待下一帧
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 保存分数更改
        // 动画结束后，确保显示的是最终的精确分数
        for (int i = 0; i < 2; i++)
        {
            scores[i] += scoreDiffs[i];
            scoreText[i].text = scores[i].ToString();
        }
    }

    [Header("进度条相关")] 
    [SerializeField] private float fillSpeed = 1f;
    [SerializeField] private float progressSliderValue;
    [SerializeField] private Slider progressSlider;
    IEnumerator ProgressCoroutine()
    {
        float target = (float)CurrentRound / TotalRound;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * fillSpeed;
            progressSlider.value = Mathf.Lerp(progressSliderValue, target, t);
            progressSliderValue = target;
            yield return null;
        }
    }

    [Header("游戏结束动画相关")] 
    [SerializeField] private PlayableDirector[] endAniDirectors;
    IEnumerator GameEndAni()
    {
        PlayableDirector director;
        if (scores[0] > scores[1])
        {
            director = endAniDirectors[0];
        }
        else if (scores[0] < scores[1])
        {
            director = endAniDirectors[1];
        }
        else
        {
            director = endAniDirectors[2];
        }
        director.Play();
        yield return new WaitForSeconds((float)director.duration);
        yield return null;
    }

    [Header("游戏开始动画相关")]
    [SerializeField] private GameObject gameCamera;
    [SerializeField] private PlayableDirector staAniDirector;
    IEnumerator GameStaAni()
    {
        PlayableDirector director = staAniDirector;
        director.Play();
        yield return new WaitForSeconds((float)director.duration);
        gameCamera.SetActive(true);
        yield return null;
    }

    #endregion
    
    #region 待实现

    IEnumerator RoundStaEffect()
    {
        yield return null;
    }

    IEnumerator RoundEndEffect()
    {
        yield return null;
    }
    
    #endregion
}