using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using BagSystem;
using DG.Tweening;
using SKCell;
using UnityEngine.InputSystem;

namespace PhaseSystem
{
    public class ItemSelectionUI : SKMonoSingleton<ItemSelectionUI>
    {
        [Header("UI组件引用")]
        [SerializeField] private GameObject selectionPanel; // 整个选择界面的父对象
        [SerializeField] private ItemDisplay[] itemDisplays = new ItemDisplay[4]; // 4个物品显示槽
        // [SerializeField] private GameObject player1Cursor; // 玩家1的光标
        // [SerializeField] private GameObject player2Cursor; // 玩家2的光标

        [SerializeField] private Itemdb itemdb;
        private int firstPlayerSelectedIndex = -1; 
        private List<ItemData> currentRandomItems;
        private ItemData player1Choice;
        private ItemData player2Choice;
        
        private PlayerID currentPlayer;
        // 新增变量来记录本回合的先后手顺序
        private PlayerID firstPlayer;
        private PlayerID secondPlayer;
        private int currentIndex;
        private bool isSelectionActive = false;

        private WaitForSeconds waitForLongAnim;
        private WaitForSeconds waitForShortAnim;

        [SerializeField] private Transform objectGenerateTransform;
        // 提供一个事件，在选择全部完成后通知外部系统
        // public event System.Action OnSelectionComplete;
        public event System.Action<ItemData, ItemData> OnSelectionComplete;
        
