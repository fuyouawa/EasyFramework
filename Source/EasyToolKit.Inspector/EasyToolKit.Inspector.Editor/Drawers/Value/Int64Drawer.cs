using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class Int64Drawer : EasyValueDrawer<long>
    {
        protected override void DrawProperty(GUIContent label)
        {
            var value = ValueEntry.SmartValue;
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.LongField(label, value);
            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = value;
            }
        }
    }
}
