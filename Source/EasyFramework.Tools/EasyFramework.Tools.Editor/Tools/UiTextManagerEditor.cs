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
            
            var mgr = (UiTextManager)target;

            var lbl = mgr.FontAssetPresetId.DefaultIfNullOrEmpty("TODO");
            EasyEditorGUI.DrawSelectorDropdown(new SelectorDropdownConfig<string>(
                EditorHelper.TempContent("字体资产预设"),
                EditorHelper.TempContent2(lbl),
                UiTextPresetsManager.Instance.FontAssetPresets.Select(p => p.Id),
                id => mgr.FontAssetPresetId = id));

            lbl = mgr.TextPropertiesPresetId.DefaultIfNullOrEmpty("TODO");
            EasyEditorGUI.DrawSelectorDropdown(new SelectorDropdownConfig<string>(
                EditorHelper.TempContent("文本属性预设"),
                EditorHelper.TempContent2(lbl),
                UiTextPresetsManager.Instance.TextPropertiesPresets.Select(p => p.Id),
                id => mgr.TextPropertiesPresetId = id));

            mgr.ApplyPresets();

            if (mgr.FontAssetPresetId.IsNotNullOrEmpty())
            {
                var fontAssetPreset = mgr.GetFontAssetPreset();
                EasyEditorGUI.BoxGroup(
                    EditorHelper.TempContent($"字体资产预设 - {mgr.FontAssetPresetId}"),
                    rect =>
                    {
                        EditorGUI.BeginChangeCheck();
                        fontAssetPreset.FontAsset = EasyEditorField.UnityObject(
                            EditorHelper.TempContent2("字体资产"),
                            fontAssetPreset.FontAsset, false);
                        fontAssetPreset.Material = EasyEditorField.UnityObject(
                            EditorHelper.TempContent2("材质"),
                            fontAssetPreset.Material, false);
                        if (EditorGUI.EndChangeCheck())
                        {
                            EditorUtility.SetDirty(mgr);
                        }
                    });
            }

            if (mgr.TextPropertiesPresetId.IsNotNullOrEmpty())
            {
                var textPropertiesPreset = mgr.GetTextPropertiesPreset();
                EasyEditorGUI.BoxGroup(
                    EditorHelper.TempContent($"文本属性预设 - {mgr.TextPropertiesPresetId}"),
                    rect =>
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
