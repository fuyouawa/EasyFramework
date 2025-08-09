using System;
using System.Reflection;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public static class InspectorPropertyInfoUtility
    {
        public static bool IsUnityObjectTypeOrDefinedUnityPropertyDrawer(Type type)
        {
            if (type.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                return true;
            }
            var isDefinedUnityPropertyDrawer = InspectorDrawerUtility.IsDefinedUnityPropertyDrawer(type);
            return isDefinedUnityPropertyDrawer;
        }

        public static bool IsSerializableType(Type type)
        {
            return type.IsBasic() ||
                   type.IsUnityBuiltInType() ||
                   type.IsInheritsFrom<UnityEngine.Object>() ||
                   type.HasCustomAttribute<SerializableAttribute>();
        }

        public static bool IsSerializableField(FieldInfo fieldInfo)
        {
            if (!IsSerializableType(fieldInfo.FieldType))
            {
                return false;
            }

            var nonSerialized = fieldInfo.HasCustomAttribute<NonSerializedAttribute>();
            if (fieldInfo.IsPublic && !nonSerialized)
            {
                return true;
            }

            return !nonSerialized && fieldInfo.HasCustomAttribute<SerializeField>();
        }

        public static bool IsAllowChildrenType(Type type)
        {
            return !type.IsBasic() &&
                !IsUnityObjectTypeOrDefinedUnityPropertyDrawer(type) &&
                type.HasCustomAttribute<SerializableAttribute>();
        }

        public static bool IsAllowChildrenField(FieldInfo fieldInfo)
        {
            if (fieldInfo.FieldType.IsBasic() ||
                IsUnityObjectTypeOrDefinedUnityPropertyDrawer(fieldInfo.FieldType))
            {
                return false;
            }

            if (fieldInfo.HasCustomAttribute<ShowInInspectorAttribute>())
            {
                return true;
            }

            return IsSerializableField(fieldInfo);
        }
    }
}
