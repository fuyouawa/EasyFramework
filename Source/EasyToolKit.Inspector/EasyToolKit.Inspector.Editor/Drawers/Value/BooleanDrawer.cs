using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class BooleanDrawer : EasyValueDrawer<bool>
    {
        protected override void OnDrawProperty(GUIContent label)
        {
            ValueEntry.SmartValue = EditorGUILayout.Toggle(label, ValueEntry.SmartValue);
        }
    }
}
