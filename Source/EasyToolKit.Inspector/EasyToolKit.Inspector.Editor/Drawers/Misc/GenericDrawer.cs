using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class GenericDrawer<T> : EasyValueDrawer<T>
    {
        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            return property.Children != null && property.Children.Count > 0;
        }

        protected override void OnDrawProperty(GUIContent label)
        {
            Property.State.Expanded = EditorGUILayout.Foldout(Property.State.Expanded, label, true);
        }
    }
}
