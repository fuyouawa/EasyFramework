using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Utilities.Editor
{
    public class FlexibleColorDrawer : OdinValueDrawer<FlexibleColor>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var val = ValueEntry.SmartValue;

            EditorGUILayout.BeginHorizontal();

            if (val.IsOvertime)
            {
                val.OvertimeOfGradient = EditorGUILayout.GradientField(label, val.OvertimeOfGradient);
            }
            else
            {
                val.InstantOfColor = EditorGUILayout.ColorField(label, val.InstantOfColor);
            }

            var content = val.IsOvertime
                ? new GUIContent("I", "切换到立即数模式（Instant）")
                : new GUIContent("O", "切换到随时间变化模式（Overtime）");

            if (GUILayout.Button(content, GUILayout.Width(EditorGUIUtility.singleLineHeight + 4)))
            {
                val.Mode = val.IsOvertime
                    ? FlexibleModes.Instant
                    : FlexibleModes.Overtime;
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
