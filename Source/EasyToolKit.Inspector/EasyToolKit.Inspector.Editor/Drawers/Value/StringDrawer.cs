using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor.Value
{
    public class StringDrawer : EasyValueDrawer<string>
    {
        protected override void OnDrawProperty(GUIContent label)
        {
            ValueEntry.SmartValue = label == null ?
                EditorGUILayout.TextField(ValueEntry.SmartValue) :
                EditorGUILayout.TextField(label, ValueEntry.SmartValue);
        }
    }
}
