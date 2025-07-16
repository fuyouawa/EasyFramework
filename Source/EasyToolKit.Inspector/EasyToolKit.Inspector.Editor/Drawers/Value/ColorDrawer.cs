using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class ColorDrawer : EasyValueDrawer<Color>
    {
        protected override void OnDrawProperty(GUIContent label)
        {
            ValueEntry.SmartValue = EditorGUILayout.ColorField(label, ValueEntry.SmartValue);
        }
    }
}
