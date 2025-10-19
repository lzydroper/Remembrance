using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SKCell;
using UnityEngine;

public class TurnCountAnim : SKMonoSingleton<TurnCountAnim>
{
    [SerializeField] private SKText turnText;
    [SerializeField] private Vector3 transformToSize = new Vector3(1, 1, 1);

    private WaitForSeconds waitForFade;

    private void Start()
    {
        waitForFade = new WaitForSeconds(3f);
    }

    public void ActiveAnim(int turnCount)
    {
        if (Constants.totalTurnNumber - turnCount > 3) 
            turnText.text = "第" + turnCount + "回合！";
        else
        {
            turnText.text = "剩余" + (Constants.totalTurnNumber - turnCount) + "回合！";
        }
        StartCoroutine(turnCoroutine());
    }

    IEnumerator turnCoroutine()
    {
        turnText.gameObject.transform.DOScale(transformToSize, 1f);
        yield return waitForFade;
        turnText.gameObject.transform.DOScale(Vector3.zero, 1f);
    }
}
