using System;
using System.Collections.Generic;

namespace EasyFramework.Serialization
{
    public class ExcludeTypesConfigurationOfMembersGetter
    {
        private static readonly Type[] DefaultExcludeTypes = new[] { typeof(EasySerializationData) };

        private readonly MembersGetterConfiguration _configuration;
        private readonly List<Type> _excludeTypes;

        internal ExcludeTypesConfigurationOfMembersGetter(MembersGetterConfiguration configuration,
            List<Type> excludeTypes)
        {
            _configuration = configuration;
            _excludeTypes = excludeTypes;
            InternalReset();
        }

        public ExcludeTypesConfigurationOfMembersGetter Reset()
        {
            InternalReset();
            return this;
        }

        public ExcludeTypesConfigurationOfMembersGetter Add(Type type)
        {
            _excludeTypes.Add(type);
            return this;
        }

        public ExcludeTypesConfigurationOfMembersGetter Remove(Type type)
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
}
