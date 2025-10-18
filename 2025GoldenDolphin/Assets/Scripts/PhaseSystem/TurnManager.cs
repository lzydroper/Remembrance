using SKCell;
using UnityEngine;

namespace PhaseSystem
{
    public class TurnManager : SKMonoSingleton<TurnManager>
    {
        private PhaseManager phaseManager = new();
        public PhaseBase GetCurrentPhase() => phaseManager.currentPhase;
        public PhaseBase GetPhase(string phaseName) => phaseManager.GetPhase(phaseName);
        public bool isFinished => currentTurnNumber >= Constants.totalTurnNumber;
        public int currentTurnNumber { get; private set; } = 0;
        public bool isStarted = false;
        // private int totalTurnNumber = 3;

        // 开始回合调用接口
        public void StartTurn()
        {
            Debug.Log($"=== 开始第 {currentTurnNumber + 1} 回合 ===");
            phaseManager.StartPhases();
            isStarted = true;
        }

        void Update()
        {
            if (isStarted && !isFinished)
            {
                UpdateTurn();
            }
        }

        public void UpdateTurn()
        {
            if (!isFinished)
            {
                phaseManager.Update();

                if (phaseManager.IsAllPhasesDone)
                {
                    currentTurnNumber++;
                    if (!isFinished)
                    {
                        phaseManager.StartPhases();
                    }
                    else
                    {
                        Debug.Log("=== 所有回合结束 ===");
                    }
                }
            }
        }
    }
}