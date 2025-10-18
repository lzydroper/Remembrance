using UnityEngine;

namespace PhaseSystem
{
    public class ResolutionPhase : PhaseBase
    {
        private bool player1Confirmed;
        private bool player2Confirmed;

        public ResolutionPhase(string name) : base(name) { }

        public override void OnEnter()
        {
            base.OnEnter();
            // 禁用玩家输入
            Player.instance.inputController.DisableAllInputs();
            player1Confirmed = false;
            player2Confirmed = false;
            Debug.Log("等待双方玩家确认结算结果...");
            // TODO: 在这里可以调用UI，显示结算界面和“等待确认”的提示
        }

        public override void OnUpdate()
        {
            if (player1Confirmed && player2Confirmed)
            {
                FinishPhase();
            }
        }

        public void PlayerConfirm(PlayerID player)
        {
            if (IsFinished) return;

            if (player == PlayerID.Player1 && !player1Confirmed)
            {
                player1Confirmed = true;
                Debug.Log("玩家1 已确认。");
            }
            else if (player == PlayerID.Player2 && !player2Confirmed)
            {
                player2Confirmed = true;
                Debug.Log("玩家2 已确认。");
            }
        }
    }
}