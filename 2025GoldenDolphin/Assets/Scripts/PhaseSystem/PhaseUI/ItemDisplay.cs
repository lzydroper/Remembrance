using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PhaseSystem
{
    public class ItemDisplay : MonoBehaviour
    {
        [SerializeField] public Image iconImage;
        // [SerializeField] private TextMeshProUGUI nameText;

        private ItemData currentItem;

        public void Setup(ItemData itemData)
        {
            currentItem = itemData;
            iconImage.sprite = itemData.icon;
            // nameText.text = itemData.itemName;
            SetAsAvailable(); // 确保每次初始化时都是可用状态
        }

        // 设置为可用状态
        public void SetAsAvailable()
        {
            Color color = iconImage.color;
            color.a = 1.0f;
            iconImage.color = color;
        }

        // 设置为不可用/已选状态
        public void SetAsUnavailable()
        {
            Color color = iconImage.color;
            color.a = 0.5f;
            iconImage.color = color;;
        }
    }
}