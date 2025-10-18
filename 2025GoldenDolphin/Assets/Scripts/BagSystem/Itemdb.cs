using System.Collections.Generic;
using UnityEngine;

namespace BagSystem
{
    [CreateAssetMenu(fileName = "Itemdb", menuName = "Itemdb", order = 0)]
    public class Itemdb : ScriptableObject
    {
        [Tooltip("游戏中所有的物品列表")] public List<ItemData> items = new();
        [Tooltip("游戏中所有的合成配方列表")] public List<ItemRecipe> recipes = new();
        [Tooltip("游戏中所有的配方解锁列表")] public HashSet<ItemData> locklists = new();
        [Tooltip("游戏中所有的基础物品的prefab")] public List<GameObject> prefabs = new();
    }
}