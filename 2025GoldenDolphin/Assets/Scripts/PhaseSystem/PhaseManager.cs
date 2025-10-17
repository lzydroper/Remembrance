using System.Collections.Generic;

namespace PhaseSystem
{
    public class PhaseManager
    {
        private Dictionary<string, PhaseBase> phases = new();
        private List<string> phaseNames = new();
        private int currentIndex = -1;
        public PhaseBase currentPhase { get; private set; }

        public bool IsAllPhasesDone => currentIndex >= phaseNames.Count;

        public PhaseManager()
        {
            // 注册阶段
            AddPhase("准备阶段");
            AddPhase("行动阶段");
            AddPhase("结算阶段");
        }

        private void AddPhase(string phaseName)
        {
            phaseNames.Add(phaseName);
            phases.Add(phaseName, new PhaseBase(phaseName));
        }

        public PhaseBase GetPhase(string phaseName)
        {
            return phases[phaseName];
        }

        public void StartPhases()
        {
            currentIndex = 0;
            if (phases.Count > 0)
            {
                currentPhase = phases[phaseNames[currentIndex]];
                currentPhase.OnEnter();
            }
        }

        public void Update()
        {
            if (IsAllPhasesDone) return;
            if (currentPhase == null) return;

            currentPhase.OnUpdate();

            if (currentPhase.IsFinished)
            {
                currentPhase.OnExit();
                currentIndex++;

                if (!IsAllPhasesDone)
                {
                    currentPhase = phases[phaseNames[currentIndex]];
                    currentPhase.OnEnter();
                }
            }
        }
    }
}