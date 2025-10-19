using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SKCell;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UI;

public class MainMenu : SKMonoSingleton<MainMenu>
{
    [SerializeField] private SKButton startGameButton;

    [SerializeField] private SKButton pauseButton;

    [SerializeField] private SKButton aboutButton;
    [SerializeField] private SKImage title;
    [SerializeField] private PlayableDirector director;
    
    [SerializeField] private GameObject cameraMachine1;
    [SerializeField] private GameObject cameraMachine2;

    public Image panel;
    // Start is called before the first frame update
    void Start()
    {
        startGameButton.AddListener(SKButtonEventType.OnPressed, StartGame);
        pauseButton.AddListener(SKButtonEventType.OnPressed, Pause);
        aboutButton.AddListener(SKButtonEventType.OnPressed, About);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Quit();
        }
    }

    void StartGame()
    {
        // SceneLoader.Instance.Load("Game");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(StartGameCoroutine());
        director.Play();
        Constants.isStart = true;
        cameraMachine2.SetActive(true);
    }

    void Pause()
    {
        PanelTransition.Instance.TransmitPanel(panel, PausePanel.Instance.panel);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PausePanel.Instance.Enactive();
    }

    void About()
    {
        
    }

    void Quit()
    {
        ExitPanel.Instance.Open();
        PanelTransition.Instance.TransmitPanel(panel,ExitPanel.Instance.panel);
    }

    IEnumerator StartGameCoroutine()
    {
        startGameButton.FadeOut();
        title.DOFade(0, 1f);
        pauseButton.gameObject.SetActive(false);
        aboutButton.gameObject.SetActive(false);
        panel.DOFade(0, 1f);
        SKAudioManager.instance.PlaySound("startGame");
        yield return new WaitForSeconds(1f);
        startGameButton.gameObject.SetActive(false);
        panel.gameObject.SetActive(false);
    }
}
