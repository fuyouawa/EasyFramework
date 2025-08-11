using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class ColorDrawer : EasyValueDrawer<Color>
    {
        protected override void DrawProperty(GUIContent label)
        {
            var value = ValueEntry.SmartValue;
            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.ColorField(label, value);
            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = value;
            }
        }
    }
}
