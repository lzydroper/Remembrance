using SKCell;
using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField] private Slider progressSlider;
        // private int totalTurnNumber = 3;

        // 开始回合调用接口
        public void StartTurn()
        {
            Debug.Log($"=== 开始第 {currentTurnNumber + 1} 回合 ===");
            progressSlider.value = (currentTurnNumber + 1) / Constants.totalTurnNumber;
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