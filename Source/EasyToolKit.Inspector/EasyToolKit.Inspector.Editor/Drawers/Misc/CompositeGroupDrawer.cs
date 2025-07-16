using System;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public static class CompositeGroupDrawer
    {
        public static bool HasDrawnGroup;
    }

    [DrawerPriority(2)]
    public class CompositeGroupDrawer<T> : EasyValueDrawer<T>
    {
        protected override bool CanDrawValueType(Type valueType)
        {
            return !valueType.IsSubclassOf(typeof(UnityEngine.Object));
        }

        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            return property.Children != null && property.Children.Count > 0;
        }

        protected override void OnDrawProperty(GUIContent label)
        {
            if (CompositeGroupDrawer.HasDrawnGroup)
            {
                CallNextDrawer(label);
                return;
            }

            Property.State.Expanded = EditorGUILayout.Foldout(Property.State.Expanded, label, true);
            if (Property.State.Expanded)
            {
                CallNextDrawer(label);
            }
        }
    }
}
