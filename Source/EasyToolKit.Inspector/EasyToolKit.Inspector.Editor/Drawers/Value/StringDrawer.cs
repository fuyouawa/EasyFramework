using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class StringDrawer : EasyValueDrawer<string>
    {
        protected override void OnDrawProperty(GUIContent label)
        {
            var value = ValueEntry.SmartValue;
            EditorGUI.BeginChangeCheck();
            value = label == null ?
                EditorGUILayout.TextField(value) :
                EditorGUILayout.TextField(label, value);
            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = value;
            }
        }
    }
}
