using System.Collections.Generic;
using System.Linq;
using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
    [CustomEditor(typeof(UiTextPresetsManager))]
    public class UiTextPresetsManagerEditor : OdinEditor
    {
        private InspectorProperty _fontAssetPresets;
        private InspectorProperty _textPropertiesPresets;

        protected override void OnEnable()
        {
            base.OnEnable();
            _fontAssetPresets = Tree.RootProperty.Children["_fontAssetPresets"];
            _textPropertiesPresets = Tree.RootProperty.Children["_textPropertiesPresets"];
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw(true);

            var mgr = (UiTextPresetsManager)target;
            
            EasyEditorGUI.Title("字体资产");
            _fontAssetPresets.Draw(new GUIContent("字体资产预设表"));
            EasyEditorGUI.Title("文本属性");
            _textPropertiesPresets.Draw(new GUIContent("文本属性预设表"));
            
            EasyEditorGUI.Title("默认预设");

            EasyEditorGUI.DrawSelectorDropdown(new SelectorDropdownConfig<string>(
                EditorHelper.TempContent("默认字体资产预设"),
                EditorHelper.TempContent2(mgr.DefaultFontAssetPresetId),
                mgr.FontAssetPresets.Keys,
                id => mgr.DefaultFontAssetPresetId = id));

            EasyEditorGUI.DrawSelectorDropdown(new SelectorDropdownConfig<string>(
                EditorHelper.TempContent("默认文本属性预设"),
                EditorHelper.TempContent2(mgr.DefaultTextPropertiesPresetId),
                mgr.TextPropertiesPresets.Keys,
                id => mgr.DefaultTextPropertiesPresetId = id));

            Tree.EndDraw();
        }
    }

    public class FontAssetPresetDrawer : OdinValueDrawer<FontAssetPreset>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            
            var preset = ValueEntry.SmartValue;

            var v = EasyEditorField.UnityObject(
                EditorHelper.TempContent("字体资产"),
                preset.FontAsset, false);

            EasyEditorField.UnityObject(
                EditorHelper.TempContent("材质"),
                ref preset.Material);

            if (v != preset.FontAsset)
            {
                preset.FontAsset = v;
                if (preset.Material == null)
                {
                    preset.Material = v.material;
                }
            }
        }
    }

    public class TextPropertiesPresetDrawer : OdinValueDrawer<TextPropertiesPreset>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var preset = ValueEntry.SmartValue;

            EasyEditorField.Value(
                EditorHelper.TempContent("字体大小"),
                ref preset.FontSize);
            
            EasyEditorField.Value(
                EditorHelper.TempContent("字体颜色"),
                ref preset.FontColor);
        }
    }
}
