using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PanelTransition : PersistentSinglenton<PanelTransition>
{
    public void TransmitPanel(Image panelFrom, Image panelTo)
    {
        StartCoroutine(FadeCoroutine(panelFrom, panelTo));
    }

    IEnumerator FadeCoroutine(Image panelFrom,Image panelTo)
    {
        panelTo.gameObject.SetActive(true);
        panelFrom.DOFade(0, 0.5f);
        panelTo.DOFade(1, 0.5f);
        yield return new WaitForSeconds(0.5f);
        panelFrom.gameObject.SetActive(false);
    }
}
