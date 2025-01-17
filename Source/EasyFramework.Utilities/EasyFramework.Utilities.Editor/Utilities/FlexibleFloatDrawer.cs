using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Utilities.Editor
{
    public class FlexibleFloatDrawer : OdinValueDrawer<FlexibleFloat>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var val = ValueEntry.SmartValue;

            using (new EditorGUILayout.HorizontalScope())
            {
                if (val.IsCurve)
                {
                    val.Curve = EditorGUILayout.CurveField(label, val.Curve);

                    var width = EditorGUIUtility.singleLineHeight * 2;

                    EditorGUILayout.LabelField(new GUIContent("x", "曲线的最小映射值"), GUILayout.Width(10));
                    val.CurveMinValueRemap = EditorGUILayout.FloatField(val.CurveMinValueRemap, GUILayout.Width(width));

                    EditorGUILayout.LabelField(new GUIContent("y", "曲线的最大映射值"), GUILayout.Width(10));
                    val.CurveMaxValueRemap = EditorGUILayout.FloatField(val.CurveMaxValueRemap, GUILayout.Width(width));
                }
                else
                {
                    val.Value = EditorGUILayout.FloatField(label, val.Value);
                }

                var content = val.IsCurve
                    ? new GUIContent("F", "切换到数值模式")
                    : new GUIContent("C", "切换到曲线模式");

                if (GUILayout.Button(content, GUILayout.Width(EditorGUIUtility.singleLineHeight)))
                {
                    val.IsCurve = !val.IsCurve;
                }
            }

            ValueEntry.SmartValue = val;
        }
    }
}
