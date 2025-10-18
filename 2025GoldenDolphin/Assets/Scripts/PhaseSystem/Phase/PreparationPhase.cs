using UnityEngine;
using UnityEngine.Events;

namespace PhaseSystem
{
    public enum PlayerID
    {
        Player1,
        Player2
    }
    
    public class PreparationPhase : PhaseBase
    {
        // private bool player1Selected;
        // private bool player2Selected;
        // private PlayerID currentPlayer;

        public PreparationPhase(string name) : base(name)
        {
            // ItemSelectionUI.instance.OnSelectionComplete += FinishPhase;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            // 禁用玩家输入
            Player.instance.inputController.DisableAllInputs();
            // player1Selected = false;
            // player2Selected = false;
            // currentPlayer = PlayerID.Player1; // 玩家1先开始
            Debug.Log("轮到 玩家1 选择道具...");
            
            if (ItemSelectionUI.instance != null)
            {
                ItemSelectionUI.instance.OnSelectionComplete += HandleSelectionComplete;
                ItemSelectionUI.instance.StartSelection();
            }
            else
            {
                Debug.LogError("找不到 ItemSelectionUI 实例！准备阶段将立即结束。");
                FinishPhase(); // 如果UI不存在，直接结束，防止游戏卡死
            }
        }

        // public override void OnUpdate()
        // {
        //     // 这个阶段的结束条件是双方都选择完毕
        //     if (player1Selected && player2Selected)
        //     {
        //         FinishPhase();
        //     }
        // }
        
        private void HandleSelectionComplete(ItemData p1Choice, ItemData p2Choice)
        {
            Debug.Log($"准备阶段收到UI选择完成信号。P1: {p1Choice.itemName}, P2: {p2Choice.itemName}");
    
            // 在这里，你可以访问 p1Choice 和 p2Choice
            // 并将它们传递给其他系统，比如玩家的背包管理器
            // PlayerInventoryManager.Instance.AddItem(PlayerID.Player1, p1Choice);
            // PlayerInventoryManager.Instance.AddItem(PlayerID.Player2, p2Choice);
            // ItemSelectionUI.instance.BagUIPanel.SetActive(true);
            Player.instance.cursorController1._currentPosition = Vector2Int.one;
            Player.instance.cursorController2._currentPosition = Vector2Int.one;
            Player.instance.inventoryController1.AddNewItemToHand(p1Choice);
            Player.instance.inventoryController2.AddNewItemToHand(p2Choice);
            FinishPhase();
        }
        
        // public void PlayerSelectItem(PlayerID player)
        // {
        //     if (IsFinished) return;
        //     if (player != currentPlayer)
        //     {
        //         Debug.LogWarning($"现在是 {currentPlayer} 的回合，{player} 无法操作！");
        //         return;
        //     }
        //
        //     if (player == PlayerID.Player1)
        //     {
        //         player1Selected = true;
        //         Debug.Log("玩家1 选择完毕。");
        //         currentPlayer = PlayerID.Player2; // 切换到玩家2
        //         Debug.Log("轮到 玩家2 选择道具...");
        //     }
        //     else if (player == PlayerID.Player2)
        //     {
        //         player2Selected = true;
        //         Debug.Log("玩家2 选择完毕。");
        //     }
        // }

        public override void OnExit()
        {
            if (ItemSelectionUI.instance != null)
            {
                ItemSelectionUI.instance.OnSelectionComplete -= HandleSelectionComplete;
            }
        }
    }
}