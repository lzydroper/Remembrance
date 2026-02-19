using NewBagSystem;
using Photon.Pun;
using Photon.Realtime;
using SKCell;
using UnityEngine;

public class MyRoomItemBtn : MyMenuBtn
{
    [Header("UI引用")] 
    [SerializeField] private SKText roomNameText;
    [SerializeField] private SKText playerCountText;
    [SerializeField] private SKImage roomIcon;
    
    public RoomInfo _info;

    public void Setup(RoomInfo info)
    {
        _info = info;
        roomNameText.text = info.Name;
        playerCountText.text = info.PlayerCount + "/" + info.MaxPlayers;

        if (_info.CustomProperties.ContainsKey("ITEM_ID"))
        {
            string itemId = (string)_info.CustomProperties["ITEM_ID"];
            var itemData = ItemManager.instance.GetItem(itemId);
            if (itemData)
            {
                roomIcon.sprite = itemData.bagHSprite;
            }
        }
    }

    // public override void OnClick()
    // {
    //     base.OnClick();
    //     
    //     // 先判断房间情况，房间有空位才允许加入
    //     if (_info != null && _info.PlayerCount < _info.MaxPlayers)
    //     {
    //         PhotonNetwork.JoinRoom(_info.Name);
    //     }
    // }
}