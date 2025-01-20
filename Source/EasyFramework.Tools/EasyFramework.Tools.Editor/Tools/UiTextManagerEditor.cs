using System.Linq;
using EasyFramework.Generic;
using EasyFramework.Inspector;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    [CustomEditor(typeof(UiTextManager))]
    public class UiTextManagerEditor : OdinEditor
    {
        protected override void DrawTree()
        {
            Tree.BeginDraw(true);

            var fontAssetPresetId = Tree.RootProperty.Children["_fontAssetPresetId"];
            var textPropertiesPresetId = Tree.RootProperty.Children["_textPropertiesPresetId"];

            EasyEditorGUI.DrawSelectorDropdown(new SelectorDropdownConfig<string>(
                "字体资产预设",
                fontAssetPresetId.WeakSmartValue<string>().DefaultIfNullOrEmpty("TODO"),
                UiTextPresetsManager.Instance.FontAssetPresets.Select(p => p.Id),
                id => fontAssetPresetId.SetWeakSmartValue(id)));

            EasyEditorGUI.DrawSelectorDropdown(new SelectorDropdownConfig<string>(
                "文本属性预设",
                textPropertiesPresetId.WeakSmartValue<string>().DefaultIfNullOrEmpty("TODO"),
                UiTextPresetsManager.Instance.TextPropertiesPresets.Select(p => p.Id),
                id => textPropertiesPresetId.SetWeakSmartValue(id)));

            var mgr = (UiTextManager)target;
            mgr.ApplyPresets();

            if (mgr.FontAssetPresetId.IsNotNullOrEmpty())
            {
                var fontAssetPreset = mgr.GetFontAssetPreset();
                EasyEditorGUI.FoldoutGroup(
                    new FoldoutGroupConfig(this, $"字体资产预设 - {mgr.FontAssetPresetId}")
                    {
                        Expandable = false,
                        OnContentGUI = rect =>
                        {
                            EditorGUI.BeginChangeCheck();
                            fontAssetPreset.FontAsset = EasyEditorField.UnityObject(
                                new GUIContent("字体资产"),
                                fontAssetPreset.FontAsset, false);
                            fontAssetPreset.Material = EasyEditorField.UnityObject(
                                new GUIContent("材质"),
                                fontAssetPreset.Material, false);
                            if (EditorGUI.EndChangeCheck())
                            {
                                EditorUtility.SetDirty(mgr);
                            }
                        }
                    });
            }

            if (mgr.TextPropertiesPresetId.IsNotNullOrEmpty())
            {
                var textPropertiesPreset = mgr.GetTextPropertiesPreset();
                EasyEditorGUI.FoldoutGroup(
                    new FoldoutGroupConfig(this, $"文本属性预设 - {mgr.TextPropertiesPresetId}")
                    {
                        Expandable = false,
                        OnContentGUI = rect =>
                        {
                            EditorGUI.BeginChangeCheck();
                            textPropertiesPreset.FontSize = EditorGUILayout.FloatField(
                                "字体资产",
                                textPropertiesPreset.FontSize);

                            textPropertiesPreset.FontColor = SirenixEditorFields.ColorField(
                                new GUIContent("字体颜色"),
                                textPropertiesPreset.FontColor);

                            if (EditorGUI.EndChangeCheck())
                            {
                                EditorUtility.SetDirty(mgr);
                            }
                        }
                    });
            }

            if (SirenixEditorGUI.Button("切换预设管理器", ButtonSizes.Medium))
            {
                Selection.activeObject = UiTextPresetsManager.Instance;
                EditorGUIUtility.PingObject(UiTextPresetsManager.Instance);
            }

            Tree.EndDraw();
        }
    }
}
