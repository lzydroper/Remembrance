using UnityEngine;

namespace PhaseSystem
{
    public abstract class PhaseBase
    {
        public string Name { get; private set; }
        public bool IsFinished { get; protected set; }

        public PhaseBase(string name)
        {
            Name = name;
        }
        
        // OnEnter, OnUpdate, OnExit 现在是 virtual 的
        public virtual void OnEnter()
        {
            Debug.Log($"进入阶段：{Name}");
            IsFinished = false;
        }

        public virtual void OnUpdate()
        {
            // 基类中不再有具体逻辑，留给子类实现
        }

        public virtual void OnExit()
        {
            // 退出时的通用清理逻辑可以放在这里
            Debug.Log($"退出阶段：{Name}");
        }
        
        public void FinishPhase()
        {
            if (IsFinished) return; // 防止重复调用
            IsFinished = true;
        }
    }
}