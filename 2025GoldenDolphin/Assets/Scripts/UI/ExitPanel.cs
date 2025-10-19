using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SKCell;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitPanel : PersistentSinglenton<ExitPanel>
{
    [SerializeField] private SKButton ContinueButton;

    [SerializeField] private SKButton ExitButton;

    public Image panel;

    [SerializeField] private GameObject exitCanvas;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(exitCanvas);
        ContinueButton.AddListener(SKButtonEventType.OnPressed, Continue);
        ExitButton.AddListener(SKButtonEventType.OnPressed, Quit);
    }

    public void Continue()
    {
        StartCoroutine(ContinueCoroutine());
    }

    public void Open()
    {
        // StartCoroutine(OpenCoroutine());
        ContinueButton.gameObject.SetActive(true);
        ExitButton.gameObject.SetActive(true);
        ContinueButton.FadeIn();
        ExitButton.FadeIn();
    }
    public void Quit()
    {
        StartCoroutine(ExitCoroutine());
    }

    // private IEnumerator OpenCoroutine()
    // {
    //     panel.gameObject.SetActive(true);
    //     panel.DOFade(1, 0.5f);
    //     ContinueButton.gameObject.SetActive(true);
    //     ExitButton.gameObject.SetActive(true);
    //     ContinueButton.FadeIn();
    //     ExitButton.FadeIn();
    //     yield return new WaitForSeconds(0.5f);
    // }
    private IEnumerator ContinueCoroutine()
    {
        // panel.DOFade(0, 0.5f);
        ContinueButton.FadeOut();
        ExitButton.gameObject.SetActive(false);
        if (Constants.isStart == false) 
            PanelTransition.Instance.TransmitPanel(panel, MainMenu.instance.panel, true);
        else
        {
            panel.DOFade(0, 1f);
            yield return new WaitForSeconds(1f);
            panel.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(0.5f);
        ContinueButton.gameObject.SetActive(false);
        // panel.gameObject.SetActive(false);
    }
    private IEnumerator ExitCoroutine()
    {
        panel.DOFade(0, 1f);
        ExitButton.FadeOut();
        ContinueButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        Application.Quit();
    }
}
