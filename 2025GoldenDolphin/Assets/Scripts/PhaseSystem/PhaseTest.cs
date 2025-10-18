using UnityEngine;

namespace PhaseSystem
{
    public class TurnTest : MonoBehaviour
    {
        private TurnManager turnManager;

        void Start()
        {
            turnManager = new TurnManager();
            turnManager.StartTurn();
        }

        void Update()
        {
            turnManager.UpdateTurn();
        }

        // --- 用于模拟UI按钮点击的公共方法 ---

        public void Test()
        {
            turnManager.GetCurrentPhase().FinishPhase();
        }

        // 模拟玩家1在“准备阶段”选择道具
        public void OnPlayer1SelectItem()
        {
            // 获取当前阶段并尝试转换为 PreparationPhase 类型
            if (turnManager.GetCurrentPhase() is PreparationPhase prepPhase)
            {
                prepPhase.PlayerSelectItem(PlayerID.Player1);
            }
        }

        // 模拟玩家2在“准备阶段”选择道具
        public void OnPlayer2SelectItem()
        {
            if (turnManager.GetCurrentPhase() is PreparationPhase prepPhase)
            {
                prepPhase.PlayerSelectItem(PlayerID.Player2);
            }
        }

        // 模拟玩家1在“结算阶段”确认
        public void OnPlayer1Confirm()
        {
            if (turnManager.GetCurrentPhase() is ResolutionPhase resPhase)
            {
                resPhase.PlayerConfirm(PlayerID.Player1);
            }
        }

        // 模拟玩家2在“结算阶段”确认
        public void OnPlayer2Confirm()
        {
            if (turnManager.GetCurrentPhase() is ResolutionPhase resPhase)
            {
                resPhase.PlayerConfirm(PlayerID.Player2);
            }
        }
    }
}