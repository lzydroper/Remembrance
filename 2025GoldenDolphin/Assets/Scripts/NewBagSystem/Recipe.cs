using System.Collections.Generic;
using UnityEngine;

namespace NewBagSystem
{
    public enum RecipeType
    {
        Good,
        Bad,
    }
    [CreateAssetMenu(fileName = "RECIPE", menuName = "ITEM/RECIPE", order = 0)]
    public class Recipe : ScriptableObject
    {
        public string id;
        public List<string> ingredients;
        public int score;
        public RecipeType type;
    }
}