using System.Collections.Generic;
using UnityEngine;

namespace NewBagSystem
{
    [CreateAssetMenu(fileName = "BASICITEM", menuName = "ITEM/BASIC ITEM", order = 0)]
    public class BasicItemData : ScriptableObject
    {
        public string id;
        [Header("Sprite")]
        public Sprite gridSprite;
        public Sprite gridHSprite;
        public Sprite bagSprite;
        public Sprite bagHSprite;
        [Header("Prefab")]
        public GameObject bagPrefab;
        public GameObject d3Prefab;     // 可恶，为什么不能是3DPrefab T_T
        [Tooltip("物品的形状，第一个坐标为锚点，锚点默认为(0,0)")]
        public List<Vector2Int> shape;
    }
}