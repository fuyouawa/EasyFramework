using System;
using System.Linq;
using System.Reflection;

namespace EasyFramework.Serialization
{
    public class EasySerializeSettings
    {
        internal IMembersWithSerializerGetter MembersWithSerializerGetter { get; }

        public MemberFilter MemberFilter { get; }

        public EasySerializeSettings()
            : this(MemberFilterPresets.Default)
        {
        }

        public EasySerializeSettings(MemberFilter memberFilter)
        {
            MemberFilter = memberFilter;
            MembersWithSerializerGetter = new MembersWithSerializerGetter(MemberFilter);
        }
    }
}