        // 各种按钮状态
        public bool p1Finish = false;
        public bool p2Finish = false;
        private void BtnClicked(PlayerID player)
        {
            if (player == PlayerID.Player1)
            {
                p1Finish = true;
            }
            if (player == PlayerID.Player2)
            {
                p2Finish = true;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            selectionPanel.SetActive(false); // 默认隐藏
            Player.instance.uiController1.BtnUIPanel.SetActive(false);
            Player.instance.uiController2.BtnUIPanel.SetActive(false);
            // 开始订阅结束事件
            Player.instance.uiController1.OnBtnClicked += () => { BtnClicked(PlayerID.Player1); };
            Player.instance.uiController2.OnBtnClicked += () => { BtnClicked(PlayerID.Player2); };
            // BagUIPanel.SetActive(false);

            waitForLongAnim = new WaitForSeconds(Constants.longAnimTime);
            waitForShortAnim = new WaitForSeconds(Constants.shortAnimTime);
        }

        void Update()
        {
            if (!isSelectionActive) return;

            HandleInput();
        }

        /// <summary>
        /// 外部调用此函数以开始选择流程
        /// </summary>
        public void StartSelection()
        {
            // 1. 随机选取四个不重复的物品
            if (itemdb.items.Count < 4)
            {
                Debug.LogError("物品总数少于4个，无法开始选择！");
                return;
            }
            currentRandomItems = itemdb.items.OrderBy(x => Random.value).Take(4).ToList();

            // 2. 设置UI显示
            for (int i = 0; i < 4; i++)
            {
                itemDisplays[i].Setup(currentRandomItems[i]);
            }
            
            // // 3. 重置状态并由玩家1开始
            // player1Choice = null;
            // player2Choice = null;
            // currentIndex = 0;
            // firstPlayerSelectedIndex = -1; // 重置上个玩家
            // SwitchPlayer(PlayerID.Player1);
            //
            // isSelectionActive = true;
            // selectionPanel.SetActive(true);
            if (Constants.turn) // true = P1 优先
            {
                firstPlayer = PlayerID.Player1;
                secondPlayer = PlayerID.Player2;
            }
            else // false = P2 优先
            {
                firstPlayer = PlayerID.Player2;
                secondPlayer = PlayerID.Player1;
            }
            Constants.turn = !Constants.turn;
            Debug.Log($"本回合由 <color=cyan>{firstPlayer}</color> 优先选择。");
            
            isSelectionActive = true;
            selectionPanel.SetActive(true);
            Player.instance.uiController1.BtnUIPanel.SetActive(false);
            Player.instance.uiController2.BtnUIPanel.SetActive(false);
            // 重置
            Player.instance.uiController1.ResetBtn();
            Player.instance.uiController2.ResetBtn();
            p1Finish = false;
            p2Finish = false;
            
            // 使用决定好的先手玩家开始
            SwitchPlayer(firstPlayer);
            StartCoroutine(InitializeSelectionRoutine());
            endAnimationTime = 0f;
        }
        
        private IEnumerator InitializeSelectionRoutine()
        {
            // 等待下一帧。到下一帧开始时，所有UI布局肯定已经计算完毕。
            // yield return new WaitForEndOfFrame(); // 这个更精确，但 yield return null; 通常也足够了
            yield return null; 

            // 现在，itemDisplays 的 transform.position 已经是正确的值了
            // 在这里调用 SwitchPlayer 来设置第一个玩家的光标位置
            SwitchPlayer(firstPlayer);
        }

        private void HandleInput()
        {
            // 根据当前玩家处理不同的输入
            if (currentPlayer == PlayerID.Player1)
            {
                if (Keyboard.current.aKey.wasPressedThisFrame) MoveCursor(-1);
                if (Keyboard.current.dKey.wasPressedThisFrame) MoveCursor(1);
                if (Keyboard.current.spaceKey.wasPressedThisFrame) ConfirmSelection();
            }
            else // PlayerID.Player2
            {
                if (Keyboard.current.leftArrowKey.wasPressedThisFrame) MoveCursor(-1);
                if (Keyboard.current.rightArrowKey.wasPressedThisFrame) MoveCursor(1);
                if (Keyboard.current.numpadEnterKey.wasPressedThisFrame) ConfirmSelection();
            }
        }

        private void MoveCursor(int direction)
        {
            int initialIndex = currentIndex;
        
            do
            {
                currentIndex += direction;

                // 循环移动
                if (currentIndex < 0) currentIndex = itemDisplays.Length - 1;
                if (currentIndex >= itemDisplays.Length) currentIndex = 0;
            
                // 如果移动了一圈又回到了原点，说明没有其他可选的项，直接跳出循环
                if (currentIndex == initialIndex) break;

            } 
            // 核心逻辑: 如果当前是玩家2，并且光标想移动到的位置是P1已经选了的位置，就继续移动
            while (currentPlayer == secondPlayer && currentIndex == firstPlayerSelectedIndex);
        
            // UpdateCursorPosition();
            // 为当前开高光，取消之前的高光
            itemDisplays[initialIndex].iconImage.sprite = currentRandomItems[initialIndex].selectUISprite;
            itemDisplays[initialIndex].transform.DOScale(Vector3.one, scaleTime);
            itemDisplays[currentIndex].iconImage.sprite = currentRandomItems[currentIndex].selectUISpriteHighlighted;
            itemDisplays[currentIndex].transform.DOScale(selectScale, scaleTime);
        }

        private void ConfirmSelection()
        {
            // // 再次检查，防止玩家2在某种边缘情况下选中了已被占用的项
            // if (currentPlayer == PlayerID.Player2 && currentIndex == firstPlayerSelectedIndex)
            // {
            //     Debug.Log("这个物品已经被玩家1选择了！");
            //     return;
            // }
            // ItemData selectedItem = currentRandomItems[currentIndex];
            //
            // if (currentPlayer == PlayerID.Player1)
            // {
            //     player1Choice = selectedItem;
            //     firstPlayerSelectedIndex = currentIndex; // 记录P1的选择
            //     Debug.Log($"玩家1 选择了: {player1Choice.itemName}");
            //
            //     // 将被选中的物品UI设置为不可用状态
            //     itemDisplays[firstPlayerSelectedIndex].SetAsUnavailable();
            //     
            //     // 切换到玩家2
            //     SwitchPlayer(PlayerID.Player2);
            // }
            // else // PlayerID.Player2
            // {
            //     player2Choice = selectedItem;
            //     Debug.Log($"玩家2 选择了: {player2Choice.itemName}");
            //     
            //     // 双方都选择完毕，结束流程
            //     EndSelection();
            // }
            if (currentPlayer == secondPlayer && currentIndex == firstPlayerSelectedIndex)
            {
                Debug.Log($"这个物品已经被 {firstPlayer} 选择了！");
                return;
            }
            
            ItemData selectedItem = currentRandomItems[currentIndex];
            
            // --- 修改点 5: 使用 firstPlayer/secondPlayer 逻辑重构 ---
            if (currentPlayer == firstPlayer)
            {
                // 先手玩家进行选择
                if (firstPlayer == PlayerID.Player1) player1Choice = selectedItem;
                else player2Choice = selectedItem;

                firstPlayerSelectedIndex = currentIndex; // 记录先手玩家的选择
                Debug.Log($"{firstPlayer} (先手) 选择了: {selectedItem.itemName}");
            
                itemDisplays[currentIndex].transform.DOScale(Vector3.one, scaleTime);
                itemDisplays[firstPlayerSelectedIndex].SetAsUnavailable();
                
                // 切换到后手玩家
                SwitchPlayer(secondPlayer);
            }
            else // 当前玩家是 secondPlayer
            {
                // 后手玩家进行选择
                if (secondPlayer == PlayerID.Player1) player1Choice = selectedItem;
                else player2Choice = selectedItem;

                Debug.Log($"{secondPlayer} (后手) 选择了: {selectedItem.itemName}");
                
                itemDisplays[currentIndex].transform.DOScale(Vector3.one, scaleTime);
                EndSelection();
            }

            GameObject newItem = GameObject.Instantiate(selectedItem.object3D, objectGenerateTransform);
            newItem.transform.position = objectGenerateTransform.transform.position;
        }

        private void SwitchPlayer(PlayerID newPlayer)
        {
            currentPlayer = newPlayer;
        
            // --- 修改点 6: 确保后手玩家初始位置正确 ---
            if (currentPlayer == secondPlayer)
            {
                // 为后手玩家找到一个不是先手玩家已选项的初始位置
                currentIndex = 0;
                if (currentIndex == firstPlayerSelectedIndex)
                {
                    MoveCursor(1); // 如果第一个就是被选的，自动跳到下一个
                }
            }
            else // currentPlayer == firstPlayer
            {
                currentIndex = 0; // 先手玩家总是从第一个开始
            }
            // if (currentPlayer == PlayerID.Player1)
            // {
            //     currentIndex = 0; // P1开始时总是从第一个开始
            // }
            // else // 切换到P2时
            // {
            //     // 为P2找到一个不是P1已选项的初始位置
            //     currentIndex = 0;
            //     if (currentIndex == firstPlayerSelectedIndex)
            //     {
            //         // 如果第一个就是被选的，自动跳到下一个
            //         MoveCursor(1); 
            //     }
            // }
            // 高光选中
            
            itemDisplays[currentIndex].iconImage.sprite = currentRandomItems[currentIndex].selectUISpriteHighlighted;
            itemDisplays[currentIndex].transform.DOScale(selectScale, scaleTime);
        }
        
        private Vector3 selectScale =  new Vector3(1.5f, 1.5f, 1f);
        private float scaleTime = 0.5f;
        // private void UpdateCursorPosition()
        // {
        //     GameObject activeCursor = (currentPlayer == PlayerID.Player1) ? player1Cursor : player2Cursor;
        //     GameObject inactiveCursor = (currentPlayer == PlayerID.Player1) ? player2Cursor : player1Cursor;
        //
        //     activeCursor.SetActive(true);
        //     inactiveCursor.SetActive(false);
        //
        //     // 将光标移动到当前选中物品的位置
        //     // Debug.Log($"global({itemDisplays[currentIndex].transform.localPosition})");
        //     // activeCursor.transform.SetParent(itemDisplays[currentIndex].transform);
        //     activeCursor.transform.position = itemDisplays[currentIndex].transform.position;
        //     // Debug.Log($"local({activeCursor.transform.position})");
        // }

        private void EndSelection()
        {
            isSelectionActive = false;
            selectionPanel.SetActive(false);
            Player.instance.uiController1.BtnUIPanel.SetActive(true);
            Player.instance.uiController2.BtnUIPanel.SetActive(true);
            Debug.Log("所有玩家选择完毕！");
            // 触发事件，通知PreparationPhase阶段可以结束了
            OnSelectionComplete?.Invoke(player1Choice, player2Choice);
        }

        public int player1Score = 0;
        public int player2Score = 0;

        public bool needPlayEndAnimation { get; private set; } = false;
        public float endAnimationTime { get; private set; } = 0f;
        public void resetEndAnimationTIme() => endAnimationTime = 0;

        // 由actionPhase调用，结算最终分数
        public void CalculateScore()
        {
            ItemData p1Result = Player.instance.inventoryController1.RemoveHeldItem();
            if (p1Result == null && p1Finish)
            {
                p1Result = Player.instance.uiController1.thinkBtn ? null : Player.instance.inventoryController1.CookIt();
            }
            ItemData p2Result = Player.instance.inventoryController2.RemoveHeldItem();
            if (p2Result == null && p2Finish)
            {
                p2Result = Player.instance.uiController2.thinkBtn ? null : Player.instance.inventoryController2.CookIt();
            }
            if (p1Result == null && p2Result == null)
            {
                endAnimationTime = 0f;
                Debug.Log("p1 and p2 create nothing");
                return;
            }
            bool p1Long = false, p2Long = false;
            if (p1Result != null)
            {
                if (itemdb.locklists.Add(p1Result))
                {
                    p1Long = true;
                    endAnimationTime += Constants.endAniLongTime;
                }
                else
                {
                    endAnimationTime += Constants.endAniShortTime;
                }
                Debug.Log($"p1 create item :{p1Result.itemName}");
            }
            if (p2Result != null)
            {
                if (itemdb.locklists.Add(p2Result))
                {
                    p2Long = true;
                    endAnimationTime += Constants.endAniLongTime;
                }
                else
                {
                    endAnimationTime += Constants.endAniShortTime;
                }
                Debug.Log($"p2 create item :{p2Result.itemName}");
            }
            StartCoroutine(PlayEndAnimation(p1Long, p2Long, p1Result, p2Result));
            // 好人加分，坏人减分
            int p1AddScore = 0;
            if (p1Result != null)
            {
                p1AddScore = p1Result.isGood ? p1Result.score : -p1Result.score;
            }
            int p2AddScore = 0;
            if (p2Result != null)
            {
                p2AddScore = p2Result.isGood ? -p2Result.score : p2Result.score;
            }
            StartCoroutine(AnimateScoreCounting(p1ScoreText, p1CurScore, p1AddScore, duration));
            StartCoroutine(AnimateScoreCounting(p2ScoreText, p2CurScore, p2AddScore, duration));
            p1CurScore += p1AddScore;
            p2CurScore += p2AddScore;
            Constants.p1Score = p1CurScore;
            Constants.p2Score = p2CurScore;
        }

        public IEnumerator PlayEndAnimation(bool p1Long, bool p2Long, ItemData p1Result, ItemData p2Result)
        {
            TurnManager.instance.gameUI.SetActive(false);
            if (p1Result != null)
            {
                DirectorManager.instance.PlayDirector(p1Long,p1Result);
                yield return p1Long ? waitForLongAnim : waitForShortAnim;
            }

            if (p2Result != null)
            {
                DirectorManager.instance.PlayDirector(p2Long,p2Result);
                yield return p2Long ? waitForLongAnim : waitForShortAnim;
            }
            yield return new WaitForEndOfFrame();
            TurnManager.instance.gameUI.SetActive(true);
            // Debug.Log("慢拔out!");
        }
        
        
        // 得分动画部分
        private int p1CurScore = 0;
        private int p2CurScore = 0;
        [SerializeField] private SKText p1ScoreText;
        [SerializeField] private SKText p2ScoreText;
        [SerializeField] private float duration = 1.5f;
        private IEnumerator AnimateScoreCounting(SKText scoreText, int startScore, int addedScore, float duration)
        {
            float elapsedTime = 0f;
            int scoreDifference = addedScore;

            // 如果没有分数变化，直接设置最终值并退出
            if (scoreDifference == 0)
            {
                yield break;
            }

            while (elapsedTime < duration)
            {
                // 计算当前时间进度
                float progress = elapsedTime / duration;
        
                // 根据进度计算当前应该达到的分数
                int currentDisplayScore = startScore + (int)(scoreDifference * progress);

                // 更新UI文本
                scoreText.text = currentDisplayScore.ToString();
        
                // 等待下一帧
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 动画结束后，确保显示的是最终的精确分数
            scoreText.text = (startScore + addedScore).ToString();
        }
    }
}