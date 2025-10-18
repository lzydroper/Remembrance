using System.Collections.Generic;
using UnityEngine;

namespace BagSystem
{
    [CreateAssetMenu(fileName = "ItemRecipe", menuName = "ItemRecipe", order = 0)]
    public class ItemRecipe : ScriptableObject
    {
        [Tooltip("合成所需的原料列表（顺序无关）")]
        public List<ItemData> ingredients;

        [Tooltip("合成成功后产出的物品")]
        public ItemData result;
    }
}