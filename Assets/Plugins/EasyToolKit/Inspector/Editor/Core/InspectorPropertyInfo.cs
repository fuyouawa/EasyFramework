using System;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    public sealed class InspectorPropertyInfo
    {
        public IInspectorValueAccessor ValueAccessor { get; private set; }
        public SerializedProperty SerializedProperty { get; private set; }

        public static InspectorPropertyInfo CreateForUnityProperty(SerializedProperty serializedProperty)
        {
            var ownerType = serializedProperty.serializedObject.targetObject.GetType();
            var accessorType = typeof(UnityInspectorValueAccessor<,>)
                .MakeGenericType(ownerType, serializedProperty.GetPropertyType());

            var info = new InspectorPropertyInfo
            {
                SerializedProperty = serializedProperty.Copy(),
                ValueAccessor = (IInspectorValueAccessor)accessorType.CreateInstance()
            };
            return info;
        }
    }
}
