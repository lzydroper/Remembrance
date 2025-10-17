// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class InventoryHighlight : MonoBehaviour
// {
//     // 白底的图像
//     [SerializeField] private RectTransform highlighter;
//
//     public void SetSize(InventoryItem targetItem)
//     {
//         Vector2 size = new Vector2();
//         size.x = targetItem.WIDTH * Constants.tileSizeWidth;
//         size.y = targetItem.HEIGHT * Constants.tileSizeHeight;
//         highlighter.sizeDelta = size;
//     }
//
//     public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem)
//     {
//         Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, targetItem.onGridPositionX, targetItem.onGridPositionY);
//         highlighter.localPosition = pos;
//     }
//
//     public void SetParent(ItemGrid targetGrid)
//     {
//         if(targetGrid==null)
//             return;
//         
//         highlighter.SetParent(targetGrid.GetComponent<RectTransform>());
//     }
//
//     public void Show(bool b)
//     {
//         highlighter.gameObject.SetActive(b);
//     }
//
//     public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem, int posX, int posY)
//     {
//         Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, posX, posY);
//         highlighter.localPosition = pos;
//     }
// }
