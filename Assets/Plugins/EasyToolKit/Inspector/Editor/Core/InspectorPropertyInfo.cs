using System;
using System.Reflection;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    public sealed class InspectorPropertyInfo
    {
        public IValueAccessor ValueAccessor { get; private set; }
        public MemberInfo MemberInfo { get; private set; }
        public Type PropertyType { get; private set; }

        public SerializedProperty SerializedProperty { get; private set; }

        private InspectorPropertyInfo() {}

        public static InspectorPropertyInfo CreateForUnityProperty(SerializedProperty serializedProperty, Type parentType)
        {
            var ownerType = serializedProperty.serializedObject.targetObject.GetType();
            var accessorType = typeof(SerializedPropertyValueAccessor<,>)
                .MakeGenericType(ownerType, serializedProperty.GetPropertyType());

            var field = parentType.GetField(serializedProperty.name, BindingFlagsHelper.AllInstance());
            Assert.True(field != null);

            var info = new InspectorPropertyInfo()
            {
                MemberInfo = field,
                PropertyType = serializedProperty.GetPropertyType(),
                SerializedProperty = serializedProperty.Copy(),
                ValueAccessor = accessorType.CreateInstance<IValueAccessor>()
            };
            return info;
        }

        public static InspectorPropertyInfo CreateForMember(MemberInfo memberInfo, IValueAccessor valueAccessor)
        {
            var info = new InspectorPropertyInfo
            {
                ValueAccessor = valueAccessor,
                MemberInfo = memberInfo
            };
            if (memberInfo is FieldInfo fieldInfo)
            {
                info.PropertyType = fieldInfo.FieldType;
            }
            else if (memberInfo is PropertyInfo propertyInfo)
            {
                info.PropertyType = propertyInfo.PropertyType;
            }
            return info;
        }
    }
}
