using UnityEngine;

namespace PhaseSystem
{
    public class TurnManager
    {
        private PhaseManager phaseManager = new PhaseManager();
        public PhaseBase GetCurrentPhase() => phaseManager.currentPhaseBase;
        public bool isFinished => currentTurnNumber >= totalTurnNumber;
        private int currentTurnNumber = 0;
        private int totalTurnNumber = 3;

        public void StartTurn()
        {
            Debug.Log($"=== 开始第 {currentTurnNumber + 1} 回合 ===");
            phaseManager.StartPhases();
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