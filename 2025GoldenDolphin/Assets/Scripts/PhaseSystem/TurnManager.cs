using UnityEngine;

namespace PhaseSystem
{
    public class TurnManager
    {
        private PhaseManager phaseManager = new();
        public PhaseBase GetCurrentPhase() => phaseManager.currentPhase;
        public bool isFinished => currentTurnNumber >= Constants.totalTurnNumber;
        public int currentTurnNumber { get; private set; } = 0;
        // private int totalTurnNumber = 3;

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