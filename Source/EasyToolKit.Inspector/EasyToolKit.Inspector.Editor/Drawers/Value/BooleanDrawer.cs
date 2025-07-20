using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class BooleanDrawer : EasyValueDrawer<bool>
    {
        protected override void DrawProperty(GUIContent label)
        {
            var value = ValueEntry.SmartValue;
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.Toggle(label, value);
            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = value;
            }
        }
    }
}
