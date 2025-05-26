using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class CurveValueDrawer : OdinValueDrawer<CurveValue>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var val = ValueEntry.SmartValue;

            EditorGUILayout.BeginHorizontal();

            if (val.IsCurve)
            {
                val.Curve = EditorGUILayout.CurveField(label, val.Curve);

                var width = EditorGUIUtility.singleLineHeight * 2;

                EditorGUILayout.LabelField(
                    new GUIContent("x", "曲线的最小映射值"), GUILayout.Width(10));

                val.CurveValueRemap.x = EditorGUILayout.FloatField(
                    val.CurveValueRemap.x, GUILayout.Width(width));

                EditorGUILayout.LabelField(
                    new GUIContent("y", "曲线的最大映射值"), GUILayout.Width(10));

                val.CurveValueRemap.y = EditorGUILayout.FloatField(
                    val.CurveValueRemap.y, GUILayout.Width(width));
            }
            else
            {
                val.Value = EditorGUILayout.FloatField(label, val.Value);
            }

            var content = val.IsCurve
                ? new GUIContent("V", "Value")
                : new GUIContent("C", "Curve");

            if (GUILayout.Button(content, GUILayout.Width(EditorGUIUtility.singleLineHeight + 4)))
            {
                val.IsCurve = !val.IsCurve;
            }

            EditorGUILayout.EndHorizontal();

            ValueEntry.SmartValue = val;
        }
    }
}
