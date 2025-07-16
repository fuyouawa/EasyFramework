using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class Vector2Drawer : EasyValueDrawer<Vector2>
    {
        protected override void OnDrawProperty(GUIContent label)
        {
            ValueEntry.SmartValue = EditorGUILayout.Vector2Field(label, ValueEntry.SmartValue);
        }
    }
}
