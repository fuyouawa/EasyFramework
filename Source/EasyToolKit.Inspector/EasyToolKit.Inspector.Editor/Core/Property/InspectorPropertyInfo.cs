using System;
using System.Reflection;
using EasyToolKit.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public sealed class InspectorPropertyInfo
    {
        [CanBeNull] public IValueAccessor ValueAccessor { get; private set; }
        public MemberInfo MemberInfo { get; private set; }
        public Type PropertyType { get; private set; }
        public string PropertyPath { get; private set; }
        public string PropertyName { get; private set; }
        public bool AllowChildren { get; private set; }
        public bool IsLogicRoot { get; private set; }
        [CanBeNull] public Type DefaultPropertyResolverType { get; private set; }
        [CanBeNull] public Type DefaultDrawerChainResolverType { get; private set; }
        [CanBeNull] public Type DefaultAttributeAccessorResolver { get; private set; }

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
                PropertyType = fieldInfo.FieldType,
                PropertyPath = serializedProperty.propertyPath,
                PropertyName = serializedProperty.name,
                DefaultPropertyResolverType = typeof(UnityPropertyResolver),
                DefaultDrawerChainResolverType = typeof(DefaultDrawerChainResolver),
                DefaultAttributeAccessorResolver = typeof(DefaultAttributeAccessorResolver),
            };
            
            var accessorType = typeof(MemberValueAccessor<,>)
                .MakeGenericType(parentType, fieldInfo.FieldType);
            info.ValueAccessor = accessorType.CreateInstance<IValueAccessor>(fieldInfo);

            var isDefinedUnityPropertyDrawer = DrawerUtility.IsDefinedUnityPropertyDrawer(info.PropertyType);
            info.AllowChildren = info.PropertyType != null &&
                                 !info.PropertyType.IsBasic() &&
                                 !info.PropertyType.IsSubclassOf(typeof(UnityEngine.Object)) &&
                                 info.PropertyType.GetCustomAttribute<SerializableAttribute>() != null &&
                                 !isDefinedUnityPropertyDrawer;
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
                DefaultPropertyResolverType = typeof(UnityPropertyResolver),
                AllowChildren = true,
                IsLogicRoot = true,
            };

            info.ValueAccessor = new GenericValueAccessor(
                typeof(int),
                info.PropertyType,
                (ref object index) => serializedObject.targetObjects[(int)index],
                null);

            return info;
        }
    }
}
