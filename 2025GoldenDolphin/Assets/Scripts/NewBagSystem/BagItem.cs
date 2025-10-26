using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
/*
 * 只作为prefab使用，只存在于Bag中
 */
namespace NewBagSystem
{
    public enum SpriteState
    {
        Grid,
        GridH,
        Bag,
        BagH,
    }
    public class BagItem : MonoBehaviour
    {
        [SerializeField] private Image itemIcon;
        public BasicItemData itemData;
        private SpriteState _spriteState;
        private Vector2Int _anchorGridPosition;
        private int _currentRotation;

        /// <summary>
        /// 获取物品旋转后所有占用的格子坐标（相对于锚点）
        /// </summary>
        private List<Vector2Int> GetRotatedShape()
        {
            var rotatedShape = new List<Vector2Int>();
            foreach (var point in itemData.shape)
            {
                switch (_currentRotation)
                {
                    case 0: // 0°
                        rotatedShape.Add(point);
                        break;
                    case 1: // 90° clockwise
                        rotatedShape.Add(new Vector2Int(point.y, -point.x));
                        break;
                    case 2: // 180°
                        rotatedShape.Add(new Vector2Int(-point.x, -point.y));
                        break;
                    case 3: // 270° clockwise
                        rotatedShape.Add(new Vector2Int(-point.y, point.x));
                        break;
                }
            }
            return rotatedShape;
        }

        /// <summary>
        /// 获取物品在整个背包网格中实际占用的所有格子坐标
        /// </summary>
        public List<Vector2Int> GetOccupiedGridPositions()
        {
            var occupiedPositions = new List<Vector2Int>();
            var rotatedShape = GetRotatedShape();
            foreach (var point in rotatedShape)
            {
                occupiedPositions.Add(_anchorGridPosition + point);
            }
            return occupiedPositions;
        }

        public void Rotate()
        {
            _currentRotation = (_currentRotation + 1) % 4;
            // 你可以在这里添加旋转模型的视觉表现代码
            transform.Rotate(0, 0, -90);
        }

        public void SetSprite(SpriteState targetState)
        {
            Sprite sprite = targetState switch
            {
                SpriteState.Grid => itemData.gridSprite,
                SpriteState.GridH => itemData.gridHSprite,
                SpriteState.Bag => itemData.bagSprite,
                SpriteState.BagH => itemData.bagHSprite,
                _ => null
            };
            itemIcon.sprite = sprite;
        }
    }
}