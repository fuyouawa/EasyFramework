using System;
using System.Linq;
using System.Reflection;

namespace EasyFramework.Serialization
{
    public class EasySerializeSettings
    {
        public MembersGetterDelegate MembersGetter { get; set; } = MembersGetterPresets.Default;
    }
}
