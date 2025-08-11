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
        [CanBeNull] private Type _propertyResolverType;
        private MemberInfo _memberInfo;
        private bool? _isArrayElement;
        private bool? _isAllowChildren;

        [CanBeNull] public IValueAccessor ValueAccessor { get; private set; }
        [CanBeNull] public Type PropertyType { get; private set; }
        public string PropertyPath { get; private set; }
        public string PropertyName { get; private set; }
        public bool IsLogicRoot { get; private set; }
        public bool IsUnityProperty { get; private set; }

        public bool IsAllowChildren
        {
            get
            {
                if (_isAllowChildren != null)
                {
                    return _isAllowChildren.Value;
                }

                _isAllowChildren = false;

                var memberInfo = TryGetMemberInfo();
                if (memberInfo != null)
                {
                    if (memberInfo is FieldInfo fieldInfo)
                    {
                        _isAllowChildren = InspectorPropertyInfoUtility.IsAllowChildrenField(fieldInfo);
                        return _isAllowChildren.Value;
                    }
                    else if (memberInfo is PropertyInfo propertyInfo)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        return false;
                    }
                }

                if (PropertyType != null)
                {
                    _isAllowChildren = InspectorPropertyInfoUtility.IsAllowChildrenType(PropertyType);
                }

                return _isAllowChildren.Value;
            }
        }

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
                Assert.IsTrue(serializedProperty.isArray);
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

        public static InspectorPropertyInfo CreateForMember(MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo fieldInfo)
            {
                return CreateForField(fieldInfo);
            }
            else if (memberInfo is PropertyInfo propertyInfo)
            {
                return CreateForProperty(propertyInfo);
            }
            else if (memberInfo is MethodInfo methodInfo)
            {
                return CreateForMethod(methodInfo);
            }
            throw new NotSupportedException($"Unsupported member type: {memberInfo.GetType()}");
        }

        public static InspectorPropertyInfo CreateForProperty(PropertyInfo propertyInfo)
        {
            throw new NotImplementedException();
        }

        public static InspectorPropertyInfo CreateForMethod(MethodInfo methodInfo)
        {
            var info = new InspectorPropertyInfo()
            {
                PropertyPath = methodInfo.Name + "()",
                PropertyName = methodInfo.Name,
                IsUnityProperty = false,
                _memberInfo = methodInfo
            };

            return info;
        }

        public static InspectorPropertyInfo CreateForField(FieldInfo fieldInfo)
        {
            var info = new InspectorPropertyInfo()
            {
                PropertyType = fieldInfo.FieldType,
                PropertyPath = fieldInfo.Name,
                PropertyName = fieldInfo.Name,
                IsUnityProperty = false,
                _memberInfo = fieldInfo
            };

            var accessorType = typeof(MemberValueAccessor<,>)
                .MakeGenericType(fieldInfo.DeclaringType, fieldInfo.FieldType);
            info.ValueAccessor = accessorType.CreateInstance<IValueAccessor>(fieldInfo);

            if (fieldInfo.FieldType.IsImplementsOpenGenericType(typeof(ICollection<>)))
            {
                var elementType = fieldInfo.FieldType.GetArgumentsOfInheritedOpenGenericType(typeof(ICollection<>))[0];

                if (fieldInfo.FieldType.IsImplementsOpenGenericType(typeof(IList<>)))
                {
                    info._propertyResolverType = typeof(ListResolver<>).MakeGenericType(elementType);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else
            {
                info._propertyResolverType = typeof(GenericPropertyResolver);
            }

            return info;
        }

        public static InspectorPropertyInfo CreateForValue(Type valueType, string valueName, IValueAccessor valueAccessor)
        {
            var info = new InspectorPropertyInfo()
            {
                PropertyType = valueType,
                PropertyPath = valueName,
                PropertyName = valueName,
                IsUnityProperty = false,
                ValueAccessor = valueAccessor
            };

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
                _isAllowChildren = true
            };

            info.ValueAccessor = new GenericValueAccessor(
                typeof(int),
                info.PropertyType,
                (ref object index) => serializedObject.targetObjects[(int)index],
                null);

            info._propertyResolverType = typeof(GenericPropertyResolver);

            return info;
        }

        public IPropertyResolver GetPreferencedChildrenResolver()
        {
            if (!IsAllowChildren)
            {
                return null;
            }

            if (_propertyResolverType == null)
            {
                return new GenericPropertyResolver();
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
            var fieldInfo = parentType.GetField(PropertyName, BindingFlagsHelper.AllInstance);
            if (fieldInfo != null)
            {
                return fieldInfo;
            }

            var propertyInfo = parentType.GetProperty(PropertyName, BindingFlagsHelper.AllInstance);
            if (propertyInfo != null)
            {
                return propertyInfo;
            }

            return null;
        }
    }
}
