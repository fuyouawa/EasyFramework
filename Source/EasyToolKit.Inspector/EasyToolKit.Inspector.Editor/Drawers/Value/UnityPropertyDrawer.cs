using System;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Attribute - 1)]
    public class UnityPropertyDrawer<T> : EasyValueDrawer<T>
    {
        private SerializedProperty _serializedProperty;

        protected override bool CanDrawValueType(Type valueType)
        {
            return !valueType.IsSubclassOf(typeof(UnityEngine.Object)) &&
                   !DrawerUtility.IsDefinedUnityPropertyDrawer(valueType);
        }

        protected override void Initialize()
        {
            _serializedProperty = Property.Tree.SerializedObject.FindProperty(Property.Info.PropertyPath);
        }

        protected override void OnDrawProperty(GUIContent label)
        {
            if (_serializedProperty == null)
            {
                EditorGUILayout.LabelField(label, DrawerUtility.NotSupportedContent);
                return;
            }

            EditorGUILayout.PropertyField(_serializedProperty, label);
        }
    }
}
