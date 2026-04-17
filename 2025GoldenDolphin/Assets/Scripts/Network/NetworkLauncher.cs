using System;
using System.Collections;
using System.Collections.Generic;
using NewBagSystem;
using Photon.Realtime;
using Photon.Pun;
using SKCell;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

namespace Network
{
    /// <summary>
    /// 控制整个连接过程直到正式进入游戏吧大概
    /// </summary>
    public class NetworkLauncher : MonoBehaviourPunCallbacks
    {
        #region 引用组件

        [Header("UI Panels")]
        [SerializeField] private GameObject multiPlayPanel;
        [SerializeField] private GameObject loadingPanel;   // 显示“连接中...”
        [SerializeField] private GameObject lobbyPanel;     // 大厅界面（显示房间列表，创建按钮）
        [SerializeField] private GameObject roomPanel;      // 房间等待界面（显示玩家列表，开始按钮）
        [SerializeField] private MainMenuPanel mainMenuPanel;
        
        [Header("Loading UI")]
        [SerializeField] private LoadingStatusMsg loadingStatusMsg;
        [SerializeField] private LoadingCircleAnime loadingCircleAnime;
        
        [Header("Lobby UI")]
        [SerializeField] private MyBtn backBtn;
        [SerializeField] private MyBtn createBtn;
        [SerializeField] private MyBtn refreshBtn;
        [SerializeField] private Transform roomListContent;
        [SerializeField] private GameObject roomItemPrefab;
        [SerializeField] private MyBtnNavigation lobbyNavi;
        
        [Header("Room UI")] 
        [SerializeField] private GameObject roomBtnPanel;
        [SerializeField] private MyBtn backToLobbyBtn;
        [SerializeField] private MyBtn startGameBtn;
        [SerializeField] private Transform playerListContent;
        [SerializeField] private GameObject playerItemPrefab;

        [SerializeField] private SKImage roomIcon;
        [SerializeField] private SKText roomNameText;

        #endregion

        private TypedLobby _lobby;
        
        private void Start()
        {
            // 初始化面板状态
            loadingPanel.SetActive(false);
            lobbyPanel.SetActive(false);
            roomPanel.SetActive(false);
            
            // 注册按钮回调
            backBtn.onClick.AddListener(OnBackBtn);
            createBtn.onClick.AddListener(OnCreateRoomBtn);
            refreshBtn.onClick.AddListener(OnRefreshBtn);
            
            backToLobbyBtn.onClick.AddListener(OnBackToLobbyBtn);
            startGameBtn.onClick.AddListener(OnStartGameBtn);
            
            // PhotonNetwork.AutomaticallySyncScene = true; 
            PhotonNetwork.GameVersion = "1.0";
        }

        private void OnRefreshBtn()
        {
            loadingCircleAnime.StartAni();
            loadingStatusMsg.SetLoginStatusMsg("正在刷新房间列表...");
            loadingPanel.SetActive(true);

            PhotonNetwork.GetCustomRoomList(_lobby, "1");
        }

        private void OnCreateRoomBtn()
        {
            loadingCircleAnime.StartAni();
            loadingStatusMsg.SetLoginStatusMsg("正在创建房间...");
            loadingPanel.SetActive(true);
            
            CreateRoom();
        }

        private void OnBackBtn()
        {
            // 不应该这么写的，但是屎山堆出来了
            // 断开连接，调用main的Restart
            PhotonNetwork.Disconnect();
            mainMenuPanel.Restart();
        }

        private void OnBackToLobbyBtn()
        {
            // 关闭房间界面UI
            roomPanel.SetActive(false);
            // 过度场景加载
            loadingCircleAnime.StartAni();
            loadingStatusMsg.SetLoginStatusMsg("正在返回大厅...");
            loadingPanel.SetActive(true);
            // 返回大厅
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                // 兜底：若不在房间，直接走返回大厅逻辑
                StartCoroutine(ReturnToLobbyCor());
            }
        }
        private IEnumerator ReturnToLobbyCor()
        {
            // 安全校验：确保网络状态正常
            yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady);
            yield return new WaitUntil(() => PhotonNetwork.NetworkClientState != ClientState.Leaving);

