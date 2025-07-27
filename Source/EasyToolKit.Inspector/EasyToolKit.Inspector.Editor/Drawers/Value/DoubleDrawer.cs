using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class DoubleDrawer : EasyValueDrawer<double>
    {
        protected override void DrawProperty(GUIContent label)
        {
            var value = ValueEntry.SmartValue;
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.DoubleField(label, value);

            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = value;
            }
        }
    }
}
