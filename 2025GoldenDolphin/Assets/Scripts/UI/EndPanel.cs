using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EndPanel : MonoBehaviour
{
    [SerializeField] private MyBtn restartBtn;
    [SerializeField] private MyBtn mainMenuBtn;
    // 不管了，屎山就屎山把
    [SerializeField] private MainMenuPanel mainMenuPanel;
    
    private void Start()
    {
        // 注册监听
        restartBtn.onClick.AddListener(OnRestartBtnClick);
        mainMenuBtn.onClick.AddListener(OnMainMenuBtnClick);
    }

    private void OnMainMenuBtnClick()
    {
        // 关闭自己，直接调用MainMenu的Restart
        GameManager.instance.CloseAllPanel();
        mainMenuPanel.Restart();
        gameObject.SetActive(false);
    }

    private void OnRestartBtnClick()
    {
        // 关闭自己，直接调用StartGameLoop
        GameManager.instance.CloseAllPanel();
        GameManager.instance.StartGame();
        gameObject.SetActive(false);
    }
}