using UnityEngine;
using UnityEngine.Events;

namespace PhaseSystem
{
    public class PhaseBase
    {
        public string Name { get; private set; }
        public bool IsFinished { get; private set; }
        public UnityEvent phaseEvent = new();

        public PhaseBase(string name)
        {
            Name = name;
        }
        
        public void OnEnter()
        {
            Debug.Log($"进入阶段：{Name}");
            IsFinished = false;
        }

        public void OnUpdate()
        {
            phaseEvent?.Invoke();
        }

        public void OnExit()
        {
            phaseEvent?.RemoveAllListeners();
        }
        
        public void FinishPhase()
        {
            Debug.Log($"退出阶段：{Name}");
            IsFinished = true;
        }
    }
}