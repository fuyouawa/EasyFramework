using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Editor
{
    public class SliderValueDrawer : OdinValueDrawer<SliderValue>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var val = ValueEntry.SmartValue;

            EditorGUILayout.BeginHorizontal();

            if (val.IsSlider)
            {
                val.Slider = SirenixEditorFields.MinMaxSlider(label, val.Slider, val.Limit, true);
            }
            else
                val.Value = EditorGUILayout.Slider(label, val.Value, val.Limit.x, val.Limit.y);

            var content = val.IsSlider
                ? new GUIContent("V", "Value")
                : new GUIContent("S", "Slider");

            if (GUILayout.Button(content, GUILayout.Width(EditorGUIUtility.singleLineHeight + 4)))
            {
                val.IsSlider = !val.IsSlider;
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
