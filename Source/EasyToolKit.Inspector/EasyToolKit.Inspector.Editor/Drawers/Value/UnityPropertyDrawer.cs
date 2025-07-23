using System;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Attribute - 1)]
    public class UnityPropertyDrawer : EasyDrawer
    {
        protected override bool CanDrawProperty(InspectorProperty property)
        {
            if (!property.Info.IsUnityProperty)
            {
                return false;
            }

            var propertyType = property.Info.PropertyType;
            var unityProperty = property.Tree.GetUnityPropertyByPath(property.Info.PropertyPath);
            if (propertyType == null || unityProperty == null)
            {
                return false;
            }

            return !propertyType.IsSubclassOf(typeof(UnityEngine.Object)) && InspectorDrawerUtility.IsDefinedUnityPropertyDrawer(propertyType);
        }
        
        private SerializedProperty _serializedProperty;

        protected override void Initialize()
        {
            _serializedProperty = Property.Tree.GetUnityPropertyByPath(Property.Info.PropertyPath);
        }

        protected override void DrawProperty(GUIContent label)
        {
            if (_serializedProperty == null)
            {
                EditorGUILayout.LabelField(label, InspectorDrawerUtility.NotSupportedContent);
                return;
            }
            EditorGUILayout.PropertyField(_serializedProperty, label);
        }
    }
}
