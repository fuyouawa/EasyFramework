using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class Vector3Drawer : EasyValueDrawer<Vector3>
    {
        protected override void OnDrawProperty(GUIContent label)
        {
            ValueEntry.SmartValue = EditorGUILayout.Vector3Field(label, ValueEntry.SmartValue);
        }
    }
}
