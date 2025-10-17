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
            player1Confirmed = false;
            player2Confirmed = false;
            Debug.Log("等待双方玩家确认结算结果...");
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