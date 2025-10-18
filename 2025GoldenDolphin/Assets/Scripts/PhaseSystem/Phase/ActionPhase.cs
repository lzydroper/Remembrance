using UnityEngine;

namespace PhaseSystem
{
    public class ActionPhase : PhaseBase
    {
        private float duration;
        private float timer;

        public ActionPhase(string name, float duration) : base(name)
        {
            this.duration = duration;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            timer = duration;
            Debug.Log($"行动阶段开始，持续 {duration} 秒。");
            // ItemSelectionUI.instance.BagUIPanel.SetActive(true);
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