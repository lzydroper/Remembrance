using System.Collections.Generic;

namespace PhaseSystem
{
    public class PhaseManager
    {
        private List<PhaseBase> phases = new List<PhaseBase>();
        private int currentIndex = -1;
        public PhaseBase currentPhaseBase { get; private set; }

        public bool IsAllPhasesDone => currentIndex >= phases.Count;

        public PhaseManager()
        {
            // 注册阶段
            phases.Add(new PhaseBase("准备阶段"));
            phases.Add(new PhaseBase("行动阶段"));
            phases.Add(new PhaseBase("结算阶段"));
        }

        public void StartPhases()
        {
            currentIndex = 0;
            if (phases.Count > 0)
            {
                currentPhaseBase = phases[currentIndex];
                currentPhaseBase.OnEnter();
            }
        }

        public void Update()
        {
            if (IsAllPhasesDone) return;
            if (currentPhaseBase == null) return;

            currentPhaseBase.OnUpdate();

            if (currentPhaseBase.IsFinished)
            {
                currentPhaseBase.OnExit();
                currentIndex++;

                if (!IsAllPhasesDone)
                {
                    currentPhaseBase = phases[currentIndex];
                    currentPhaseBase.OnEnter();
                }
            }
        }
    }
}