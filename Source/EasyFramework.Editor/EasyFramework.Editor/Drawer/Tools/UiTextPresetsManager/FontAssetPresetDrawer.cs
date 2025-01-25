using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Editor.Drawer
{
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
}
