using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class UnityObjectDrawer<T> : EasyValueDrawer<T>
        where T : UnityEngine.Object
    {
        protected override void DrawProperty(GUIContent label)
        {
            var value = ValueEntry.SmartValue;
            EditorGUI.BeginChangeCheck();
            value = (T)EditorGUILayout.ObjectField(
                label,
                value,
                typeof(T),
                Property.GetAttribute<AssetsOnlyAttribute>() == null);
            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = value;
            }
        }
    }
}
