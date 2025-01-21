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
        protected override void DrawTree()
        {
            Tree.BeginDraw(true);
            
            var fontAssetPresets = Tree.RootProperty.Children["_fontAssetPresets"];
            var textPropertiesPresets = Tree.RootProperty.Children["_textPropertiesPresets"];
            var defaultFontAssetPresetId = Tree.RootProperty.Children["_defaultFontAssetPresetId"];
            var defaultTextPropertiesPresetId = Tree.RootProperty.Children["_defaultTextPropertiesPresetId"];
            
            EasyEditorGUI.Title("字体资产");
            fontAssetPresets.Draw(new GUIContent("字体资产预设表"));
            EasyEditorGUI.Title("文本属性");
            textPropertiesPresets.Draw(new GUIContent("文本属性预设表"));
            
            EasyEditorGUI.Title("默认预设");

            var lbl = defaultFontAssetPresetId.WeakSmartValue<string>();
            EasyEditorGUI.DrawSelectorDropdown(new SelectorDropdownConfig<string>(
                EditorHelper.TempContent("默认字体资产预设"),
                EditorHelper.TempContent2(lbl),
                fontAssetPresets.WeakSmartValue<List<FontAssetPreset>>().Select(p => p.Id),
                id => defaultFontAssetPresetId.SetWeakSmartValue(id)));

            lbl = defaultTextPropertiesPresetId.WeakSmartValue<string>();
            EasyEditorGUI.DrawSelectorDropdown(new SelectorDropdownConfig<string>(
                EditorHelper.TempContent("默认文本属性预设"),
                EditorHelper.TempContent2(lbl),
                textPropertiesPresets.WeakSmartValue<List<TextPropertiesPreset>>().Select(p => p.Id),
                id => defaultTextPropertiesPresetId.SetWeakSmartValue(id)));

            Tree.EndDraw();
        }
    }

    public class FontAssetPresetDrawer : OdinValueDrawer<FontAssetPreset>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            
            var id = Property.Children["Id"];
            var fontAsset = Property.Children["FontAsset"];
            var material = Property.Children["Material"];

            id.Draw(new GUIContent("标识"));
            fontAsset.Draw(new GUIContent("字体资产"));
            material.Draw(new GUIContent("材质"));

            fontAsset.ValueEntry.OnValueChanged += i =>
            {
                material.SetWeakSmartValue(fontAsset.WeakSmartValue<TMP_Asset>()?.material);
            };
        }
    }

    public class TextPropertiesPresetDrawer : OdinValueDrawer<TextPropertiesPreset>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var id = Property.Children["Id"];
            var fontSize = Property.Children["FontSize"];
            var fontColor = Property.Children["FontColor"];

            id.Draw(new GUIContent("标识"));
            fontSize.Draw(new GUIContent("字体大小"));
            fontColor.Draw(new GUIContent("字体颜色"));
        }
    }
}
