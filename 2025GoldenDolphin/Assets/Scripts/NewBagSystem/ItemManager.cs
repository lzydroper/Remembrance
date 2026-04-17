using System;
using System.Collections.Generic;
using System.Linq;
using SKCell;
using UnityEngine;

namespace NewBagSystem
{
    /*
     * 管理所有的itemso、recipeso
     */
    public class ItemManager : SKMonoSingleton<ItemManager>
    {
        private Dictionary<string, BasicItemData> _items = new();
        private Dictionary<string, Recipe> _recipes = new();
        private Dictionary<string, List<Recipe>> _ingredientToRecipes = new();

        private void Start()
        {
            BasicItemData[] items = Resources.LoadAll<BasicItemData>("Items/BasicItems");
            foreach (BasicItemData item in items)
            {
                _items.TryAdd(item.id, item);
            }
            
            Recipe[] recipes = Resources.LoadAll<Recipe>("Items/Recipes");
            foreach (Recipe recipe in recipes)
            {
                _recipes.TryAdd(recipe.id, recipe);
            }
            
            BuildIndex();
        }
        
        private void BuildIndex()
        {
            _ingredientToRecipes.Clear();
            foreach (var recipe in _recipes.Values)
            {
                foreach (var ing in recipe.ingredients)
                {
                    if (!_ingredientToRecipes.ContainsKey(ing))
                        _ingredientToRecipes[ing] = new List<Recipe>();
                    _ingredientToRecipes[ing].Add(recipe);
                }
            }
        }

        public BasicItemData GetItem(string id)
        {
            _items.TryGetValue(id, out BasicItemData item);
            return item;
        }

        public List<BasicItemData> GetAllItems()
        {
            return _items.Values.ToList();
        }

        public List<BasicItemData> GetRandomItem(int count)
        {
            if (count == 0 || count > _items.Count)
            {
                return null;
            }
            List<BasicItemData> items = _items.Values.ToList();
            System.Random rand = new System.Random();

            // Fisher-Yates 洗牌
            for (int i = items.Count - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                (items[i], items[j]) = (items[j], items[i]);
            }

            return items.Take(count).ToList();
        }

        public Recipe GetRecipe(string id)
        {
            _recipes.TryGetValue(id, out Recipe recipe);
            return recipe;
        }

        public Recipe GetRecipe(List<string> ids)
        {
            // 去重、排序，防止顺序不同但内容相同的情况
            var normalized = ids.OrderBy(x => x).ToList();

            foreach (var recipe in _recipes.Values)
            {
                if (recipe.ingredients.Count != normalized.Count)
                    continue;

                var recipeIngredients = recipe.ingredients.OrderBy(x => x).ToList();

                bool match = recipeIngredients.SequenceEqual(normalized);

                if (match)
                    return recipe;
            }

            return GetRecipe("failed"); // 未找到
        }

        public List<Recipe> GetAllRecipes()
        {
            return _recipes.Values.ToList();
        }
        
        // 选择物品时，悬浮显示可能的合成配方时调用，返回对应item可能组合的其他原料
        public List<Recipe> GetPossibleRecipes(string id)
        {
            _ingredientToRecipes.TryGetValue(id, out List<Recipe> recipes);
            return recipes;
        }
    }
}