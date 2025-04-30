using System;
using System.Linq;
using System.Reflection;

namespace EasyFramework.Serialization
{
    public class MemberFilterImpl
    {
        public static bool Invoke(MemberInfo member, MemberFilterFlags flags, Type[] excludeTypes)
        {
            bool allowPublic = flags.HasFlag(MemberFilterFlags.Public);
            bool allowNonPublic = flags.HasFlag(MemberFilterFlags.NonPublic);

            switch (member)
            {
                case FieldInfo field:
                {
                    if (!flags.HasFlag(MemberFilterFlags.Field))
                        return false;

                    if (excludeTypes?.Contains(field.FieldType) == true)
                        return false;

                    if (!flags.HasFlag(MemberFilterFlags.SerializeField))
                    {
                        if (field.IsPublic && !allowPublic) return false;
                        if (!field.IsPublic && !allowNonPublic) return false;
                    }

                    return true;
                }

                case PropertyInfo prop:
                {
                    if ((flags & MemberFilterFlags.AllProperty) == 0)
                        return false;

                    if (excludeTypes?.Contains(prop.PropertyType) == true)
                        return false;

                    var getter = prop.GetGetMethod(true);
                    var setter = prop.GetSetMethod(true);

                    bool canRead = getter != null;
                    bool canWrite = setter != null;

                    if (canRead)
                    {
                        if (getter.IsPublic && !allowPublic) return false;
                        if (!getter.IsPublic && !allowNonPublic) return false;
                    }

                    if (canWrite)
                    {
                        if (setter.IsPublic && !allowPublic) return false;
                        if (!setter.IsPublic && !allowNonPublic) return false;
                    }

                    if (canRead && !canWrite)
                        return flags.HasFlag(MemberFilterFlags.ReadOnlyProperty);
                    if (!canRead && canWrite)
                        return flags.HasFlag(MemberFilterFlags.WriteOnlyProperty);
                    if (canRead && canWrite)
                        return flags.HasFlag(MemberFilterFlags.ReadWriteProperty);

                    return false;
                }

                default:
                    return false;
            }
        }
    }
}
