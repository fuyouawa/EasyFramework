using System;
using System.Reflection;

namespace EasyFramework.Serialization
{
    internal static class ReflectionUtility
    {
        public static object GetMemberValue(this MemberInfo member, object target)
        {
            if (member is FieldInfo field)
            {
                return field.GetValue(target);
            }

            if (member is PropertyInfo property)
            {
                if (!property.CanRead)
                    throw new ArgumentException($"The member '{property.Name}' cannot get a value.");
                return property.GetValue(target);
            }

            throw new NotImplementedException();
        }

        public static void SetMemberValue(this MemberInfo member, object target, object value)
        {
            if (member is FieldInfo field)
            {
                field.SetValue(target, value);
                return;
            }

            if (member is PropertyInfo property)
            {
                if (!property.CanWrite)
                    throw new ArgumentException($"The member '{property.Name}' cannot set a value.");
                property.SetValue(target, value);
                return;
            }

            throw new NotImplementedException();
        }

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
