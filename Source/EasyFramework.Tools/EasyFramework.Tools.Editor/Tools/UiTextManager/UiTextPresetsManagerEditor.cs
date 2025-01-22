using EasyFramework.Inspector;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
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

            foreach (var child in _fontAssetPresets.Children)
            {
                var attr = child.GetAttribute<DictionaryDrawerSettings>();
                if (attr != null)
                {
                    attr.KeyLabel = "标识";
                    attr.ValueLabel = "预设";
                    break;
                }
            }

            foreach (var child in _textPropertiesPresets.Children)
            {
                var attr = child.GetAttribute<DictionaryDrawerSettings>();
                if (attr != null)
                {
                    attr.KeyLabel = "标识";
                    attr.ValueLabel = "预设";
                    break;
                }
            }
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
}
