using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class Vector3Drawer : EasyValueDrawer<Vector3>
    {
        protected override void DrawProperty(GUIContent label)
        {
            var value = ValueEntry.SmartValue;
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.Vector3Field(label, value);
            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = value;
            }
        }
    }
}
