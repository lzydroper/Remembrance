using UnityEngine;

namespace PhaseSystem
{
    public class TurnEndPhase : PhaseBase
    {
        private float duration;
        private float timer;

        public TurnEndPhase(string name, float duration) : base(name)
        {
            this.duration = duration;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            timer = duration;
            Debug.Log($"回合结束，进入过渡阶段，持续 {duration} 秒。");
        }

        public override void OnUpdate()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                FinishPhase();
            }
        }
    }
}