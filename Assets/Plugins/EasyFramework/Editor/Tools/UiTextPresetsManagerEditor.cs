using System.Reflection;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.Drawers;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace EasyGameFramework.Editor
{
    [CustomEditor(typeof(UiTextPresetsManager))]
    public class UiTextPresetsManagerEditor : OdinEditor
    {
        // public override void OnInspectorGUI()
        // {
        //
        // }
    }

    public class FontAssetPresetDrawer : OdinValueDrawer<FontAssetPreset>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            EditorGUI.BeginChangeCheck();

            var fontAsset = Property.Children["FontAsset"];
            var material = Property.Children["Material"];

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
            Property.Children["FontSize"].Draw(new GUIContent("字体大小"));
            Property.Children["FontColor"].Draw(new GUIContent("字体颜色"));
        }
    }

    public class FontAssetPresetDictionaryDrawer : DictionaryDrawer<FontAssetPresetDictionary, string, FontAssetPreset>
    {
        protected override void Initialize()
        {
            base.Initialize();

            var type = GetType().BaseType!;

            type.GetField("keyLabel", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(this, new GUIContent("标识"));
            type.GetField("valueLabel", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(this, new GUIContent("预设设置"));
        }
    }

    public class TextPropertiesPresetDictionaryDrawer : DictionaryDrawer<TextPropertiesPresetDictionary, string, TextPropertiesPreset>
    {
        protected override void Initialize()
        {
            base.Initialize();

            var type = GetType().BaseType!;

            type.GetField("keyLabel", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(this, new GUIContent("标识"));
            type.GetField("valueLabel", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(this, new GUIContent("预设设置"));
        }
    }
}
