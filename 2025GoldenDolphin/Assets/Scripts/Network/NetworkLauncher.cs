using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine;

namespace Network
{
    /// <summary>
    /// 控制整个连接过程吧大概
    /// 连接pun2服务器
    /// </summary>
    public class NetworkLauncher : MonoBehaviourPunCallbacks
    {
        #region 引用组件

        [Header("UI Panels")]
        [SerializeField] private GameObject loadingPanel;   // 显示“连接中...”
        [SerializeField] private GameObject lobbyPanel;     // 大厅界面（显示房间列表，创建按钮）
        [SerializeField] private GameObject roomPanel;      // 房间等待界面（显示玩家列表，开始按钮）
        [SerializeField] private MainMenuPanel mainMenuPanel;
        
        [Header("Loading UI")]
        [SerializeField] private LoadingStatusMsg loadingStatusMsg;
        [SerializeField] private LoadingCircleAnime loadingCircleAnime;
        
        [Header("Lobby UI")]
        [SerializeField] private MyBtn backBtn;
        
        // [Header("Room UI")]

        #endregion

        private void Start()
        {
            // 初始化面板状态
            loadingPanel.SetActive(false);
            lobbyPanel.SetActive(false);
            roomPanel.SetActive(false);
            
            // 注册按钮回调
            backBtn.onClick.AddListener(OnBackBtn);
            
            // PhotonNetwork.AutomaticallySyncScene = true; 
        }

        public void OnBackBtn()
        {
            // 不应该这么写的，但是屎山堆出来了
            // 断开连接，调用main的Restart
            PhotonNetwork.Disconnect();
            mainMenuPanel.Restart();
        }

        public void OnMultiPlay()
        {
            // 主菜单点击多人游戏时
            // 显示相关UI
            loadingPanel.SetActive(true);
            lobbyPanel.SetActive(false);
            roomPanel.SetActive(false);
            // 设置UI信息
            loadingCircleAnime.StartAni();
            loadingStatusMsg.SetLoginStatusMsg("正在连接服务器...");
            // 连接pun2服务器
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                // 如果已经连上了，直接进大厅
                StartCoroutine(ConnectServerCor());
            }
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN2服务器连接成功！");
            StartCoroutine(ConnectServerCor());
        }
        private IEnumerator ConnectServerCor()
        {
            // 连接成功后，修改提示文字
            loadingStatusMsg.SetLoginStatusMsg("正在进入服务器大厅...");
            yield return null;
            // 进入大厅
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("加入大厅成功！");
            StartCoroutine(JoinLobbyCor());
        }
        private IEnumerator JoinLobbyCor()
        {
            // 没什么意义的延迟后关闭加载界面
            yield return new WaitForSeconds(1f);
            loadingCircleAnime.StopAni();
            loadingPanel.SetActive(false);
            // 显示大厅界面
            lobbyPanel.SetActive(true);
            yield return null;
        }
    }
}