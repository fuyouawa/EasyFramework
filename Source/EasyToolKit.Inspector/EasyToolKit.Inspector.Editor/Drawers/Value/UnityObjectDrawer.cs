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

            bool assetsOnly = Property.GetAttribute<AssetsOnlyAttribute>() == null;
            if (label == null)
            {
                value = (T)EditorGUILayout.ObjectField(value, typeof(T), assetsOnly);
            }
            else
            {
                value = (T)EditorGUILayout.ObjectField(label, value, typeof(T), assetsOnly);
            }

            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = value;
            }
        }
    }
}
