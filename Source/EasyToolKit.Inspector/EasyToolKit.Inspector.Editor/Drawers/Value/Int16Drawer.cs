using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class Int16Drawer : EasyValueDrawer<short>
    {
        protected override void DrawProperty(GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            var value = EditorGUILayout.IntField(label, ValueEntry.SmartValue);

            if (value < short.MinValue)
            {
                value = short.MinValue;
            }
            else if (value > short.MaxValue)
            {
                value = short.MaxValue;
            }

            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = (short)value;
            }
        }
    }
}
