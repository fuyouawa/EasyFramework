using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(2)]
    public class UnityPropertyDrawer : EasyDrawer
    {
        private SerializedProperty _serializedProperty;

        public override bool CanDrawProperty(InspectorProperty property)
        {
            var serializedProperty = property.TryGetUnitySerializedProperty();
            return serializedProperty != null;
        }
        
        protected override void Initialize()
        {
            _serializedProperty = Property.TryGetUnitySerializedProperty();
        }

        protected override void OnDrawProperty(GUIContent label)
        {
            if (EditorGUILayout.PropertyField(_serializedProperty, label, false))
            {
                CallNextDrawer(label);
            }
        }
    }
}
