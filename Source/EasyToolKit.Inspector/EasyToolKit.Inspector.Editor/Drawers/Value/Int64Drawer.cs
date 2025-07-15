using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor.Value
{
    public class Int64Drawer : EasyValueDrawer<long>
    {
        protected override void OnDrawProperty(GUIContent label)
        {
            ValueEntry.SmartValue = EditorGUILayout.LongField(label, ValueEntry.SmartValue);
        }
    }
}
