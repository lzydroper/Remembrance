using System.Collections.Generic;

namespace PhaseSystem
{
    public class PhaseManager
    {
        // 使用List来保证顺序，用Dictionary来快速查找
        private Dictionary<string, PhaseBase> phases = new();
        private List<PhaseBase> phaseOrder = new();
        private int currentIndex = -1;
        public PhaseBase currentPhase { get; private set; }

        public bool IsAllPhasesDone => currentIndex >= phaseOrder.Count;

        public PhaseManager()
        {
            // 注册具体的阶段实例
            // AddPhase(new EmptyPhase("起始空阶段"));      // 后续流程内通过finish这个阶段来实现进入准备阶段
            AddPhase(new TimerPhase("起始动画阶段", Constants.startPhaseTime));
            AddPhase(new PreparationPhase("准备阶段"));
            AddPhase(new ActionPhase("行动阶段", Constants.actionPhaseTime));
            AddPhase(new ResolutionPhase("结算阶段"));
            // AddPhase(new EmptyPhase("结束空阶段"));      // 后续流程内通过finish这个阶段来实现
            // AddPhase(new TurnEndPhase("回合结束阶段", 2.0f)); // 假设过渡持续2秒
        }

        private void AddPhase(PhaseBase phase)
        {
            phaseOrder.Add(phase);
            phases.Add(phase.Name, phase);
        }

        public PhaseBase GetPhase(string phaseName)
        {
            phases.TryGetValue(phaseName, out var phase);
            return phase;
        }

        public void StartPhases()
        {
            currentIndex = 0;
            if (phaseOrder.Count > 0)
            {
                currentPhase = phaseOrder[currentIndex];
                currentPhase.OnEnter();
            }
        }

        public void Update()
        {
            if (IsAllPhasesDone || currentPhase == null) return;

            currentPhase.OnUpdate();

            if (currentPhase.IsFinished)
            {
                currentPhase.OnExit();
                currentIndex++;

                if (!IsAllPhasesDone)
                {
                    currentPhase = phaseOrder[currentIndex];
                    currentPhase.OnEnter();
                }
            }
        }
    }
}