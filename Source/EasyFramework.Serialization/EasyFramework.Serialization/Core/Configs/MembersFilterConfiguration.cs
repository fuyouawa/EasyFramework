using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;
using UnityEngine;

namespace EasyFramework.Serialization
{
    [Flags]
    public enum MemberFilterFlags
    {
        None = 0,
        Public = 1 << 0,
        NonPublic = 1 << 1,

        /// <summary>
        /// 有SerializeField特性的成员，并且忽略访问修饰符
        /// </summary>
        SerializeField = 1 << 2,
        Field = 1 << 3,
        ReadOnlyProperty = 1 << 4,
        WriteOnlyProperty = 1 << 5,
        ReadWriteProperty = 1 << 6,
        AllProperty = ReadOnlyProperty | WriteOnlyProperty | ReadWriteProperty
    }

    public class MemberFilterConfiguration
    {
        #region Configuration

        public class ExcludeTypesConfiguration
        {
            private static readonly Type[] DefaultExcludeTypes = new[] { typeof(EasySerializationData) };

            private readonly MemberFilterConfiguration _configuration;
            private readonly List<Type> _excludeTypes;

            internal ExcludeTypesConfiguration(MemberFilterConfiguration configuration,
                List<Type> excludeTypes)
            {
                _configuration = configuration;
                _excludeTypes = excludeTypes;
                ResetImpl();
            }

            public ExcludeTypesConfiguration Reset()
            {
                ResetImpl();
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

            private void ResetImpl()
            {
                _excludeTypes.Clear();
                _excludeTypes.AddRange(DefaultExcludeTypes);
            }
        }

        public class FlagsConfiguration<T>
            where T : struct, Enum
        {
            private readonly MemberFilterConfiguration _configuration;
            private readonly Action<T> _setFlags;
            private readonly Func<T?> _getFlags;

            internal FlagsConfiguration(MemberFilterConfiguration configuration, Action<T> setFlags,
                Func<T?> getFlags)
            {
                _configuration = configuration;
                _setFlags = setFlags;
                _getFlags = getFlags;
            }

            public MemberFilterConfiguration Add(T flags)
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

            public MemberFilterConfiguration Remove(T flags)
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

            public MemberFilterConfiguration Set(T flags)
            {
                _setFlags(flags);
                return _configuration;
            }
        }

        #endregion

        private MemberFilterFlags _memberFilterFlags;
        private readonly List<Type> _excludeTypes = new List<Type>();

        public FlagsConfiguration<MemberFilterFlags> MemberFilterFlags { get; }
        public ExcludeTypesConfiguration ExcludeTypes { get; }

        public MemberFilterConfiguration()
        {
            MemberFilterFlags = new FlagsConfiguration<MemberFilterFlags>(
                this, flags => _memberFilterFlags = flags, () => _memberFilterFlags);

            ExcludeTypes = new ExcludeTypesConfiguration(this, _excludeTypes);
        }

        public MemberFilter CreateFilter()
        {
            var fieldGetterFlags = _memberFilterFlags;
            var excludeTypes = _excludeTypes.ToArray();

            return member => MemberFilterImpl.Invoke(member, fieldGetterFlags, excludeTypes);
        }
    }
}
