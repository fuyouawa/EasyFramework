using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class Vector4Drawer : EasyValueDrawer<Vector4>
    {
        protected override void OnDrawProperty(GUIContent label)
        {
            ValueEntry.SmartValue = EditorGUILayout.Vector4Field(label, ValueEntry.SmartValue);
        }
    }
}
