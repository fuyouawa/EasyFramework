using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor.Value
{
    public class Int32Drawer : EasyValueDrawer<int>
    {
        protected override void OnDrawProperty(GUIContent label)
        {
            ValueEntry.SmartValue = EditorGUILayout.IntField(label, ValueEntry.SmartValue);
        }
    }
}
