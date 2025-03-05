using System;
using System.Linq;
using System.Reflection;

namespace EasyFramework.Serialization
{
    public class EasySerializeSettings
    {
        public MembersGetterDelegate MembersGetterOfGeneric { get; set; } = MembersGetterPresets.Default;
    }
}
