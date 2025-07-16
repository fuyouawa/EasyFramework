using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace EasyToolKit.Inspector.Editor
{
    public class UnityEventDrawer<T> : EasyValueDrawer<T>
        where T : UnityEventBase
    {
        private SerializedProperty _serializedProperty;

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
