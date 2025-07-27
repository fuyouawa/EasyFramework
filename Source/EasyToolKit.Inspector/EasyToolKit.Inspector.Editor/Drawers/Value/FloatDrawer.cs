using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class FloatDrawer : EasyValueDrawer<float>
    {
        protected override void DrawProperty(GUIContent label)
        {
            var value = ValueEntry.SmartValue;
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.FloatField(label, value);

            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = value;
            }
        }
    }
}
