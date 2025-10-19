using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SKCell;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePanel : PersistentSinglenton<PausePanel>
{
    [SerializeField] private GameObject canvas;
    public Image panel;
    [SerializeField] private SKButton backButton;

    [SerializeField] private Slider musicSlider;

    [SerializeField] private Slider sfxSlider;
    
    // Start is called before the first frame update
    void Start()
    {
        musicSlider.value = Constants.musicValue;
        sfxSlider.value = Constants.sfxValue;
        musicSlider.onValueChanged.AddListener((float value)=>
        {
            Constants.musicValue = value;
        });
        sfxSlider.onValueChanged.AddListener((float value) =>
        {
            Constants.sfxValue = value;
        });
        backButton.AddListener(SKButtonEventType.OnPressed,Back);
        DontDestroyOnLoad(canvas);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Enactive()
    {
        // StartCoroutine(ActiveCoroutine());
        backButton.FadeIn();
    }
    public void Back()
    {
        // StartCoroutine(BackCoroutine());
        backButton.FadeOut();
        if (Constants.isStart == false)
            PanelTransition.Instance.TransmitPanel(panel, MainMenu.instance.panel, true);
        else
        {
            StartCoroutine(BackCoroutine());
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    IEnumerator BackCoroutine()
    {
        panel.DOFade(0, 0.5f);
        backButton.FadeOut();
        musicSlider.gameObject.SetActive(false);
        sfxSlider.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        backButton.gameObject.SetActive(false);
        panel.gameObject.SetActive(false);
    }
    // IEnumerator ActiveCoroutine()
    // {
    //     panel.gameObject.SetActive(true);
    //     panel.DOFade(1, 0.5f);
    //     backButton.FadeIn();
    //     yield return new WaitForSeconds(0.5f);
    // }
}
