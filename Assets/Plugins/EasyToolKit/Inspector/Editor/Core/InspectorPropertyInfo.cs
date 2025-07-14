using System;
using System.Reflection;
using EasyToolKit.Core;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public sealed class InspectorPropertyInfo
    {
        public IValueAccessor ValueAccessor { get; private set; }
        public MemberInfo MemberInfo { get; private set; }
        public Type PropertyType { get; private set; }
        public string PropertyPath { get; private set; }
        public string PropertyName { get; private set; }

        private InspectorPropertyInfo() {}

        public static InspectorPropertyInfo CreateForUnityProperty(SerializedProperty serializedProperty, Type parentType)
        {
            var ownerType = serializedProperty.serializedObject.targetObject.GetType();

            var field = parentType.GetField(serializedProperty.name, BindingFlagsHelper.AllInstance());
            Assert.True(field != null);

            var accessorType = typeof(SerializedPropertyValueAccessor<,>)
                .MakeGenericType(ownerType, field.FieldType);

            var info = new InspectorPropertyInfo()
            {
                MemberInfo = field,
                PropertyType = field.FieldType,
                PropertyPath = serializedProperty.propertyPath,
                ValueAccessor = accessorType.CreateInstance<IValueAccessor>(serializedProperty.Copy()),
                PropertyName = serializedProperty.name
            };
            return info;
        }
    }
}
