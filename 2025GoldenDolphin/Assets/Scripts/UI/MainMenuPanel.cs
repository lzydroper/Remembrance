using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    [SerializeField] private MyBtn startBtn;
    [SerializeField] private MyBtn pauseBtn;
    [SerializeField] private MyBtn aboutBtn;
    [SerializeField] private GameObject objectsOnBook;
    [SerializeField] private Image titleImg;
    [SerializeField] private Image mainMenuPanelImg;

    private void Start()
    {
        // 注册监听
        startBtn.onClick.AddListener(OnStartBtnClick);
        pauseBtn.onClick.AddListener(OnPauseBtnClick);
        aboutBtn.onClick.AddListener(OnAboutBtnClick);
    }

    public void Restart()
    {
        // 重新恢复现场以保证玩家可以重新回到主菜单
        objectsOnBook.SetActive(true);
        
        startBtn.gameObject.SetActive(true);
        pauseBtn.gameObject.SetActive(true);
        aboutBtn.gameObject.SetActive(true);
        titleImg.gameObject.SetActive(true);
        mainMenuPanelImg.gameObject.SetActive(true);
    }

    private void OnStartBtnClick()
    {
        StartCoroutine(StartBtnCor());
    }
    private IEnumerator StartBtnCor()
    {
        objectsOnBook.SetActive(false);

        startBtn.img.DOFade(0, 1f);
        titleImg.DOFade(0, 1f);
        mainMenuPanelImg.DOFade(0, 1f);
        pauseBtn.gameObject.SetActive(false);
        aboutBtn.gameObject.SetActive(false);
        
        // 调用从主菜单开始游戏
        GameManager.instance.StartGame();
        yield return new WaitForSeconds(1f);
        
        startBtn.gameObject.SetActive(false);
        mainMenuPanelImg.gameObject.SetActive(false);

        yield return null;
    }

    private void OnPauseBtnClick()
    {
        
    }

    private void OnAboutBtnClick()
    {
        
    }
}