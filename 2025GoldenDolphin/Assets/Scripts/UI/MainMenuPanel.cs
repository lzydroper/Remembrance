using System.Collections;
using DG.Tweening;
using Network;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    [Header("Main Menu Panel")] 
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private MyBtn startBtn;
    [SerializeField] private MyBtn multiplayBtn;
    [SerializeField] private MyBtn aboutBtn;
    [SerializeField] private GameObject objectsOnBook;
    [SerializeField] private Image titleImg;
    [SerializeField] private Image mainMenuPanelImg;
    [Header("About Us Panel")] 
    [SerializeField] private GameObject aboutUsPanel;
    [SerializeField] private MyBtn aboutBackBtn;
    [Header("MultiPlay Panel")] 
    [SerializeField] private GameObject multiplayPanel;
    [SerializeField] private NetworkLauncher networkLauncher;

    private void Start()
    {
        // 注册监听
        // main
        startBtn.onClick.AddListener(OnStartBtnClick);
        multiplayBtn.onClick.AddListener(OnMultiplayBtnClick);
        aboutBtn.onClick.AddListener(OnAboutBtnClick);
        // about
        aboutBackBtn.onClick.AddListener(Restart);
    }

    public void Restart()
    {
        // 重置各种Panel的active关系
        mainMenuPanel.SetActive(true);
        aboutUsPanel.SetActive(false);
        multiplayPanel.SetActive(false);
        // 重新恢复现场以保证玩家可以重新回到主菜单
        objectsOnBook.SetActive(true);
        
        startBtn.gameObject.SetActive(true);
        startBtn.img.enabled = true;
        startBtn.img.color = Color.white;
        multiplayBtn.gameObject.SetActive(true);
        multiplayBtn.img.enabled = true;
        multiplayBtn.img.color = Color.white;
        aboutBtn.gameObject.SetActive(true);
        aboutBtn.img.enabled = true;
        aboutBtn.img.color = Color.white;
        titleImg.gameObject.SetActive(true);
        titleImg.color = Color.white;
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
        multiplayBtn.img.enabled = false;
        aboutBtn.img.enabled = false;
        
        // 调用从主菜单开始游戏
        GameManager.instance.StartGame();
        yield return new WaitForSeconds(1f);
        
        startBtn.gameObject.SetActive(false);
        mainMenuPanelImg.gameObject.SetActive(false);

        yield return null;
    }

    private void OnMultiplayBtnClick()
    {
        StartCoroutine(MultiplayBtnCor());
    }
    private IEnumerator MultiplayBtnCor()
    {
        objectsOnBook.SetActive(false);

        multiplayBtn.img.DOFade(0, 1f);
        titleImg.DOFade(0, 1f);
        mainMenuPanelImg.DOFade(0, 1f);
        startBtn.img.enabled = false;
        aboutBtn.img.enabled = false;
        
        yield return new WaitForSeconds(1f);
        // 关闭mainPanel，打开multiplayPanel
        mainMenuPanel.SetActive(false);
        multiplayPanel.SetActive(true);
        // 调用Launcher的逻辑
        networkLauncher.OnMultiPlay();
        
        multiplayBtn.gameObject.SetActive(false);
        mainMenuPanelImg.gameObject.SetActive(false);

        yield return null;
    }

    private void OnAboutBtnClick()
    {
        StartCoroutine(AboutBtnCor());
    }
    private IEnumerator AboutBtnCor()
    {
        objectsOnBook.SetActive(false);

        aboutBtn.img.DOFade(0, 1f);
        titleImg.DOFade(0, 1f);
        mainMenuPanelImg.DOFade(0, 1f);
        multiplayBtn.img.enabled = false;
        startBtn.img.enabled = false;
        
        yield return new WaitForSeconds(1f);
        // 关闭mainPanel，打开aboutPanel
        mainMenuPanel.SetActive(false);
        aboutUsPanel.SetActive(true);
        
        aboutBtn.gameObject.SetActive(false);
        mainMenuPanelImg.gameObject.SetActive(false);

        yield return null;
    }
}