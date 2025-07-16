using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class UnityObjectDrawer<T> : EasyValueDrawer<T>
        where T : UnityEngine.Object
    {
        protected override void OnDrawProperty(GUIContent label)
        {
            ValueEntry.SmartValue = (T)EditorGUILayout.ObjectField(
                label,
                ValueEntry.SmartValue,
                typeof(T),
                Property.GetAttribute<AssetsOnlyAttribute>() == null);
        }
    }
}
