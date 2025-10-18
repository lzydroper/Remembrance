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

    [SerializeField] private SKSlider musicSlider;

    [SerializeField] private SKSlider sfxSlider;

    [SerializeField] private AudioSource musicSource;

    [SerializeField] private AudioSource sfxSource;
    // Start is called before the first frame update
    void Start()
    {
        musicSlider.SetValueRaw(musicSource.volume);
        sfxSlider.SetValueRaw(sfxSource.volume);
        musicSlider.onValueChanged.AddListener(() =>
        {
            musicSource.volume = musicSlider.value;
        });
        sfxSlider.onValueChanged.AddListener(() =>
        {
            sfxSource.volume = sfxSlider.value;
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
            PanelTransition.Instance.TransmitPanel(panel,MainMenu.instance.panel);
        else
        {
            StartCoroutine(BackCoroutine());
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
