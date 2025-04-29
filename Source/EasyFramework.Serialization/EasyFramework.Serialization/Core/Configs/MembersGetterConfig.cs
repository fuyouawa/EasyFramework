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
        #region Configuration

        public class ExcludeTypesConfiguration
        {
            private static readonly Type[] DefaultExcludeTypes = new[] { typeof(EasySerializationData) };

            private readonly MembersGetterConfiguration _configuration;
            private readonly List<Type> _excludeTypes;

            internal ExcludeTypesConfiguration(MembersGetterConfiguration configuration,
                List<Type> excludeTypes)
            {
                _configuration = configuration;
                _excludeTypes = excludeTypes;
                InternalReset();
            }

            public ExcludeTypesConfiguration Reset()
            {
                InternalReset();
                return this;
            }

            public ExcludeTypesConfiguration Add(Type type)
            {
                _excludeTypes.Add(type);
                return this;
            }

            public ExcludeTypesConfiguration Remove(Type type)
            {
                _excludeTypes.Remove(type);
                return this;
            }

            private void InternalReset()
            {
                _excludeTypes.Clear();
                _excludeTypes.AddRange(DefaultExcludeTypes);
            }
        }

        public class FlagsConfiguration<T>
            where T : struct, Enum
        {
            private readonly MembersGetterConfiguration _configuration;
            private readonly Action<T> _setFlags;
            private readonly Func<T?> _getFlags;

            internal FlagsConfiguration(MembersGetterConfiguration configuration, Action<T> setFlags,
                Func<T?> getFlags)
            {
                _configuration = configuration;
                _setFlags = setFlags;
                _getFlags = getFlags;
            }

            public MembersGetterConfiguration Add(T flags)
            {
                var x = _getFlags();

                var res = Convert.ToInt32(flags);
                if (x.HasValue)
                {
                    res |= Convert.ToInt32(x.Value);
                }

                _setFlags((T)(object)res);
                return _configuration;
            }

            public MembersGetterConfiguration Remove(T flags)
            {
                var x = _getFlags();

                var res = Convert.ToInt32(flags);
                if (x.HasValue)
                {
                    res &= ~Convert.ToInt32(x.Value);
                }

                _setFlags((T)(object)res);
                return _configuration;
            }

            public MembersGetterConfiguration Set(T flags)
            {
                _setFlags(flags);
                return _configuration;
            }
        }

        #endregion

        private BindingFlags? _fieldBindingFlags;
        private BindingFlags? _propertyBindingFlags;
        private FieldGetterFlags _fieldGetterFlags;
        private PropertyGetterFlags _propertyGetterFlags;
        private readonly List<Type> _excludeTypes = new List<Type>();
        private bool _makeCache = true;

        public FlagsConfiguration<BindingFlags> FieldBindingFlags { get; }
        public FlagsConfiguration<BindingFlags> PropertyBindingFlags { get; }
        public FlagsConfiguration<FieldGetterFlags> FieldGetterFlags { get; }
        public FlagsConfiguration<PropertyGetterFlags> PropertyGetterFlags { get; }
        public ExcludeTypesConfiguration ExcludeTypes { get; }

        public MembersGetterConfiguration()
        {
            FieldBindingFlags = new FlagsConfiguration<BindingFlags>(
                this, flags => _fieldBindingFlags = flags, () => _fieldBindingFlags);
            PropertyBindingFlags = new FlagsConfiguration<BindingFlags>(
                this, flags => _propertyBindingFlags = flags, () => _propertyBindingFlags);

            FieldGetterFlags = new FlagsConfiguration<FieldGetterFlags>(
                this, flags => _fieldGetterFlags = flags, () => _fieldGetterFlags);
            PropertyGetterFlags = new FlagsConfiguration<PropertyGetterFlags>(
                this, flags => _propertyGetterFlags = flags, () => _propertyGetterFlags);

            ExcludeTypes = new ExcludeTypesConfiguration(this, _excludeTypes);
        }

        public MembersGetterConfiguration MakeCache(bool b = true)
        {
            _makeCache = b;
            return this;
        }

        public MembersGetterDelegate CreateGetter()
        {
            Dictionary<Type, MemberInfo[]> cache = null;
            if (_makeCache)
            {
                cache = new Dictionary<Type, MemberInfo[]>();
            }

            var fieldBindingFlags = _fieldBindingFlags;
            var fieldGetterFlags = _fieldGetterFlags;
            var propertyBindingFlags = _propertyBindingFlags;
            var propertyGetterFlags = _propertyGetterFlags;
            var excludeTypes = _excludeTypes.ToArray();

            return type => InternalMemberGetter.Invoke(
                type,
                fieldBindingFlags, fieldGetterFlags,
                propertyBindingFlags, propertyGetterFlags,
                excludeTypes,
                cache);
        }
    }

    internal class InternalMemberGetter
    {
        public static MemberInfo[] Invoke(
            Type targetType,
            BindingFlags? fieldBindingFlags,
            FieldGetterFlags fieldGetterFlags,
            BindingFlags? propertyBindingFlags,
            PropertyGetterFlags propertyGetterFlags,
            Type[] excludeTypes,
            Dictionary<Type, MemberInfo[]> cache)
        {
            if (cache != null && cache.TryGetValue(targetType, out var members))
            {
                return members;
            }

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

            var t = total.ToArray();
            if (cache != null)
            {
                cache[targetType] = t;
            }

            return t;
        }
    }
}
