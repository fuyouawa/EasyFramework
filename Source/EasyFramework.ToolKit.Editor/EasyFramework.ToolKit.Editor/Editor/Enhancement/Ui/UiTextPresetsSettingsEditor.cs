using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    [CustomEditor(typeof(UiTextPresetsSettings))]
    public class UiTextPresetsSettingsEditor : OdinEditor
    {
        private InspectorProperty _fontAssetPresets;
        private InspectorProperty _textPropertiesPresets;

        protected override void OnEnable()
        {
            base.OnEnable();
            _fontAssetPresets = Tree.RootProperty.Children["_fontAssetPresets"];
            _textPropertiesPresets = Tree.RootProperty.Children["_textPropertiesPresets"];

            var p = _fontAssetPresets.WeakSmartValue<SerializedDictionary<string, FontAssetPreset>>();
            p.DrawerSettings.KeyLabel = "标识";
            p.DrawerSettings.ValueLabel = "预设";

            var p2 = _textPropertiesPresets.WeakSmartValue<SerializedDictionary<string, TextPropertiesPreset>>();
            p2.DrawerSettings.KeyLabel = "标识";
            p2.DrawerSettings.ValueLabel = "预设";
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw(true);

            var mgr = (UiTextPresetsSettings)target;

            EasyEditorGUI.Title("字体资产");
            _fontAssetPresets.Draw(new GUIContent("字体资产预设表"));
            EasyEditorGUI.Title("文本属性");
            _textPropertiesPresets.Draw(new GUIContent("文本属性预设表"));

            EasyEditorGUI.Title("默认预设");

            EasyEditorGUI.DrawSelectorDropdown(
                mgr.FontAssetPresets.Keys,
                EditorHelper.TempContent("默认字体资产预设"),
                EditorHelper.TempContent2(mgr.DefaultFontAssetPresetId),
                id => mgr.DefaultFontAssetPresetId = id);

            EasyEditorGUI.DrawSelectorDropdown(
                mgr.TextPropertiesPresets.Keys,
                EditorHelper.TempContent("默认文本属性预设"),
                EditorHelper.TempContent2(mgr.DefaultTextPropertiesPresetId),
                id => mgr.DefaultTextPropertiesPresetId = id);

            Tree.EndDraw();
        }
    }
}
