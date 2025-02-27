using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class FlexibleFloatDrawer : OdinValueDrawer<FlexibleFloat>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var val = ValueEntry.SmartValue;

            EditorGUILayout.BeginHorizontal();

            if (val.Mode == FlexibleModes.Overtime)
            {
                val.OvertimeOfCurve = EditorGUILayout.CurveField(label, val.OvertimeOfCurve);

                var width = EditorGUIUtility.singleLineHeight * 2;

                EditorGUILayout.LabelField(
                    new GUIContent("x", "曲线的最小映射值"), GUILayout.Width(10));

                val.CurveMinValueRemap = EditorGUILayout.FloatField(
                    val.CurveMinValueRemap, GUILayout.Width(width));

                EditorGUILayout.LabelField(
                    new GUIContent("y", "曲线的最大映射值"), GUILayout.Width(10));

                val.CurveMaxValueRemap = EditorGUILayout.FloatField(
                    val.CurveMaxValueRemap, GUILayout.Width(width));
            }
            else
            {
                val.InstantOfValue = EditorGUILayout.FloatField(label, val.InstantOfValue);
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

            ValueEntry.SmartValue = val;
        }
    }
}
