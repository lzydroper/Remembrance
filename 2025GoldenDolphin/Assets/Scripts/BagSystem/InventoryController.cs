using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SKCell;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [HideInInspector]
    private ItemGrid selectedItemGrid;
    public ItemGrid SelectedItemGrid
    {
        get => selectedItemGrid;
        set
        {
            selectedItemGrid = value;
            // inventoryHighlight.SetParent(value);
        }
    }
    
    private InventoryItem selectedItem;
    private InventoryItem overlapItem;
    private RectTransform rectTransform;
    private InventoryItem itemToHighLight;

    private Vector2Int oldPosition;
    private Vector2Int currentGridPosition;
    private bool isMovingWithKeyboard = false;
    private Vector2Int lastMoveDirection;

    [SerializeField] private List<ItemData> items;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform canvasTransform;
    // private InventoryHighlight inventoryHighlight;
    private List<InventoryItem> placedItems = new List<InventoryItem>();
    
    private void Awake()
    {
        // inventoryHighlight = GetComponent<InventoryHighlight>();
    }

    private void Update()
    {
        // ItemIconDrag();
        HandleKeyboardInput();
        // if (Input.GetKeyDown(KeyCode.Q))
        // {
        //     if(selectedItem==null)
        //         CreateRandomItem();
        // }
        //
        // if (Input.GetKeyDown(KeyCode.W))
        // {
        //     InsertRandomItem();
        // }
        //
        // if (Input.GetKeyDown(KeyCode.R))
        // {
        //     RotateItem();
        // }
        // if (selectedItemGrid == null)
        // {
        //     // inventoryHighlight.Show(false);
        //     return;
        // }
        HandleKeyboardMovement();
        // Debug.Log(selectedItemGrid.GetTileGridPosition(Input.mousePosition));
        // HandleHighlight();

        // if (Input.GetMouseButtonDown(0))
        // {
        //     LeftMouseButtonPress();
        // }
    }

    private void RotateItem()
    {
        if (selectedItem == null)
            return;
        bool originalRotated = selectedItem.rotated;
        selectedItem.Rotate();
        if (!CanPlaceItemAtPosition(currentGridPosition.x, currentGridPosition.y, selectedItem))
        {
            selectedItem.rotated = originalRotated;
            selectedItem.Rotate();
            SKUtils.EditorLogNormal("Invalid rotate.");
        }
        else
        {
            UpdateSelectedItemPosition(currentGridPosition);
        }
    }

    // private void HandleHighlight()
    // {
    //     Vector2Int positionOnGrid = isMovingWithKeyboard ? currentGridPosition : GetTileGridPosition();
    //
    //     if (oldPosition == positionOnGrid)
    //         return;
    //     oldPosition = positionOnGrid;
    //     if (!isMovingWithKeyboard)
    //     {
    //         currentGridPosition = positionOnGrid;
    //     }
    //     if (selectedItem == null)
    //     {
    //         itemToHighLight = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);
    //         if (itemToHighLight != null)
    //         {
    //             inventoryHighlight.Show(true);
    //             inventoryHighlight.SetSize(itemToHighLight);
    //             // inventoryHighlight.SetParent(selectedItemGrid);
    //             inventoryHighlight.SetPosition(selectedItemGrid, itemToHighLight);
    //         }
    //         else
    //         {
    //             inventoryHighlight.Show(false);
    //         }
    //     }
    //     else
    //     {
    //         bool canPlace = CanPlaceItemAtPosition(positionOnGrid.x, positionOnGrid.y, selectedItem);
    //         inventoryHighlight.Show(canPlace);
    //         inventoryHighlight.SetSize(selectedItem);
    //         // inventoryHighlight.SetParent(selectedItemGrid);
    //         inventoryHighlight.SetPosition(selectedItemGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);
    //     }
    // }

    public void CreateRandomItem()
    {
        if (selectedItemGrid == null)
            return;
        
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;
        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        rectTransform.SetAsLastSibling();
        int selectedItemID = UnityEngine.Random.Range(0, items.Count);
        inventoryItem.Set(items[selectedItemID]);
        
        currentGridPosition=Vector2Int.zero;
        isMovingWithKeyboard = true;
        UpdateSelectedItemPosition(currentGridPosition);
    }

    // private void InsertRandomItem()
    // {
    //     if(selectedItemGrid==null)
    //         return;
    //     CreateRandomItem();
    //     InventoryItem itemToInsert = selectedItem;
    //     selectedItem = null;
    //     InsertItem(itemToInsert);
    // }

    // private void InsertItem(InventoryItem itemToInsert)
    // {
    //     Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(itemToInsert);
    //     if (posOnGrid == null)
    //         return;
    //     selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    // }
    //
    // private void LeftMouseButtonPress()
    // {
    //     Vector2Int tileGridPosition = GetTileGridPosition();
    //     if (selectedItem == null)
    //     {
    //         PickupItem(tileGridPosition);
    //     }
    //     else
    //     {
    //         PlaceItem(tileGridPosition);
    //     }
    // }

    public void Move(Vector2Int direction)
    {
        if (selectedItem = null)
            return;

        Vector2Int newPosition = currentGridPosition + direction;
        if (CanPlaceItemAtPosition(newPosition.x, newPosition.y, selectedItem))
        {
            currentGridPosition = newPosition;
            UpdateSelectedItemPosition(currentGridPosition);
        }
    }
    public void ConfirmPress()
    {
        if (selectedItem == null)
            return;
        if (CanPlaceItemAtPosition(currentGridPosition.x, currentGridPosition.y, selectedItem))
        {
            bool complete = selectedItemGrid.PlaceItem(selectedItem, currentGridPosition.x, currentGridPosition.y,
                ref overlapItem);
            if (complete)
            {
                AddToPlacedItems(selectedItem);
                if (overlapItem != null)
                {
                    RemoveFromPlacedItems(overlapItem);
                    Destroy(overlapItem.gameObject);
                }

                selectedItem = null;
                isMovingWithKeyboard = false;
            }
            else
            {
                SKUtils.EditorLogNormal("Invalid position.");
            }
        }
    }
    
    private void UpdateSelectedItemPosition(Vector2Int gridPosition)
    {
        if (selectedItem == null || selectedItemGrid == null) return;

        selectedItem.onGridPositionX = gridPosition.x;
        selectedItem.onGridPositionY = gridPosition.y;
        // 更新视觉位置
        Vector2 position = selectedItemGrid.CalculatePositionOnGrid(selectedItem, currentGridPosition.x, currentGridPosition.y);
        rectTransform.localPosition = position;
        
        // 更新高亮
        // inventoryHighlight.SetPosition(selectedItemGrid, selectedItem, currentGridPosition.x, currentGridPosition.y);
    }
    private Vector2Int GetTileGridPosition()
    {
        Vector2 position = Input.mousePosition;
        if (selectedItem != null)
        {
            position.x -= (selectedItem.WIDTH - 1) * Constants.tileSizeWidth / 2;
            position.y += (selectedItem.HEIGHT - 1) * Constants.tileSizeHeight / 2;
        }
        return selectedItemGrid.GetTileGridPosition(position);
    }

    bool CanPlaceItemAtPosition(int posX, int posY, InventoryItem item)
    {
        if (selectedItemGrid == null || item == null)
            return false;
        // if (!selectedItemGrid.BoundaryCheck(posX, posY, item.WIDTH, item.HEIGHT))
        //     return false;
        // if (!CheckValidPositions(posX, posY, item)) 
        //     return false;
        // if (!CheckOverlap(posX, posY, item)) 
        //     return false;
        return selectedItemGrid.CanPlaceItem(item, posX, posY);
    }

    private bool CheckOverlap(int posX, int posY, InventoryItem item)
    {
        bool[,] shape = item.GetCurrentShape();
        for (int x = 0; x < item.WIDTH; x++)
        {
            for (int y = 0; y < item.HEIGHT; y++)
            {
                if (shape[x, y])
                {
                    int gridX = posX + x;
                    int gridY = posY + y;
                    InventoryItem itemAtPos = selectedItemGrid.GetItem(gridX, gridY);
                    if (itemAtPos != null && itemAtPos != selectedItem)
                        return false;
                }
            }
        }

        return true;
    }

    private bool CheckValidPositions(int posX, int posY, InventoryItem item)
    {
        bool[,] shape = item.GetCurrentShape();
        for (int x = 0; x < item.WIDTH; x++)
        {
            for (int y = 0; y < item.HEIGHT; y++)
            {
                if (shape[x, y])
                {
                    int gridX = posX + x;
                    int gridY = posY + y;
                    if (!IsPositionValid(gridX, gridY))
                        return false;
                }
            }
        }

        return true;
    }

    private bool IsPositionValid(int x, int y)
    {
        return selectedItemGrid.BoundaryCheck(x, y, 1, 1);
    }

    private void PlaceItem(Vector2Int tileGridPosition)
    {
        bool complete =
            selectedItemGrid.PlaceItem(selectedItem, tileGridPosition.x, tileGridPosition.y, ref overlapItem);
        if (complete)
        {
            selectedItem = null;
            if (overlapItem != null)
            {
                selectedItem = overlapItem;
                overlapItem = null;
                rectTransform = selectedItem.GetComponent<RectTransform>();
                rectTransform.SetAsLastSibling();
            }
        }
    }

    private void PickupItem(Vector2Int tileGridPosition)
    {
        selectedItem = selectedItemGrid.PickUpItem(tileGridPosition.x, tileGridPosition.y);
        if (selectedItem != null)
        {
            rectTransform = selectedItem.GetComponent<RectTransform>();
        }
    }

    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (selectedItem == null)
            {
                CreateRandomItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateItem();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ConfirmPress();
        }
    }

    Vector2Int GetCurrentItemGridPosition()
    {
        if (selectedItem != null)
        {
            return new Vector2Int(selectedItem.onGridPositionX, selectedItem.onGridPositionY);
        }
        return Vector2Int.zero;
    }

    Vector2Int? FindValidPositionForItem(InventoryItem item)
    {
        if (selectedItemGrid == null || item == null)
            return null;
        return selectedItemGrid.FindSpaceForObject(item);
    }

    void AddToPlacedItems(InventoryItem item)
    {
        if (!placedItems.Contains(item))
        {
            placedItems.Add(item);
        }
    }

    void RemoveFromPlacedItems(InventoryItem item)
    {
        if (placedItems.Contains(item))
        {
            placedItems.Remove(item);
        }
    }

    public int GetPlacedItemCount()
    {
        return placedItems.Count;
    }
    //
    // private void ItemIconDrag()
    // {
    //     if (selectedItem != null)
    //     {
    //         rectTransform.position = Input.mousePosition;
    //     }
    // }
    private void HandleKeyboardMovement()
    {
        if (selectedItem == null || selectedItemGrid == null) return;

        Vector2Int moveDirection = GetMoveDirection();
        
        if (moveDirection != Vector2Int.zero)
        {
            if (!isMovingWithKeyboard)
            {
                currentGridPosition = GetCurrentItemGridPosition();
                isMovingWithKeyboard = true;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
                Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                lastMoveDirection = moveDirection;
            }
        }
    }
    private Vector2Int GetMoveDirection()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            return Vector2Int.up;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            return Vector2Int.down;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            return Vector2Int.left;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            return Vector2Int.right;
            
        return Vector2Int.zero;
    }
}

