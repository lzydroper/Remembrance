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

        public void FinishCurrentPhase()
        {
            if (!turnManager.isFinished)
            {
                var phase = turnManager.GetCurrentPhase();
                if (phase != null)
                {
                    Debug.Log($"自动结束第{turnManager.currentTurnNumber}回合阶段：{phase.Name}");
                    phase.FinishPhase();
                }
            }
        }
    }
}