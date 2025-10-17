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

            // 每个阶段 2 秒自动结束
            InvokeRepeating(nameof(FinishCurrentPhase), 2f, 2f);
        }

        void Update()
        {
            turnManager.UpdateTurn();
        }

        void FinishCurrentPhase()
        {
            var phase = turnManager.GetCurrentPhase();
            if (phase != null)
            {
                Debug.Log($"自动结束阶段：{phase.Name}");
                phase.FinishPhase();
            }
        }
    }
}