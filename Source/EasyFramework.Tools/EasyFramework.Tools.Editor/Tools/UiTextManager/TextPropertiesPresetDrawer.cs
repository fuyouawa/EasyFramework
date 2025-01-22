using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.Tools.Editor
{
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