            // 重新加入大厅（复用原有大厅配置）
            if (!PhotonNetwork.InLobby)
            {
                loadingStatusMsg.SetLoginStatusMsg("正在重新进入大厅...");
                PhotonNetwork.JoinLobby(_lobby);
            }
            else
            {
                // 若已在大厅，直接刷新房间列表+显示大厅UI
                OnRefreshBtn(); // 复用刷新逻辑，保证列表最新
            }
        }

        private void OnStartGameBtn()
        {
            if (CheckRoomStatus())
            {
                // 关闭按钮Panel防止乱点
                roomBtnPanel.SetActive(false);
                // 显示加载信息
                loadingCircleAnime.StartAni();
                loadingStatusMsg.SetLoginStatusMsg("正在进入游戏...");
                loadingPanel.SetActive(true);
                // rpc同步通知所有人该开始游戏了
                if (PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC(nameof(RpcStartGame), RpcTarget.All);
                }
            }
        }

        [PunRPC]
        public void RpcStartGame()
        {
            // 关闭界面
            multiPlayPanel.SetActive(false);
            // 开启多人游戏模式
            GameManager.instance.SetMultiPlay(true);
            
            GameManager.instance.StartGame();
            if (loadingCircleAnime.isActiveAndEnabled)
            {
                loadingCircleAnime.StopAni();
            }
        }

        public void OnMultiPlay()
        {
            // 主菜单点击多人游戏时
            // 显示相关UI
            loadingPanel.SetActive(true);
            lobbyPanel.SetActive(false);
            roomPanel.SetActive(false);
            backBtn.gameObject.SetActive(true);
            refreshBtn.gameObject.SetActive(true);
            createBtn.gameObject.SetActive(true);
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
            loadingPanel.SetActive(true);
            loadingCircleAnime.StartAni();
            loadingStatusMsg.SetLoginStatusMsg("正在进入服务器大厅...");
            yield return null;
            yield return new WaitUntil(() => PhotonNetwork.NetworkClientState != ClientState.Leaving);
            yield return null;
            yield return null;
            
            // 设置特殊头像
            string randomItemId = ItemManager.instance.GetRandomItemId();
            Hashtable playerProps = new Hashtable()
            { 
                { "ITEM_ID", randomItemId }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProps);

            // 进入大厅
            _lobby = new TypedLobby("Remeberance", LobbyType.SqlLobby);
            PhotonNetwork.JoinLobby(_lobby);
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

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            loadingCircleAnime.StopAni();
            loadingPanel.SetActive(false);
            
            // 清理旧列表UI
            foreach (Transform child in roomListContent)
            {
                Destroy(child.gameObject);
            }
            
            // 生成新按钮
            foreach (RoomInfo room in roomList)
            {
                if (room.RemovedFromList || !room.IsOpen || !room.IsVisible)
                    continue;
                
                GameObject roomItemGO = Instantiate(roomItemPrefab, roomListContent);
                MyRoomItemBtn roomItem = roomItemGO.GetComponent<MyRoomItemBtn>();
                roomItem.Setup(room);
                roomItem.onClick.AddListener(() => OnMyRoomItemBtn(roomItem._info));
            }
            
            // 刷新界面
            if (lobbyPanel.activeInHierarchy)
            {
                StartCoroutine(RefreshRoomListCor());
            }
            
        }
        private IEnumerator RefreshRoomListCor()
        {
            // 只有进入大厅后才能刷新
            if (!PhotonNetwork.InLobby)
            {
                yield return new WaitUntil(() => PhotonNetwork.InLobby);
                yield return new WaitForSeconds(1.5f);
            }
            yield return new WaitForEndOfFrame();
            
            lobbyNavi.RefreshButtonList();
            
            yield return null;
        }

        public void OnMyRoomItemBtn(RoomInfo info)
        {
            if (info != null && info.PlayerCount < info.MaxPlayers)
            {
                OnJoiningRoom();
                
                PhotonNetwork.JoinRoom(info.Name);
            }
        }

        private void OnJoiningRoom()
        {
            // 正在加入房间中的过渡显示
            // 打开loadingPanel
            loadingCircleAnime.StartAni();
            loadingStatusMsg.SetLoginStatusMsg("正在加入房间...");
            loadingPanel.SetActive(true);
            // 关闭lobbyPanel
            lobbyPanel.SetActive(false);
        }

        private void CreateRoom()
        {
            OnJoiningRoom();
            
            string randomItemId = ItemManager.instance.GetRandomItemId();
                
            string roomName = "Room#" + Random.Range(1, 10000).ToString("D4") + 
                              "_" + randomItemId;
            RoomOptions options = new RoomOptions
            {
                MaxPlayers = 2,
                PublishUserId = true
            };
            options.CustomRoomProperties = new Hashtable() 
            { 
                { "ITEM_ID", randomItemId }
            };
            options.CustomRoomPropertiesForLobby = new string[] { "ITEM_ID" };
            PhotonNetwork.CreateRoom(roomName, options, _lobby);
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            Debug.Log("成功创建房间！");

            if (loadingCircleAnime.isActiveAndEnabled)
            {
                loadingCircleAnime.StopAni();
            }
            loadingPanel.SetActive(false);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogWarning($"房间创建失败: {message}，正在重试...");
        
            // 只要失败，就重新调用 CreateRoom，它会生成一个新的随机数
            CreateRoom();
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log("成功加入房间！");
            
            if (loadingCircleAnime.isActiveAndEnabled)
            {
                loadingCircleAnime.StopAni();
            }
            loadingPanel.SetActive(false);
            
            // 设置房间信息
            RoomInfo info = PhotonNetwork.CurrentRoom;
            if (info !=  null)
            {
                roomNameText.text = info.Name;
                if (info.CustomProperties.ContainsKey("ITEM_ID"))
                {
                    string itemId = (string)info.CustomProperties["ITEM_ID"];
                    var itemData = ItemManager.instance.GetItem(itemId);
                    if (itemData)
                    {
                        roomIcon.sprite = itemData.bagHSprite;
                    }
                }
            }
            
            // 显示房间内界面
            roomBtnPanel.SetActive(true);
            roomPanel.SetActive(true);
            
            UpdatePlayerList();
            CheckRoomStatus();
        }
        
        // 有新玩家进入
        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            UpdatePlayerList();
            CheckRoomStatus();
            Debug.Log(newPlayer.UserId + "Join the room !");
        }

        // 有玩家离开
        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            UpdatePlayerList();
            CheckRoomStatus();
            Debug.Log(otherPlayer.UserId + "Leave the room !");
            
            // 出现意外时，通知GameManager关闭游戏并立刻回到主菜单界面
            GameManager.instance.MultiPlayError();
        }

        private void UpdatePlayerList()
        {
            // 清理旧列表UI
            foreach (Transform child in playerListContent)
            {
                Destroy(child.gameObject);
            }
            
            // 生成新按钮
            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            {
                GameObject playerItemGO = Instantiate(playerItemPrefab, playerListContent);
                PlayerItem playerItem = playerItemGO.GetComponent<PlayerItem>();
                playerItem.Setup(player);
            }
        }

        private bool CheckRoomStatus()
        {
            // 检查是否能够开始游戏，只有主机需要检查
            bool roomStatus = 
                PhotonNetwork.IsMasterClient && 
                PhotonNetwork.CurrentRoom.PlayerCount >= PhotonNetwork.CurrentRoom.MaxPlayers;
            
            startGameBtn.gameObject.SetActive(roomStatus);
            
            return roomStatus;
        }
    }
}