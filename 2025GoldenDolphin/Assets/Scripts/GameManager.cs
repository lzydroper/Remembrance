using System.Collections;
using System.Collections.Generic;
using Control;
using DG.Tweening;
using NewBagSystem;
using Photon.Pun;
using SKCell;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

#region 枚举变量

// 游戏状态
// public enum GameState
// {
//     None,
//     MainMenu,
//     Playing,
//     Ending,
// }
// 回合状态
// public enum RoundState
// {
//     None,
//     RoundStart,
//     SelectItem,
//     PlaceItem,
//     // WaitConfirm,
//     FixCal,
//     RoundEnd,
// }
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
// public enum PlayerID
// {
//     Player1,
//     Player2,
// }

#endregion
// 管理从按下游戏开始到游戏完全结束，的整个流程
public class GameManager : SKMonoSingleton<GameManager>
{
    private bool _gameState;
    
    [SerializeField] private int totalRound = 10;
    [SerializeField] private int currentRound = 1;
    // public RoundState CurrentRoundState { get; private set; } = RoundState.None;
    // public CursorState P1CursorState { get; private set; } = CursorState.None;
    // public CursorState P2CursorState { get; private set; } = CursorState.None;

    [SerializeField] private List<int> scores = new() { 0, 0 };
    public List<BasicItemData> SelectResult = new(2) { null, null };
    // 合成结果
    public List<Recipe> CookResult = new(2) { null, null };
    
    // 主菜单开始游戏走这里，和结束界面再来一把走这里
    [ContextMenu("StartGame")]
    public void StartGame()
    {
        StartCoroutine(StartGameCoroutine());
    }
    IEnumerator StartGameCoroutine()
    {
        // 设置游戏状态为开始
        _gameState = true;
        // 播放开始动画
        // 从主菜单按下开始游戏按钮到主界面的动画
        yield return GameStaAni();
        // 开启游戏循环
        StartGameLoop();
        yield return null;
    }
    
    private void StartGameLoop()
    {
        StartCoroutine(GameLoop());
    }
    IEnumerator GameLoop()
    {
        // 游戏开始初始化
        GameStaAct();
        while (currentRound <= totalRound)
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
            
            currentRound++;
        }

        GameEndAct();
        // 游戏结束，播放根据最后分数，播放结束动画
        yield return GameEndAni();
        yield return null;
        // 播放完以后跳出再来一局、回主界面
        GameFinishAct();
    }

    public void CloseAllPanel()
    {
        gameUI.SetActive(false);
        endAniPanel.SetActive(false);
        endPanel.SetActive(false);
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
        // 选中物品后掉落对应实体的功能放到Select
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
        // 初始化分数为0
        for (int i = 0; i < scores.Count; i++)
        {
            scores[i] = 0;
        }
        // 初始化已选择和结果
        for (int i = 0; i < SelectResult.Count; i++)
        {
            SelectResult[i] = null;
        }

        for (int i = 0; i < CookResult.Count; i++)
        {
            CookResult[i] = null;
        }
        // 初始化回合数为1
        currentRound = 1;
        // 关闭endPanel
        endAniPanel.SetActive(false);
        // 重置slider
        progressSlider.value = 0;
        // 清空背包
        placeItemControl.Restart();
        // 重置选项顺序，清空场景中的物体
        selectItemControl.Restart();
    }

    private void GameEndAct()
    {
        // 开启endPanel
        endAniPanel.SetActive(true);
        // 关闭gameUI
        gameUI.SetActive(false);
    }

    private void RoundStartAct()
    {
        // CurrentRoundState = RoundState.RoundStart;
        // P1CursorState = CursorState.None;
        // P2CursorState = CursorState.None;
        // 进入回合开始阶段时，禁用玩家输入
        InputManager.instance.DisableAllInputs();
        SetSelectItem(0, null);
        SetSelectItem(1, null);
        SetCookResult(0, null);
        SetCookResult(1, null);
        roundStartText.transform.localScale = Vector3.zero;
    }
    
    private void SelectItemAct()
    {
        // CurrentRoundState = RoundState.SelectItem;
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
        
    }

    private void GameFinishAct()
    {
        endPanel?.SetActive(true);
        InputManager.instance.EnableAllInputs();
        gameCamera.SetActive(false);
        mainMenuCamera.SetActive(true);
        
        _gameState = false;
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

    [Header("UIPanel的引用")]
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject endAniPanel;
    [Tooltip("问你要不要再来一把的panel")]
    [SerializeField] private GameObject endPanel;       // 问你要不要再来一把的panel

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
        int remainRounds = totalRound - currentRound + 1;
        if (remainRounds > 3)
        {
            roundStartText.text = "第" + currentRound + "回合！";
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
        // if (scoreDiffs.All(num => num == 0))
        // {
        //     yield break;
        // }

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
            scoreText[1].text = currentDisplayScore[1].ToString();
        
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
    // [SerializeField] private float progressSliderValue;
    [SerializeField] private Slider progressSlider;
    IEnumerator ProgressCoroutine()
    {
        float target = (float)currentRound / totalRound;
        float t = 0f;
        float startValue = progressSlider.value;
        while (t < 1f) {
            t += Time.deltaTime * fillSpeed;
            progressSlider.value = Mathf.Lerp(startValue, target, t);
            yield return null;
        }
        // progressSliderValue = progressSlider.value;
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
    [SerializeField] private GameObject mainMenuCamera;
    [SerializeField] private GameObject gameCamera;
    [SerializeField] private PlayableDirector staAniDirector;
    IEnumerator GameStaAni()
    {
        // 重置摄像机
        gameCamera.SetActive(false);
        mainMenuCamera.SetActive(true);
        PlayableDirector director = staAniDirector;
        director.Play();
        gameCamera.SetActive(true);
        yield return new WaitForSeconds((float)director.duration);
        yield return null;
    }

    #endregion

    #region 多人游戏

    [Header("多人游戏相关")]
    [SerializeField] private MainMenuPanel mainMenuPanel;
    [SerializeField] public PhotonView photonView;
    private static bool isMultiPlaying = false;
    // 有是否是主机来判断，主机一定是玩家1
    // private static bool isPlayer1 = false;

    public bool GetIsMultiPlaying() => isMultiPlaying;
    // public bool GetIsPlayer1() => isPlayer1;
    
    public void SetMultiPlay(bool value = true)
    {
        isMultiPlaying = value;
    }
    // public void SetPlayer1(bool value = true)
    // {
    //     isPlayer1 = value;
    // }

    public void MultiPlayError()
    {
        // 出现意外，立刻停止游戏进度，退出至主菜单界面
        // 检测游戏状态
        if (isMultiPlaying && _gameState)
        {
            StopAllCoroutines();
            InputManager.instance.EnableAllInputs();
            gameCamera.SetActive(false);
            mainMenuCamera.SetActive(true);
            isMultiPlaying = false;
            _gameState = false;
            CloseAllPanel();
            mainMenuPanel.Restart();
            PhotonNetwork.Disconnect();
        }
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