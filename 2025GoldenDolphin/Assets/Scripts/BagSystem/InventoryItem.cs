using System.Collections;
using System.Collections.Generic;
using SKCell;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public ItemData itemData;

    public int HEIGHT
    {
        get
        {
            if (rotated == false)
            {
                return itemData.height;
            }

            return itemData.width;
        }
    }

    public int WIDTH
    {
        get
        {
            if (rotated == false)
            {
                return itemData.width;
            }

            return itemData.height;
        }
    }

    public int onGridPositionX;
    public int onGridPositionY;

    public bool rotated = false;
    public void Set(ItemData itemData)
    {
        this.itemData = itemData;
        // TODO: 改UI
        SKCell.SKUtils.GetComponentNonAlloc<SKImage>(gameObject).sprite = itemData.itemIcon;
        // GetComponent<Image>().sprite = itemData.itemIcon;

        Vector2 size = new Vector2();
        size.x = itemData.width * Constants.tileSizeWidth;
        size.y = itemData.height * Constants.tileSizeHeight;
        SKCell.SKUtils.GetComponentNonAlloc<RectTransform>(gameObject).sizeDelta = size;
        // GetComponent<RectTransform>().sizeDelta = size;
    }

    public void Rotate()
    {
        rotated = !rotated;
        // RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransform rectTransform = SKCell.SKUtils.GetComponentNonAlloc<RectTransform>(gameObject);
        if (rotated)
        {
            rectTransform.rotation = Quaternion.Euler(0, 0, 90f);
            rectTransform.sizeDelta = new Vector2(itemData.height * Constants.tileSizeWidth,
                itemData.width * Constants.tileSizeHeight);
        }
        else
        {
            rectTransform.rotation = Quaternion.Euler(0, 0, 0f);
            rectTransform.sizeDelta = new Vector2(itemData.width * Constants.tileSizeWidth,
                itemData.height * Constants.tileSizeHeight);
        }
    }

    public bool[,] GetCurrentShape()
    {
        return itemData.GetRotatedShape(rotated);
    }
    // public bool[,] GetRotatedShape()
    // {
    //     if (!rotated)
    //     {
    //         return itemData.shape;
    //     }
    //
    //     bool[,] rotatedShape = new bool[itemData.height, itemData.width];
    //     for (int x = 0; x < itemData.width; x++)
    //     {
    //         for (int y = 0; y < itemData.height; y++)
    //         {
    //             rotatedShape[itemData.height - 1 - y, x] = itemData.shape[x, y];
    //         }
    //     }
    //
    //     return rotatedShape;
    // }
}
