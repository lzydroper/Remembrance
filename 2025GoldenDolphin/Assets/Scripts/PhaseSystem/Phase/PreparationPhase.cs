using UnityEngine;

namespace PhaseSystem
{
    public enum PlayerID
    {
        Player1,
        Player2
    }
    
    public class PreparationPhase : PhaseBase
    {
        private bool player1Selected;
        private bool player2Selected;
        private PlayerID currentPlayer;

        public PreparationPhase(string name) : base(name) { }

        public override void OnEnter()
        {
            base.OnEnter();
            player1Selected = false;
            player2Selected = false;
            currentPlayer = PlayerID.Player1; // 玩家1先开始
            Debug.Log("轮到 玩家1 选择道具...");
            
            // TODO: 调用显示选择物品的界面
            // 紧接着禁用玩家2的光标，将玩家1的光标显示出来
        }

        public override void OnUpdate()
        {
            // 这个阶段的结束条件是双方都选择完毕
            if (player1Selected && player2Selected)
            {
                FinishPhase();
            }
        }
        
        public void PlayerSelectItem(PlayerID player)
        {
            if (IsFinished) return;
            if (player != currentPlayer)
            {
                Debug.LogWarning($"现在是 {currentPlayer} 的回合，{player} 无法操作！");
                return;
            }

            if (player == PlayerID.Player1)
            {
                player1Selected = true;
                Debug.Log("玩家1 选择完毕。");
                currentPlayer = PlayerID.Player2; // 切换到玩家2
                Debug.Log("轮到 玩家2 选择道具...");
                // TODO：启用玩家2的光标，禁用玩家1的
            }
            else if (player == PlayerID.Player2)
            {
                player2Selected = true;
                Debug.Log("玩家2 选择完毕。");
            }
        }

        public override void OnExit()
        {
            // TODO: 移动光标至背包中，生成选择的物品到手上
        }
    }
}