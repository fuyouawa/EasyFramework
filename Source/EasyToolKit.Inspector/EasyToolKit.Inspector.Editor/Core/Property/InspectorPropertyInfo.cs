using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using EasyToolKit.Core;
using JetBrains.Annotations;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    public sealed class InspectorPropertyInfo
    {
        private Type _propertyResolverType;
        private MemberInfo _memberInfo;
        private bool? _isArrayElement;

        [CanBeNull] public IValueAccessor ValueAccessor { get; private set; }
        [CanBeNull] public Type PropertyType { get; private set; }
        public string PropertyPath { get; private set; }
        public string PropertyName { get; private set; }
        public bool IsLogicRoot { get; private set; }
        public bool IsUnityProperty { get; private set; }

        private InspectorPropertyInfo()
        {
        }

        public bool IsArrayElement
        {
            get
            {
                if (_isArrayElement == null)
                {
                    if (ValueAccessor == null || !ValueAccessor.OwnerType.IsImplementsOpenGenericType(typeof(ICollection<>)))
                    {
                        _isArrayElement = false;
                    }
                    else
                    {
                        _isArrayElement = true;
                        //TODO IsArrayElement其他情况的补充
                    }
                }
                return _isArrayElement.Value;
            }
        }

        public static InspectorPropertyInfo CreateForUnityProperty(
            SerializedProperty serializedProperty,
            Type parentType, Type valueType)
        {
            var info = new InspectorPropertyInfo()
            {
                PropertyType = valueType,
                PropertyPath = serializedProperty.propertyPath,
                PropertyName = serializedProperty.name,
                IsUnityProperty = true
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
                var isDefinedUnityPropertyDrawer = InspectorDrawerUtility.IsDefinedUnityPropertyDrawer(PropertyType);
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

        public MemberInfo TryGetMemberInfo()
        {
            if (_memberInfo != null)
            {
                return _memberInfo;
            }

            if (ValueAccessor == null ||
                IsArrayElement ||
                PropertyName.IsNullOrEmpty())
            {
                return null;
            }

            var parentType = ValueAccessor.OwnerType;
            var fieldInfo = parentType.GetField(PropertyName, BindingFlagsHelper.AllInstance());
            if (fieldInfo != null)
            {
                return fieldInfo;
            }

            var propertyInfo = parentType.GetProperty(PropertyName, BindingFlagsHelper.AllInstance());
            if (propertyInfo != null)
            {
                return propertyInfo;
            }

            return null;
        }
    }
}
