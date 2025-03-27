using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Editor
{
    public class GradientColorDrawer : OdinValueDrawer<GradientColor>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var val = ValueEntry.SmartValue;

            EditorGUILayout.BeginHorizontal();

            if (val.IsGradient)
                val.Gradient = EditorGUILayout.GradientField(label, val.Gradient);
            else
                val.Color = EditorGUILayout.ColorField(label, val.Color);

            var content = val.IsGradient
                ? new GUIContent("C", "Color")
                : new GUIContent("G", "Gradient");

            if (GUILayout.Button(content, GUILayout.Width(EditorGUIUtility.singleLineHeight + 4)))
            {
                val.IsGradient = !val.IsGradient;
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
