using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using EasyToolKit.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public sealed class InspectorPropertyInfo
    {
        [CanBeNull] public IValueAccessor ValueAccessor { get; private set; }
        [CanBeNull] public MemberInfo MemberInfo { get; private set; }
        [CanBeNull] public Type PropertyType { get; private set; }
        public string PropertyPath { get; private set; }
        public string PropertyName { get; private set; }
        public bool IsLogicRoot { get; private set; }
        public bool IsUnityProperty { get; private set; }

        private Type _propertyResolverType;

        public bool IsValueType => MemberInfo is FieldInfo || MemberInfo is PropertyInfo;

        private InspectorPropertyInfo()
        {
        }

        public static InspectorPropertyInfo CreateForUnityProperty(SerializedProperty serializedProperty,
            Type parentType, Type valueType)
        {
            var info = new InspectorPropertyInfo()
            {
                PropertyType = valueType,
                PropertyPath = serializedProperty.propertyPath,
                PropertyName = serializedProperty.name,
                IsUnityProperty = true,
            };

            if (valueType.IsImplementsOpenGenericType(typeof(ICollection<>)))
            {
                Assert.True(serializedProperty.isArray);
                var elementType = valueType.GetArgumentsOfInheritedOpenGenericType(typeof(ICollection<>))[0];

                var accessorType = typeof(UnityCollectionAccessor<,,>)
                    .MakeGenericType(parentType, valueType, elementType);
                info.ValueAccessor = accessorType.CreateInstance<IValueAccessor>(serializedProperty);
                info._propertyResolverType = typeof(UnityCollectionResolver<>).MakeGenericType(elementType);
            }
            else
            {
                try
                {
                    var accessorType = typeof(UnityPropertyAccessor<,>)
                        .MakeGenericType(parentType, valueType);
                    info.ValueAccessor = accessorType.CreateInstance<IValueAccessor>(serializedProperty);
                    info.ValueAccessor.GetWeakValue(FormatterServices.GetUninitializedObject(parentType));
                }
                catch (Exception e) //TODO 有的类型无法通过SerializedProperty获取
                {
                    info.ValueAccessor = null;
                }

                info._propertyResolverType = typeof(UnityPropertyResolver);
            }

            return info;
        }

        internal static InspectorPropertyInfo CreateForLogicRoot(SerializedObject serializedObject)
        {
            var iterator = serializedObject.GetIterator();

            var info = new InspectorPropertyInfo()
            {
                PropertyType = serializedObject.targetObject.GetType(),
                PropertyPath = iterator.propertyPath,
                PropertyName = iterator.name,
                IsLogicRoot = true,
            };

            info.ValueAccessor = new GenericValueAccessor(
                typeof(int),
                info.PropertyType,
                (ref object index) => serializedObject.targetObjects[(int)index],
                null);
            
            info._propertyResolverType = typeof(UnityPropertyResolver);

            return info;
        }

        public bool AllowChildren()
        {
            if (IsLogicRoot) return true;

            var allowChildren = false;
            if (PropertyType != null)
            {
                allowChildren = PropertyType != null &&
                                !PropertyType.IsBasic() &&
                                !PropertyType.IsSubclassOf(typeof(UnityEngine.Object)) &&
                                PropertyType.GetCustomAttribute<SerializableAttribute>() != null;
            }

            if (allowChildren)
            {
                var isDefinedUnityPropertyDrawer = DrawerUtility.IsDefinedUnityPropertyDrawer(PropertyType);
                allowChildren = !isDefinedUnityPropertyDrawer;
            }

            return allowChildren;
        }

        public IPropertyResolver GetPreferencedChildrenResolver()
        {
            if (!AllowChildren())
            {
                return null;
            }

            return _propertyResolverType.CreateInstance<IPropertyResolver>();
        }
    }
}
