using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(2)]
    public class UnityPropertyDrawer : EasyDrawer
    {
        public override bool CanDrawProperty(InspectorProperty property)
        {
            var unityProperty = property.TryGetUnitySerializedProperty();
            return unityProperty != null && unityProperty.propertyType != SerializedPropertyType.Generic;
        }

        protected override void OnDrawProperty(GUIContent label)
        {
            EditorGUILayout.PropertyField(Property.TryGetUnitySerializedProperty(), label);
            CallNextDrawer(label);
        }
    }
}
