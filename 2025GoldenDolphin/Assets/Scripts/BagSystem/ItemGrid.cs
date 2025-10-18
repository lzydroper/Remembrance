// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using SKCell;
//
// public class ItemGrid : MonoBehaviour
// {
//     private InventoryItem[,] inventoryItemSlot;
//     private bool[,] validGridPositions;
//     private RectTransform rectTransform;
//
//     private Vector2 positionOnTheGrid = new Vector2();
//     private Vector2Int tileGridPosition = new Vector2Int();
//
//     [SerializeField] private int gridSizeWidth = 20;
//     [SerializeField] private int gridSizeHeight = 10;
//     private void Start()
//     {
//         rectTransform = GetComponent<RectTransform>();
//         Init(gridSizeWidth, gridSizeHeight);
//     }
//
//     void Init(int width, int height)
//     {
//         inventoryItemSlot = new InventoryItem[width, height];
//         validGridPositions = new bool[width, height];
//         for (int x = 0; x < width; x++)
//         {
//             for (int y = 0; y < height; y++)
//             {
//                 validGridPositions[x, y] = true;
//             }
//         }
//         // 去角
//         validGridPositions[0, 0] = false;
//         validGridPositions[0, height - 1] = false;
//         validGridPositions[width - 1, 0] = false;
//         validGridPositions[width - 1, height - 1] = false;
//         
//         Vector2 size = new Vector2(width * Constants.tileSizeWidth, height * Constants.tileSizeHeight);
//         rectTransform.sizeDelta = size;
//     }
//     public Vector2Int GetTileGridPosition(Vector2 mousePosition)
//     {
//         positionOnTheGrid.x = mousePosition.x - rectTransform.position.x;
//         positionOnTheGrid.y = rectTransform.position.y - mousePosition.y;
//
//         tileGridPosition.x = (int)(positionOnTheGrid.x / Constants.tileSizeWidth);
//         tileGridPosition.y = (int)(positionOnTheGrid.y / Constants.tileSizeHeight);
//         return tileGridPosition;
//     }
//
//     public bool PlaceItem(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem overlapItem)
//     {
//         if (BoundaryCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT) == false)
//             return false;
//
//         if (ValidPositionCheck(posX, posY, inventoryItem) == false)
//         {
//             return false;
//         }
//         
//         if (OverLapCheck(posX, posY, inventoryItem, ref overlapItem) ==
//             false)
//         {
//             overlapItem = null;
//             return false;
//         }
//
//         if (overlapItem != null)
//         {
//             CleanGridReference(overlapItem);
//         }
//         PlaceItem(inventoryItem, posX, posY);
//         return true;
//     }
//
//     private bool ValidPositionCheck(int posX, int posY, InventoryItem item)
//     {
//         bool[,] currentShape = item.GetCurrentShape();
//         for (int x = 0; x < item.WIDTH; x++)
//         {
//             for (int y = 0; y < item.HEIGHT; y++)
//             {
//                 if (currentShape[x, y])
//                 {
//                     if (!IsPositionValid(posX + x, posY + y))
//                     {
//                         return false;
//                     }
//                 }
//             }
//         }
//
//         return true;
//     }
//
//     public void PlaceItem(InventoryItem inventoryItem, int posX, int posY)
//     {
//         RectTransform rectTransform = SKUtils.GetComponentNonAlloc<RectTransform>(inventoryItem.gameObject);
//         rectTransform.SetParent(this.rectTransform);
//
//         bool[,] currentShape = inventoryItem.GetCurrentShape();
//         for (int x = 0; x < inventoryItem.WIDTH; x++)
//         {
//             for (int y = 0; y < inventoryItem.HEIGHT; y++)
//             {
//                 if(currentShape[x,y])
//                     inventoryItemSlot[posX + x, posY + y] = inventoryItem;
//             }
//         }
//
//         inventoryItem.onGridPositionX = posX;
//         inventoryItem.onGridPositionY = posY;
//         Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);
//         rectTransform.localPosition = position;
//     }
//
//     public Vector2 CalculatePositionOnGrid(InventoryItem inventoryItem, int posX, int posY)
//     {
//         Vector2 position = new Vector2();
//         position.x = posX * Constants.tileSizeWidth + inventoryItem.WIDTH*Constants.tileSizeWidth/ 2;
//         position.y = -(posY * Constants.tileSizeHeight + inventoryItem.HEIGHT*Constants.tileSizeHeight / 2);
//         return position;
//     }
//
//     private bool OverLapCheck(int posX, int posY, InventoryItem item, ref InventoryItem overlapItem)
//     {
//         bool[,] currentShape = item.GetCurrentShape();
//         for (int x = 0; x < item.WIDTH; x++)
//         {
//             for (int y = 0; y < item.HEIGHT; y++)
//             {
//                 if (currentShape[x,y])
//                 {
//                     if (inventoryItemSlot[posX + x, posY + y] != null)
//                     {
//                         if (overlapItem != null)
//                         {
//                             overlapItem = inventoryItemSlot[posX + x, posY + y];
//                         }
//                         else
//                         {
//                             if (overlapItem != inventoryItemSlot[posX + x, posY + y])
//                             {
//                                 return false;
//                             }
//                         }
//                     }
//                 }
//             }
//         }
//         return true;
//     }
//     private bool CheckAvailableSpace(int posX, int posY, InventoryItem item)
//     {
//         bool[,] currentShape = item.GetCurrentShape();
//         for (int x = 0; x < item.WIDTH; x++)
//         {
//             for (int y = 0; y < item.HEIGHT; y++)
//             {
//                 if (currentShape[x, y])
//                 {
//                     if (inventoryItemSlot[posX + x, posY + y] != null)
//                     {
//                         return false;
//                     }
//                 }
//             }
//         }
//         return true;
//     }
//     public InventoryItem PickUpItem(int x, int y)
//     {
//         InventoryItem toReturn = inventoryItemSlot[x, y];
//         if (toReturn == null)
//             return null;
//         
//         CleanGridReference(toReturn);
//         return toReturn;
//     }
//
//     public Vector2Int? FindSpaceForObject(InventoryItem itemToInsert)
//     {
//         int height = gridSizeHeight - itemToInsert.HEIGHT + 1;
//         int width = gridSizeWidth - itemToInsert.WIDTH + 1;
//         for (int y = 0; y < height; y++)
//         {
//             for (int x = 0; x < width; x++)
//             {
//                 if (CheckAvailableSpace(x, y, itemToInsert) && ValidPositionCheck(x,y,itemToInsert))
//                 {
//                     return new Vector2Int(x, y);
//                 }
//             }
//         }
//
//         return null;
//     }
//     private void CleanGridReference(InventoryItem item)
//     {
//         bool[,] currentShape = item.GetCurrentShape();
//         for (int ix = 0; ix < item.WIDTH; ix++)
//         {
//             for (int iy = 0; iy < item.HEIGHT; iy++)
//             {
//                 if (currentShape[ix, iy])
//                 {
//                     inventoryItemSlot[item.onGridPositionX + ix, item.onGridPositionY + iy] = null;
//                 }
//             }
//         }
//     }
//
//     bool PositionCheck(int posX, int posY)
//     {
//         if (posX < 0 || posY < 0)
//         {
//             return false;
//         }
//
//         if (posX >= gridSizeWidth || posY >= gridSizeHeight)
//         {
//             return false;
//         }
//
//         return true;
//     }
//
//     bool IsPositionValid(int posX, int posY)
//     {
//         if (!PositionCheck(posX, posY))
//         {
//             return false;
//         }
//
//         return validGridPositions[posX, posY];
//     }
//     public bool BoundaryCheck(int posX, int posY, int width, int height)
//     {
//         if (!PositionCheck(posX, posY))
//             return false;
//
//         if (!PositionCheck(posX + width - 1, posY + height - 1)) 
//             return false;
//
//         return true;
//     }
//
//     public InventoryItem GetItem(int x, int y)
//     {
//         return inventoryItemSlot[x, y];
//     }
//
//     public bool CanPlaceItem(InventoryItem item, int posX, int posY)
//     {
//         if (!BoundaryCheck(posX, posY, item.WIDTH, item.HEIGHT))
//             return false;
//         bool[,] shape = item.GetCurrentShape();
//         for (int x = 0; x < item.WIDTH; x++)
//         {
//             for (int y = 0; y < item.HEIGHT; y++)
//             {
//                 if (shape[x, y] && !IsPositionValid(posX + x, posY + y))
//                 {
//                     return false;
//                 }
//             }
//         }
//
//         for (int x = 0; x < item.WIDTH; x++)
//         {
//             for (int y = 0; y < item.HEIGHT; y++)
//             {
//                 if (shape[x, y])
//                 {
//                     InventoryItem itemAtPos = inventoryItemSlot[posX + x, posY + y];
//                     if (itemAtPos != null && itemAtPos != item)
//                     {
//                         return false;
//                     }
//                 }
//             }
//         }
//
//         return true;
//     }
//     private void OnDrawGizmos()
//     {
//         if (validGridPositions == null)
//         {
//             return;
//         }
//
//         for (int x = 0; x < gridSizeWidth; x++)
//         {
//             for (int y = 0; y < gridSizeHeight; y++)
//             {
//                 if (validGridPositions[x, y])
//                 {
//                     Gizmos.color = Color.green;
//                 }
//                 else
//                 {
//                     Gizmos.color = Color.red;
//                 }
//
//                 Vector3 worldPos = transform.position +
//                                    new Vector3(x * Constants.tileSizeWidth + Constants.tileSizeWidth / 2,
//                                        -y * Constants.tileSizeHeight - Constants.tileSizeHeight / 2, 0);
//                 Gizmos.DrawWireCube(worldPos, new Vector3(Constants.tileSizeWidth, Constants.tileSizeHeight, 0));
//             }
//         }
//     }
// }
