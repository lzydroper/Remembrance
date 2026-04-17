using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NewBagSystem
{
    public class BagController : MonoBehaviour
    {
        // 导航系统根据这个来判断事件订阅情况
        [SerializeField] private int playerID;
        [Header("背包格子设定")] 
        [SerializeField] private const int GRID_SIZE_X = 4;
        [SerializeField] private const int GRID_SIZE_Y = 4;
        [SerializeField] private float cellSize = 111f;
        
        // ====  状态变量  ====
        private BagItem[,] _grid = new BagItem[GRID_SIZE_X, GRID_SIZE_Y];
        private List<BagItem> _placedItems = new();
        public BagItem _heldItem;
        private Vector2Int _heldItemAnchorPos;
        
        // 动画显示

        public static UnityAction<int> OnPlace;
        public static UnityAction<int> OnCannotPlace;

        private void OnEnable()
        {
            // 开始时订阅输入事件
            if (playerID == 0)
            {
                InputManager.OnP1Move += OnMove;
                InputManager.OnP1Rotate += OnRotate;
                InputManager.OnP1Confirm += OnConfirm;
            }
            else if (playerID == 1)
            {
                InputManager.OnP2Move += OnMove;
                InputManager.OnP2Rotate += OnRotate;
                InputManager.OnP2Confirm += OnConfirm;
            }
            // 获取将要加入的物品
            // BasicItemData itemToAdd = ItemManager.instance.GetRandomItem(1)[0];     // 测试
            BasicItemData itemToAdd = GameManager.instance.SelectResult[playerID];
            // 检查是否能够放置该物品
            if (!CanPlaceAnyItem(new List<BasicItemData>{ itemToAdd }))
            {
                OnCannotPlace?.Invoke(playerID);
            }
            // 若能放置则生成物品
            _heldItem = Instantiate(itemToAdd.bagPrefab, transform).GetComponent<BagItem>();
            _heldItemAnchorPos = Vector2Int.one;
            UpdateHeldItem();
        }

        private void OnDisable()
        {
            // 取消订阅输入
            if (playerID == 0)
            {
                InputManager.OnP1Move -= OnMove;
                InputManager.OnP1Rotate -= OnRotate;
                InputManager.OnP1Confirm -= OnConfirm;
            }
            else if (playerID == 1)
            {
                InputManager.OnP2Move -= OnMove;
                InputManager.OnP2Rotate -= OnRotate;
                InputManager.OnP2Confirm -= OnConfirm;
            }
        }

        #region 输入回调函数

        private void OnMove(Vector2Int direction)
        {
            Vector2Int newPos = _heldItemAnchorPos + direction;
            if (!CheckPosValid(newPos))
                return;
            _heldItemAnchorPos = newPos;
            UpdateHeldItem();
        }

        private void OnRotate()
        {
            _heldItem.Rotate();
            UpdateHeldItem();
        }

        private void OnConfirm()
        {
            if (!CheckValidity())
            {
                return;
            }

            foreach (var pos in _heldItem.GetOccupiedGridPositions())
            {
                _grid[pos.x, pos.y] = _heldItem;
            }
            _placedItems.Add(_heldItem);
            _heldItem.SetSprite(SpriteState.Bag);
            _heldItem = null;
            
            // 选完以后背包内就不需要输入了
            OnDisable();    // 有点弱智的代码了
            OnPlace?.Invoke(playerID);
            
            if (!CanPlaceAnyItem())
            {
                OnCannotPlace?.Invoke(playerID);
            }
        }

        #endregion

        #region 私有功能函数

        private bool CheckPosValid(Vector2Int pos)
        {
            // 边界
            bool IsInBounds(Vector2Int position)
            {
                return position.x is >= 0 and < GRID_SIZE_X &&
                       position.y is >= 0 and < GRID_SIZE_Y;
            }
            if (!IsInBounds(pos)) return false;
            
            // 四角
            bool IsInBorder(Vector2Int position)
            {
                return position.x is 0 or GRID_SIZE_X - 1 &&
                       position.y is 0 or GRID_SIZE_Y - 1;
            }
            if (IsInBorder(pos)) return false;
            
            return true;
        }
        
        private bool CheckValidity()
        {
            if (_heldItem == null)
                return false;
            List<Vector2Int> occupied = _heldItem.GetOccupiedGridPositions();
            foreach (Vector2Int pos in occupied)
            {
                if (!CheckPosValid(pos))
                {
                    return false;
                }

                if (_grid[pos.x, pos.y] != null)
                {
                    return false;
                }
            }
            return true;
        }

        private void UpdateHeldItem()
        {
            if (_heldItem == null) return;

            _heldItem.SetAnchor(_heldItemAnchorPos);
            _heldItem.transform.position = GridToWorldPosition(_heldItemAnchorPos);
            // 不管了，再检查一遍
            _heldItem.SetSprite(CheckValidity() ? SpriteState.BagH : SpriteState.BagHR);
        }

        private Vector3 GridToWorldPosition(Vector2Int gridPos)
        {
            float xOffset = (gridPos.x - GRID_SIZE_X / 2.0f + 0.5f) * cellSize;
            float yOffset = (gridPos.y - GRID_SIZE_Y / 2.0f + 0.5f) * cellSize;

            // 4. 将计算出的偏移量应用到GameObject的中心位置上。
            return transform.position + new Vector3(xOffset, yOffset, 0);
        }
        
        
        
        // ====  ai ia  ====
        // 遍历解决所有问题
        
        /// <summary>
        /// 检查背包中是否还有空间可以容纳 ItemManager 中的任何一个物品。
        /// 这是一个消耗性能的操作，请谨慎调用。
        /// </summary>
        /// <returns>如果至少有一个物品能被放下，则返回true；否则返回false。</returns>
        private bool CanPlaceAnyItem()
        {
            var allPossibleItems = ItemManager.instance.GetAllItems();
            if (allPossibleItems == null || allPossibleItems.Count == 0)
            {
                return false; // 如果没有物品可供检查，则认为无法放置
            }

            return CanPlaceAnyItem(allPossibleItems);
        }

        // private bool CanPlaceHeldItem()
        // {
        //     // 没有东西当然随便放
        //     if (_heldItem == null)
        //         return true;
        //     List<BasicItemData> list = new();
        //     list.Add(_heldItem.itemData);
        //     return CanPlaceAnyItem(list);
        // }

        #endregion

        #region 公共函数

        public List<string> GetPlacedItems()
        {
            List<string> items = new();
            foreach (var item in _placedItems)
            {
                items.Add(item.itemData.id);
            }
            return items;
        }
        
        public void ClearBag()
        {
            if (_heldItem != null)
            {
                Destroy(_heldItem.gameObject);
                _heldItem = null;
            }
            
            foreach (BagItem item in _placedItems)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
            
            _placedItems.Clear();

            System.Array.Clear(_grid, 0, _grid.Length);
        }

        #endregion
        
        #region CanPlaceAnyItem 的辅助函数 (新添加的)

        private bool CanPlaceAnyItem(List<BasicItemData> list)
        {
            // 获取所有有效的、可以作为锚点的位置
            List<Vector2Int> validAnchorPositions = GetAllValidGridPositions();

            // 遍历 ItemManager 提供的每一种物品
            foreach (var itemData in list)
            {
                // 遍历该物品的4个旋转方向
                for (int rotation = 0; rotation < 4; rotation++)
                {
                    List<Vector2Int> rotatedShape = GetRotatedShapeForItemData(itemData, rotation);
                    
                    // 遍历背包中每一个可能的锚点位置
                    foreach (var anchorPos in validAnchorPositions)
                    {
                        // 检查在此位置、此旋转下是否可以放置
                        if (CheckPlacementValidity(rotatedShape, anchorPos))
                        {
                            // 只要找到一个可以放置的情况，就立即返回true
                            return true;
                        }
                    }
                }
            }

            // 如果遍历完所有物品、所有旋转、所有位置都无法放置，则返回false
            return false;
        }

        /// <summary>
        /// 获取当前网格内所有有效的格子坐标。
        /// </summary>
        private List<Vector2Int> GetAllValidGridPositions()
        {
            var validPositions = new List<Vector2Int>();
            for (int x = 0; x < GRID_SIZE_X; x++)
            {
                for (int y = 0; y < GRID_SIZE_Y; y++)
                {
                    var pos = new Vector2Int(x, y);
                    if (CheckPosValid(pos))
                    {
                        validPositions.Add(pos);
                    }
                }
            }
            return validPositions;
        }

        /// <summary>
        /// 根据物品数据和旋转值，计算出旋转后的形状。
        /// (这是从 BagItem 中提取的静态逻辑版本)
        /// </summary>
        private List<Vector2Int> GetRotatedShapeForItemData(BasicItemData itemData, int rotation)
        {
            var rotatedShape = new List<Vector2Int>();
            foreach (var point in itemData.shape)
            {
                switch (rotation)
                {
                    case 0: rotatedShape.Add(point); break; // 0°
                    case 1: rotatedShape.Add(new Vector2Int(point.y, -point.x)); break; // 90°
                    case 2: rotatedShape.Add(new Vector2Int(-point.x, -point.y)); break; // 180°
                    case 3: rotatedShape.Add(new Vector2Int(-point.y, point.x)); break; // 270°
                }
            }
            return rotatedShape;
        }

        /// <summary>
        /// 检查一个给定的形状，在指定的锚点位置，是否可以被放置在当前背包网格中。
        /// </summary>
        private bool CheckPlacementValidity(List<Vector2Int> shape, Vector2Int anchor)
        {
            foreach (var point in shape)
            {
                Vector2Int finalPos = anchor + point;
                
                // 检查是否在有效区域内
                if (!CheckPosValid(finalPos))
                {
                    return false;
                }

                // 检查是否已被占据
                if (_grid[finalPos.x, finalPos.y] != null)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}