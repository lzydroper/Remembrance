using NewBagSystem;
using Photon.Pun;
using SKCell;
using UnityEngine;

namespace Network
{
    public class PlayerItem : MonoBehaviour
    {
        [Header("UI引用")] 
        [SerializeField] private SKText playerIDText;
        [SerializeField] private SKImage playerIcon;
        
        public Photon.Realtime.Player Player;
        
        public void Setup(Photon.Realtime.Player player)
        {
            Player = player;
            playerIDText.text = player.UserId;

            if (player.CustomProperties.ContainsKey("ITEM_ID"))
            {
                string itemId = (string)player.CustomProperties["ITEM_ID"];
                var itemData = ItemManager.instance.GetItem(itemId);
                if (itemData)
                {
                    playerIcon.sprite = itemData.bagHSprite;
                }
            }
        }
    }
}