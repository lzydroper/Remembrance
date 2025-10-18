// using UnityEditor;
// using UnityEngine;
//
// [CustomEditor(typeof(ItemData))]
// public class ItemDataEditor : Editor
// {
//     private const float cellSize = 20f;
//     private const float spacing = 2f;
//
//     public override void OnInspectorGUI()
//     {
//         // 绘制默认属性
//         DrawDefaultInspector();
//
//         ItemData itemData = (ItemData)target;
//
//         EditorGUILayout.Space();
//         EditorGUILayout.LabelField("Shape Editor", EditorStyles.boldLabel);
//
//         // 确保形状数组已初始化
//         if (itemData.shape == null || itemData.shape.Length != itemData.height)
//         {
//             itemData.InitializeShape();
//         }
//
//         // 绘制形状编辑器
//         DrawShapeEditor(itemData);
//
//         if (GUI.changed)
//         {
//             EditorUtility.SetDirty(itemData);
//         }
//     }
//
//     private void DrawShapeEditor(ItemData itemData)
//     {
//         // 开始一个垂直组来居中显示网格
//         EditorGUILayout.BeginVertical();
//         GUILayout.FlexibleSpace();
//         
//         // 开始一个水平组来居中显示网格
//         EditorGUILayout.BeginHorizontal();
//         GUILayout.FlexibleSpace();
//
//         // 绘制网格
//         Rect gridRect = EditorGUILayout.BeginVertical();
//         
//         for (int y = 0; y < itemData.height; y++)
//         {
//             EditorGUILayout.BeginHorizontal();
//             for (int x = 0; x < itemData.width; x++)
//             {
//                 // 确保不越界
//                 if (y < itemData.shape.Length && x < itemData.shape[y].columns.Length)
//                 {
//                     bool currentValue = itemData.shape[y].columns[x];
//                     bool newValue = EditorGUILayout.Toggle(currentValue, GUILayout.Width(cellSize));
//                     if (newValue != currentValue)
//                     {
//                         itemData.shape[y].columns[x] = newValue;
//                     }
//                 }
//             }
//             EditorGUILayout.EndHorizontal();
//         }
//         
//         EditorGUILayout.EndVertical();
//
//         GUILayout.FlexibleSpace();
//         EditorGUILayout.EndHorizontal();
//         
//         GUILayout.FlexibleSpace();
//         EditorGUILayout.EndVertical();
//
//         // 添加一些控制按钮
//         EditorGUILayout.BeginHorizontal();
//         if (GUILayout.Button("Fill All"))
//         {
//             SetAllCells(itemData, true);
//         }
//         if (GUILayout.Button("Clear All"))
//         {
//             SetAllCells(itemData, false);
//         }
//         if (GUILayout.Button("Invert"))
//         {
//             InvertCells(itemData);
//         }
//         EditorGUILayout.EndHorizontal();
//     }
//
//     private void SetAllCells(ItemData itemData, bool value)
//     {
//         for (int y = 0; y < itemData.height; y++)
//         {
//             for (int x = 0; x < itemData.width; x++)
//             {
//                 itemData.shape[y].columns[x] = value;
//             }
//         }
//     }
//
//     private void InvertCells(ItemData itemData)
//     {
//         for (int y = 0; y < itemData.height; y++)
//         {
//             for (int x = 0; x < itemData.width; x++)
//             {
//                 itemData.shape[y].columns[x] = !itemData.shape[y].columns[x];
//             }
//         }
//     }
// }