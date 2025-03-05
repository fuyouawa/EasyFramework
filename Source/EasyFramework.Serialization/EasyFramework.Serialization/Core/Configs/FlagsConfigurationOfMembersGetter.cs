using System;

namespace EasyFramework.Serialization
{
    public class FlagsConfigurationOfMembersGetter<T>
        where T : struct, Enum
    {
        private readonly MembersGetterConfiguration _configuration;
        private readonly Action<T> _setFlags;
        private readonly Func<T?> _getFlags;

        internal FlagsConfigurationOfMembersGetter(MembersGetterConfiguration configuration, Action<T> setFlags,
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

}
