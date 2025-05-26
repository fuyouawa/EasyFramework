// using EasyFramework.Editor;
// using Sirenix.OdinInspector;
// using Sirenix.OdinInspector.Editor;
// using Sirenix.Utilities.Editor;
// using UnityEditor;
// using UnityEngine;
//
// namespace EasyFramework.ToolKit.Editor
// {
//     [CustomEditor(typeof(UiTextManager))]
//     public class UiTextManagerEditor : OdinEditor
//     {
//         protected override void DrawTree()
//         {
//             Tree.BeginDraw(true);
//             
//             var mgr = (UiTextManager)target;
//             
//             var lbl = mgr.FontAssetPresetId.DefaultIfNullOrEmpty("TODO");
//             
//             EasyEditorGUI.DrawSelectorDropdown(
//                 UiTextPresetsSettings.Instance.FontAssetPresets.Keys,
//                 EditorHelper.TempContent("字体资产预设"),
//                 EditorHelper.TempContent2(lbl),
//                 id => mgr.FontAssetPresetId = id);
//             
//             lbl = mgr.TextPropertiesPresetId.DefaultIfNullOrEmpty("TODO");
//             
//             EasyEditorGUI.DrawSelectorDropdown(
//                 UiTextPresetsSettings.Instance.TextPropertiesPresets.Keys,
//                 EditorHelper.TempContent("文本属性预设"),
//                 EditorHelper.TempContent2(lbl),
//                 id => mgr.TextPropertiesPresetId = id);
//             
//             mgr.ApplyPresets();
//             
//             var fontAssetPreset = mgr.GetFontAssetPreset();
//             if (fontAssetPreset != null)
//             {
//                 EasyEditorGUI.BoxGroup(
//                     EditorHelper.TempContent($"字体资产预设 - {mgr.FontAssetPresetId}"),
//                     rect =>
//                     {
//                         EditorGUI.BeginChangeCheck();
//                         fontAssetPreset.FontAsset = EasyEditorField.UnityObject(
//                             EditorHelper.TempContent2("字体资产"),
//                             fontAssetPreset.FontAsset, false);
//                         fontAssetPreset.Material = EasyEditorField.UnityObject(
//                             EditorHelper.TempContent2("材质"),
//                             fontAssetPreset.Material, false);
//                         if (EditorGUI.EndChangeCheck())
//                         {
//                             EditorUtility.SetDirty(mgr);
//                         }
//                     });
//             }
//             
//             var textPropertiesPreset = mgr.GetTextPropertiesPreset();
//             if (textPropertiesPreset != null)
//             {
//                 EasyEditorGUI.BoxGroup(
//                     EditorHelper.TempContent($"文本属性预设 - {mgr.TextPropertiesPresetId}"),
//                     rect =>
//                     {
//                         EditorGUI.BeginChangeCheck();
//                         textPropertiesPreset.FontSize = EditorGUILayout.FloatField(
//                             "字体资产",
//                             textPropertiesPreset.FontSize);
//             
//                         textPropertiesPreset.FontColor = SirenixEditorFields.ColorField(
//                             new GUIContent("字体颜色"),
//                             textPropertiesPreset.FontColor);
//             
//                         if (EditorGUI.EndChangeCheck())
//                         {
//                             EditorUtility.SetDirty(mgr);
//                         }
//                     });
//             }
//             
//             if (SirenixEditorGUI.Button("切换预设管理器", ButtonSizes.Medium))
//             {
//                 Selection.activeObject = UiTextPresetsSettings.Instance;
//                 EditorGUIUtility.PingObject(UiTextPresetsSettings.Instance);
//             }
//
//             Tree.EndDraw();
//         }
//     }
// }
