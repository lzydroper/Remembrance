// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.EventSystems;
//
// [RequireComponent(typeof(ItemGrid))]
// public class GridInteract : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
// {
//     private InventoryController inventoryController;
//     private ItemGrid itemGrid;
//     private void Awake()
//     {
//         inventoryController = FindObjectOfType(typeof(InventoryController)) as InventoryController;
//         itemGrid = GetComponent<ItemGrid>();
//     }
//
//     public void OnPointerEnter(PointerEventData eventData)
//     {
//         // Debug.Log("Enter");
//         inventoryController.SelectedItemGrid = itemGrid;
//     }
//
//     public void OnPointerExit(PointerEventData eventData)
//     {
//         // Debug.Log("Exit");
//         inventoryController.SelectedItemGrid = null;
//     }
// }
