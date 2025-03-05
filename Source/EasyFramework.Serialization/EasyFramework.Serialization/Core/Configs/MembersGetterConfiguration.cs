using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using UnityEngine;

namespace EasyFramework.Serialization
{
    [Flags]
    public enum FieldGetterFlags
    {
        None = 0,
        NeedSerializeFieldIfNonPublic = 1 << 0,
    }

    [Flags]
    public enum PropertyGetterFlags
    {
        None = 0,
        Gettable = 1 << 0,
        Settable = 1 << 1,
    }

    public class MembersGetterConfiguration
    {

        private BindingFlags? _fieldBindingFlags;
        private BindingFlags? _propertyBindingFlags;
        private FieldGetterFlags _fieldGetterFlags;
        private PropertyGetterFlags _propertyGetterFlags;
        private readonly List<Type> _excludeTypes = new List<Type>();

        public FlagsConfigurationOfMembersGetter<BindingFlags> FieldBindingFlags { get; }
        public FlagsConfigurationOfMembersGetter<BindingFlags> PropertyBindingFlags { get; }
        public FlagsConfigurationOfMembersGetter<FieldGetterFlags> FieldGetterFlags { get; }
        public FlagsConfigurationOfMembersGetter<PropertyGetterFlags> PropertyGetterFlags { get; }
        public ExcludeTypesConfigurationOfMembersGetter ExcludeTypes { get; }

        public MembersGetterConfiguration()
        {
            FieldBindingFlags = new FlagsConfigurationOfMembersGetter<BindingFlags>(
                this, flags => _fieldBindingFlags = flags, () => _fieldBindingFlags);
            PropertyBindingFlags = new FlagsConfigurationOfMembersGetter<BindingFlags>(
                this, flags => _propertyBindingFlags = flags, () => _propertyBindingFlags);

            FieldGetterFlags = new FlagsConfigurationOfMembersGetter<FieldGetterFlags>(
                this, flags => _fieldGetterFlags = flags, () => _fieldGetterFlags);
            PropertyGetterFlags = new FlagsConfigurationOfMembersGetter<PropertyGetterFlags>(
                this, flags => _propertyGetterFlags = flags, () => _propertyGetterFlags);

            ExcludeTypes = new ExcludeTypesConfigurationOfMembersGetter(this, _excludeTypes);
        }

        public MembersGetterDelegate CreateGetter() => type => InternalMemberGetter.Invoke(
            type,
            _fieldBindingFlags, _fieldGetterFlags,
            _propertyBindingFlags, _propertyGetterFlags,
            _excludeTypes.ToArray());
    }

    internal class InternalMemberGetter
    {
        public static MemberInfo[] Invoke(
            Type targetType,
            BindingFlags? fieldBindingFlags,
            FieldGetterFlags fieldGetterFlags,
            BindingFlags? propertyBindingFlags,
            PropertyGetterFlags propertyGetterFlags,
            Type[] excludeTypes)
        {
            var total = new List<MemberInfo>();
            if (fieldBindingFlags.HasValue)
            {
                var needSerializeFieldIfNonPublic =
                    fieldGetterFlags.HasFlag(FieldGetterFlags.NeedSerializeFieldIfNonPublic);

                total.AddRange(targetType.GetFields(fieldBindingFlags.Value)
                    .Where(f =>
                    {
                        if (excludeTypes.Contains(f.FieldType))
                            return false;
                        if (!f.IsPublic && needSerializeFieldIfNonPublic)
                        {
                            if (f.GetCustomAttribute<SerializeField>() == null)
                                return false;
                        }

                        return true;
                    }));
            }

            if (propertyBindingFlags.HasValue)
            {
                var gettable = propertyGetterFlags.HasFlag(PropertyGetterFlags.Gettable);
                var settable = propertyGetterFlags.HasFlag(PropertyGetterFlags.Settable);

                total.AddRange(targetType.GetProperties(propertyBindingFlags.Value)
                    .Where(p =>
                    {
                        if (excludeTypes.Contains(p.PropertyType))
                            return false;
                        if (gettable && p.GetMethod == null)
                            return false;
                        if (settable && p.SetMethod == null)
                            return false;
                        return true;
                    }));
            }

            return total.ToArray();
        }
    }
}
