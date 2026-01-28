using UnityEngine;

namespace PhaseSystem
{
    public class ResolutionPhase : PhaseBase
    {
        // private bool player1Confirmed;
        // private bool player2Confirmed;
        private float duration;
        private float timer = 0;

        public ResolutionPhase(string name) : base(name) { }

        public override void OnEnter()
        {
            base.OnEnter();
            // 禁用玩家输入
            Player.instance.inputController.DisableAllInputs();
            // player1Confirmed = false;
            // player2Confirmed = false;
            Debug.Log("等待双方玩家确认结算结果...");
            // TO-DO: 在这里可以调用UI，显示结算界面和“等待确认”的提示
            duration = ItemSelectionUI.instance.endAnimationTime;
            Debug.Log($"等待时间{duration}");
            timer = duration;
        }

        public override void OnUpdate()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                timer = 0;
                FinishPhase();
            }
            // if (player1Confirmed && player2Confirmed)
            // {
            //     FinishPhase();
            // }
            //
            // if (!player1Confirmed && Input.GetKeyDown(KeyCode.Space))
            // {
            //     player1Confirmed = true;
            //     Debug.Log("玩家1 已确认。");
            //     // 可以在这里更新UI，比如在P1头像旁打个勾
            //     // UIManager.Instance.SetConfirmationStatus(PlayerID.Player1, true);
            // }
            //
            // if (!player2Confirmed && (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)))
            // {
            //     player2Confirmed = true;
            //     Debug.Log("玩家2 已确认。");
            //     // 更新UI
            //     // UIManager.Instance.SetConfirmationStatus(PlayerID.Player2, true);
            // }
            //
            // if (player1Confirmed && player2Confirmed)
            // {
            //     FinishPhase();
            // }
        }

        // public void PlayerConfirm(PlayerID player)
        // {
        //     if (IsFinished) return;
        //
        //     if (player == PlayerID.Player1 && !player1Confirmed)
        //     {
        //         player1Confirmed = true;
        //         Debug.Log("玩家1 已确认。");
        //     }
        //     else if (player == PlayerID.Player2 && !player2Confirmed)
        //     {
        //         player2Confirmed = true;
        //         Debug.Log("玩家2 已确认。");
        //     }
        // }
    }
}