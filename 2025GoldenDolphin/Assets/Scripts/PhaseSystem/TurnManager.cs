using System;
using System.Collections;
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

        private WaitForSeconds waitForDelayFill;

        [SerializeField] private float fillSpeed = 1f;

        [SerializeField] private float fillDelay = 0.5f;

        private float t = 0f;

        private float preValue = 0;

        [SerializeField] private GameObject gameUI;
        // private int totalTurnNumber = 3;

        private void Start()
        {
            waitForDelayFill = new WaitForSeconds(fillDelay);
        }

        // 开始回合调用接口
        public void StartTurn()
        {
            Debug.Log($"=== 开始第 {currentTurnNumber + 1} 回合 ===");
            StartCoroutine(StartTurnCoroutine());
            isStarted = true;
        }

        IEnumerator StartTurnCoroutine()
        {
            TurnCountAnim.instance.ActiveAnim(TurnManager.instance.currentTurnNumber + 1);
            yield return new WaitForSeconds(5f);
            gameUI.SetActive(true);
            StartCoroutine(BufferFillingCoroutine((float)(currentTurnNumber + 1) / Constants.totalTurnNumber));
            phaseManager.StartPhases();
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
                        TurnCountAnim.instance.ActiveAnim(TurnManager.instance.currentTurnNumber);
                        phaseManager.StartPhases();
                    }
                    else
                    {
                        Debug.Log("=== 所有回合结束 ===");
                    }
                }
            }
        }

        IEnumerator BufferFillingCoroutine(float target)
        {
            yield return waitForDelayFill;
            t = 0f;
            while (t<1f)
            {
                t += Time.deltaTime * fillSpeed;
                progressSlider.value = Mathf.Lerp(preValue, target, t);
                yield return null;
            }
        }
    }
}