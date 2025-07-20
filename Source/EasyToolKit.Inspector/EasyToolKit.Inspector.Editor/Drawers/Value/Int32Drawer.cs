using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class Int32Drawer : EasyValueDrawer<int>
    {
        protected override void DrawProperty(GUIContent label)
        {
            var value = ValueEntry.SmartValue;
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.IntField(label, value);

            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = value;
            }
        }
    }
}
