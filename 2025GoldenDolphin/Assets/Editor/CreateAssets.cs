using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using NewBagSystem;
using OfficeOpenXml;

public class CreateAssets : Editor
{
    private enum BasicItemSheetTitle
    {
        ID,
        GridSpritePath,
        GridHSpritePath,
        BagSpritePath,
        BagHSpritePath,
        BagPrefabPath,
        D3PrefabPath,
        Shape,
        Description,
    }
    private enum RecipeSheetTitle
    {
        ID,
        Ingredients,
        Score,
        Type,
        TexturePath,
        TextContent,
        Scale,
        RoundStaEff,
        RoundEndEff,
        Description,
    }
    [MenuItem("Tools/CreateAssetsFromExcel")]
    public static void CreateAssetsFromExcel()
    {
        string excelPath = Path.Combine(Application.dataPath, "Editor/Data.xlsx");
        var package = new ExcelPackage(new FileInfo(excelPath));
        ExcelWorksheet basicItemSheet = package.Workbook.Worksheets["BasicItemData"];
        ExcelWorksheet recipeSheet = package.Workbook.Worksheets["Recipe"];
        CreateBasicItem(basicItemSheet);
        CreateRecipe(recipeSheet);
    }

    private static void CreateBasicItem(ExcelWorksheet sheet)
    {
        int startRow = 2, startCol = 1;
        for (int i = startRow; i <= sheet.Dimension.Rows; i++)
        {
            BasicItemData item = CreateInstance<BasicItemData>();
            item.id = sheet.Cells[i, startCol + (int)BasicItemSheetTitle.ID].Text;
            item.gridSprite =
                AssetDatabase.LoadAssetAtPath<Sprite>(sheet.Cells[i, startCol + (int)BasicItemSheetTitle.GridSpritePath].Text);
            item.gridHSprite =
                AssetDatabase.LoadAssetAtPath<Sprite>(sheet.Cells[i, startCol + (int)BasicItemSheetTitle.GridHSpritePath].Text);
            item.bagSprite =
                AssetDatabase.LoadAssetAtPath<Sprite>(sheet.Cells[i, startCol + (int)BasicItemSheetTitle.BagSpritePath].Text);
            item.bagHSprite =
                AssetDatabase.LoadAssetAtPath<Sprite>(sheet.Cells[i, startCol + (int)BasicItemSheetTitle.BagHSpritePath].Text);
            item.bagPrefab = 
                AssetDatabase.LoadAssetAtPath<GameObject>(sheet.Cells[i, startCol + (int)BasicItemSheetTitle.BagPrefabPath].Text);
            item.d3Prefab = 
                AssetDatabase.LoadAssetAtPath<GameObject>(sheet.Cells[i, startCol + (int)BasicItemSheetTitle.D3PrefabPath].Text);
            item.shape = ParseVector2(sheet.Cells[i, startCol + (int)BasicItemSheetTitle.Shape].Text);

            string filepath = $"Assets/Resources/Items/BasicItems/{item.id}.asset";
            AssetDatabase.CreateAsset(item, filepath);
            AssetDatabase.SaveAssets();
        }
    }

    private static void CreateRecipe(ExcelWorksheet sheet)
    {
        int startRow = 2, startCol = 1;
        for (int i = startRow; i <= sheet.Dimension.Rows; i++)
        {
            Recipe recipe = CreateInstance<Recipe>();
            recipe.id = sheet.Cells[i, startCol + (int)RecipeSheetTitle.ID].Text;
            recipe.ingredients = ParseStringList(sheet.Cells[i, startCol + (int)RecipeSheetTitle.Ingredients].Text);
            recipe.score = int.Parse(sheet.Cells[i, startCol + (int)RecipeSheetTitle.Score].Text);
            recipe.type = (RecipeType)int.Parse(sheet.Cells[i, startCol + (int)RecipeSheetTitle.Type].Text);
            recipe.texture = 
                AssetDatabase.LoadAssetAtPath<Texture>(sheet.Cells[i, startCol + (int)RecipeSheetTitle.TexturePath].Text);
            recipe.textContent = sheet.Cells[i, startCol + (int)RecipeSheetTitle.TextContent].Text;
            recipe.scale = ParseVector3(sheet.Cells[i, startCol + (int)RecipeSheetTitle.Scale].Text);

            string filepath = $"Assets/Resources/items/Recipes/{recipe.id}.asset";
            AssetDatabase.CreateAsset(recipe, filepath);
            AssetDatabase.SaveAssets();
        }
    }
    
    private static List<Vector2Int> ParseVector2(string input)
    {
        var vectorList = new List<Vector2Int>();

        // 去除字符串中的空格（可选，根据实际格式调整）
        input = input.Replace(" ", "");

        // 分割出每个括号内的坐标（如 "(0,0)"）
        var vectorStrs = input.Split(new[] { "),(" }, StringSplitOptions.RemoveEmptyEntries);

        // 处理第一个和最后一个字符串的括号
        if (vectorStrs.Length > 0)
        {
            vectorStrs[0] = vectorStrs[0].TrimStart('(');
            vectorStrs[^1] = vectorStrs[^1].TrimEnd(')');
        }

        // 解析每个坐标字符串
        foreach (var str in vectorStrs)
        {
            var parts = str.Split(',');
            if (parts.Length == 2 && 
                int.TryParse(parts[0], out int x) && 
                int.TryParse(parts[1], out int y))
            {
                vectorList.Add(new Vector2Int(x, y));
            }
            else
            {
                Debug.LogWarning($"无效的坐标格式：{str}");
            }
        }

        return vectorList;
    }

    private static Vector3 ParseVector3(string input)
    {
        string str = input.Replace(" ", "");
        str = str.TrimStart('(');
        str = str.TrimEnd(')');
        var parts = str.Split(',');
        if (parts.Length == 3 &&
            float.TryParse(parts[0], out float x) &&
            float.TryParse(parts[1], out float y) &&
            float.TryParse(parts[2], out float z))
        {
            return new Vector3(x, y, z);
        }

        Debug.LogWarning($"无效的坐标格式：{str}");
        return Vector3.one;
    }
    
    private static List<string> ParseStringList(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return new List<string>(); // 处理空字符串，返回空列表
        }
        
        // 按逗号分割字符串，过滤空项（避免连续逗号导致的空元素）
        string[] parts = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        
        // 转换为 List<string>
        return parts.ToList();
    }

}