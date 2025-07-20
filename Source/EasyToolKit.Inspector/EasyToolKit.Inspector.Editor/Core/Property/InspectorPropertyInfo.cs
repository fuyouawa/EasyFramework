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
    public enum PropertyType
    {
        Value,
        Method
    }

    public sealed class InspectorPropertyInfo
    {
        public IValueAccessor ValueAccessor { get; private set; }
        public MemberInfo MemberInfo { get; private set; }
        public PropertyType PropertyType { get; private set; }
        [CanBeNull] public Type TypeOfProperty { get; private set; }
        public string PropertyPath { get; private set; }
        public string PropertyName { get; private set; }
        public bool AllowChildren { get; private set; }
        public bool IsLogicRoot { get; private set; }

        [CanBeNull] public IPropertyResolver DefaultChildrenResolver { get; private set; }
        [CanBeNull] public IDrawerChainResolver DefaultDrawerChainResolver { get; private set; }
        [CanBeNull] public IAttributeAccessorResolver DefaultAttributeAccessorResolver { get; private set; }

        private InspectorPropertyInfo()
        {
        }

        public static InspectorPropertyInfo CreateForUnityProperty(SerializedProperty serializedProperty,
            Type parentType)
        {
            var fieldInfo = parentType.GetField(serializedProperty.name, BindingFlagsHelper.AllInstance());
            Assert.True(fieldInfo != null);


            var info = new InspectorPropertyInfo()
            {
                MemberInfo = fieldInfo,
                TypeOfProperty = fieldInfo.FieldType,
                PropertyPath = serializedProperty.propertyPath,
                PropertyName = serializedProperty.name,
                DefaultDrawerChainResolver = new DefaultDrawerChainResolver(),
                DefaultAttributeAccessorResolver = new DefaultAttributeAccessorResolver(),
                PropertyType = PropertyType.Value
            };

            var isDefinedUnityPropertyDrawer = DrawerUtility.IsDefinedUnityPropertyDrawer(info.TypeOfProperty);
            info.AllowChildren = info.TypeOfProperty != null &&
                                 !info.TypeOfProperty.IsBasic() &&
                                 !info.TypeOfProperty.IsSubclassOf(typeof(UnityEngine.Object)) &&
                                 info.TypeOfProperty.GetCustomAttribute<SerializableAttribute>() != null &&
                                 !isDefinedUnityPropertyDrawer;

            if (info.TypeOfProperty.IsImplementsOpenGenericType(typeof(ICollection<>)))
            {
                Assert.True(serializedProperty.isArray);
                var arrayElementType = info.TypeOfProperty.GetArgumentsOfInheritedOpenGenericType(typeof(ICollection<>))[0];

                var accessorType = typeof(UnityCollectionAccessor<,,>)
                    .MakeGenericType(parentType, info.TypeOfProperty, arrayElementType);
                info.ValueAccessor = accessorType.CreateInstance<IValueAccessor>(serializedProperty);

                if (info.AllowChildren)
                {
                    info.DefaultChildrenResolver = new UnityCollectionResolver(arrayElementType);
                }
            }
            else
            {
                try
                {
                    var accessorType = typeof(UnityPropertyAccessor<,>)
                        .MakeGenericType(parentType, fieldInfo.FieldType);
                    info.ValueAccessor = accessorType.CreateInstance<IValueAccessor>(serializedProperty);
                    info.ValueAccessor.GetWeakValue(FormatterServices.GetUninitializedObject(parentType));
                }
                catch (Exception e) //TODO 有的unity类型无法通过SerializedProperty获取，这里先用个通过反射直接获取的
                {
                    var accessorType = typeof(MemberValueAccessor<,>)
                        .MakeGenericType(parentType, fieldInfo.FieldType);
                    info.ValueAccessor = accessorType.CreateInstance<IValueAccessor>(fieldInfo);
                }

                if (info.AllowChildren)
                {
                    info.DefaultChildrenResolver = new UnityPropertyResolver();
                }
            }

            return info;
        }

        public static InspectorPropertyInfo CreateForUnityArrayElement(SerializedProperty serializedProperty, int index, Type collectionType, Type elementType)
        {
            var info = new InspectorPropertyInfo()
            {
                TypeOfProperty = elementType,
                PropertyPath = serializedProperty.propertyPath,
                PropertyName = serializedProperty.name,
                DefaultDrawerChainResolver = new DefaultDrawerChainResolver(),
                PropertyType = PropertyType.Value
            };

            info.ValueAccessor = new GenericValueAccessor(
                collectionType,
                elementType,
                (ref object collection) => ((IList)collection)[index],
                (ref object collection, object element) => ((IList)collection)[index] = element);

            //TODO 优化重复代码
            var isDefinedUnityPropertyDrawer = DrawerUtility.IsDefinedUnityPropertyDrawer(info.TypeOfProperty);
            info.AllowChildren = info.TypeOfProperty != null &&
                                 !info.TypeOfProperty.IsBasic() &&
                                 !info.TypeOfProperty.IsSubclassOf(typeof(UnityEngine.Object)) &&
                                 info.TypeOfProperty.GetCustomAttribute<SerializableAttribute>() != null &&
                                 !isDefinedUnityPropertyDrawer;
            
            if (info.AllowChildren)
            {
                if (info.TypeOfProperty.IsImplementsOpenGenericType(typeof(ICollection<>)))
                {
                    var arguments = info.TypeOfProperty.GetArgumentsOfInheritedOpenGenericType(typeof(ICollection<>));
                    info.DefaultChildrenResolver = new UnityCollectionResolver(arguments[0]);
                }
                else
                {
                    info.DefaultChildrenResolver = new UnityPropertyResolver();
                }
            }

            return info;
        }

        internal static InspectorPropertyInfo CreateForLogicRoot(SerializedObject serializedObject)
        {
            var iterator = serializedObject.GetIterator();

            var info = new InspectorPropertyInfo()
            {
                TypeOfProperty = serializedObject.targetObject.GetType(),
                PropertyPath = iterator.propertyPath,
                PropertyName = iterator.name,
                DefaultChildrenResolver = new UnityPropertyResolver(),
                AllowChildren = true,
                IsLogicRoot = true,
                PropertyType = PropertyType.Value
            };

            info.ValueAccessor = new GenericValueAccessor(
                typeof(int),
                info.TypeOfProperty,
                (ref object index) => serializedObject.targetObjects[(int)index],
                null);

            return info;
        }
    }
}
