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
            // 启用玩家输入
            Player.instance.inputController.EnableAllInputs();
            // 激活光标
            Player.instance.ActivateCursor();
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

        public override void OnExit()
        {
            // base.OnExit();
            // 时间结束，删除拿在手里的物品，并强制合成
            ItemSelectionUI.instance.CalculateScore();
            // 关闭光标
            Player.instance.InactivateCursor();
        }
    }
}