using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using BagSystem;
using SKCell;

namespace PhaseSystem
{
    public class ItemSelectionUI : SKMonoSingleton<ItemSelectionUI>
    {
        [Header("UI组件引用")]
        [SerializeField] private GameObject selectionPanel; // 整个选择界面的父对象
        [SerializeField] private ItemDisplay[] itemDisplays = new ItemDisplay[4]; // 4个物品显示槽
        [SerializeField] private GameObject player1Cursor; // 玩家1的光标
        [SerializeField] private GameObject player2Cursor; // 玩家2的光标

        [SerializeField] private Itemdb itemdb;
        private int player1SelectedIndex = -1; 
        private List<ItemData> currentRandomItems;
        private ItemData player1Choice;
        private ItemData player2Choice;
        
        private PlayerID currentPlayer;
        private int currentIndex;
        private bool isSelectionActive = false;

        // 提供一个事件，在选择全部完成后通知外部系统
        // public event System.Action OnSelectionComplete;
        public event System.Action<ItemData, ItemData> OnSelectionComplete;

        protected override void Awake()
        {
            base.Awake();
            selectionPanel.SetActive(false); // 默认隐藏
            // BagUIPanel.SetActive(false);
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
            
            // 3. 重置状态并由玩家1开始
            player1Choice = null;
            player2Choice = null;
            currentIndex = 0;
            player1SelectedIndex = -1; // 重置P1选择索引
            SwitchPlayer(PlayerID.Player1);
            
            isSelectionActive = true;
            selectionPanel.SetActive(true);
        }

        private void HandleInput()
        {
            // 根据当前玩家处理不同的输入
            if (currentPlayer == PlayerID.Player1)
            {
                if (Input.GetKeyDown(KeyCode.A)) MoveCursor(-1);
                if (Input.GetKeyDown(KeyCode.D)) MoveCursor(1);
                if (Input.GetKeyDown(KeyCode.Space)) ConfirmSelection();
            }
            else // PlayerID.Player2
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveCursor(-1);
                if (Input.GetKeyDown(KeyCode.RightArrow)) MoveCursor(1);
                if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) ConfirmSelection();
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
            while (currentPlayer == PlayerID.Player2 && currentIndex == player1SelectedIndex);
        
            UpdateCursorPosition();
        }

        private void ConfirmSelection()
        {
            // 再次检查，防止玩家2在某种边缘情况下选中了已被占用的项
            if (currentPlayer == PlayerID.Player2 && currentIndex == player1SelectedIndex)
            {
                Debug.Log("这个物品已经被玩家1选择了！");
                return;
            }
            ItemData selectedItem = currentRandomItems[currentIndex];
            
            if (currentPlayer == PlayerID.Player1)
            {
                player1Choice = selectedItem;
                player1SelectedIndex = currentIndex; // 记录P1的选择
                Debug.Log($"玩家1 选择了: {player1Choice.itemName}");
            
                // 将被选中的物品UI设置为不可用状态
                itemDisplays[player1SelectedIndex].SetAsUnavailable();
                
                // 切换到玩家2
                SwitchPlayer(PlayerID.Player2);
            }
            else // PlayerID.Player2
            {
                player2Choice = selectedItem;
                Debug.Log($"玩家2 选择了: {player2Choice.itemName}");
                
                // 双方都选择完毕，结束流程
                EndSelection();
            }
        }

        private void SwitchPlayer(PlayerID newPlayer)
        {
            currentPlayer = newPlayer;
        
            if (currentPlayer == PlayerID.Player1)
            {
                currentIndex = 0; // P1开始时总是从第一个开始
            }
            else // 切换到P2时
            {
                // 为P2找到一个不是P1已选项的初始位置
                currentIndex = 0;
                if (currentIndex == player1SelectedIndex)
                {
                    // 如果第一个就是被选的，自动跳到下一个
                    MoveCursor(1); 
                }
            }
        
            UpdateCursorPosition();
        }

        private void UpdateCursorPosition()
        {
            GameObject activeCursor = (currentPlayer == PlayerID.Player1) ? player1Cursor : player2Cursor;
            GameObject inactiveCursor = (currentPlayer == PlayerID.Player1) ? player2Cursor : player1Cursor;

            activeCursor.SetActive(true);
            inactiveCursor.SetActive(false);

            // 将光标移动到当前选中物品的位置
            activeCursor.transform.position = itemDisplays[currentIndex].transform.position;
        }

        private void EndSelection()
        {
            isSelectionActive = false;
            selectionPanel.SetActive(false);
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
            ItemData p2Result = Player.instance.inventoryController2.RemoveHeldItem();
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
                Debug.Log("p1 create item");
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
                Debug.Log("p2 create item");
            }
            StartCoroutine(PlayEndAnimation(p1Long, p2Long, p1Result, p2Result));
        }

        public IEnumerator PlayEndAnimation(bool p1Long, bool p2Long, ItemData p1Result, ItemData p2Result)
        {
            yield return new WaitForEndOfFrame();
            Debug.Log("慢拔out!");
        }
    }
}