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
        public Type DefaultPropertyResolverType { get; private set; }
        public Type DefaultDrawerChainResolverType { get; private set; }
        public Type DefaultAttributeAccessorResolver { get; private set; }

        private InspectorPropertyInfo() {}

        public static InspectorPropertyInfo CreateForUnityProperty(SerializedProperty serializedProperty, Type parentType)
        {
            var ownerType = serializedProperty.serializedObject.targetObject.GetType();

            var fieldInfo = parentType.GetField(serializedProperty.name, BindingFlagsHelper.AllInstance());
            Assert.True(fieldInfo != null);

            var accessorType = typeof(GenericValueAccessor<,>)
                .MakeGenericType(ownerType, fieldInfo.FieldType);
            var info = new InspectorPropertyInfo()
            {
                MemberInfo = fieldInfo,
                PropertyType = fieldInfo.FieldType,
                PropertyPath = serializedProperty.propertyPath,
                ValueAccessor = accessorType.CreateInstance<IValueAccessor>(fieldInfo),
                PropertyName = serializedProperty.name,
                DefaultPropertyResolverType = typeof(UnityPropertyResolver),
                DefaultDrawerChainResolverType = typeof(DefaultDrawerChainResolver),
                DefaultAttributeAccessorResolver = typeof(DefaultAttributeAccessorResolver)
            };
            return info;
        }
    }
}
