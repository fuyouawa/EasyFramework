using System;
using System.Reflection;

namespace EasyFramework.Serialization
{
    internal static class ReflectionUtility
    {
        public static Type GetMemberType(this MemberInfo member)
        {
            if (member is FieldInfo field)
            {
                return field.FieldType;
            }

            if (member is PropertyInfo property)
            {
                return property.PropertyType;
            }

            throw new NotImplementedException();
        }
    }
}
