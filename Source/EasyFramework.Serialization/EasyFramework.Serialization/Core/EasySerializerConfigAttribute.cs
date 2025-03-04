using System;

namespace EasyFramework.Serialization
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EasySerializerConfigAttribute : Attribute
    {
        public int Priority { get; }
        public bool AllowInherit { get; }

        public EasySerializerConfigAttribute()
            : this(EasySerializerProiority.Custom)
        {
        }

        public EasySerializerConfigAttribute(EasySerializerProiority priority, bool allowInherit = false)
        {
            Priority = (int)priority;
            AllowInherit = allowInherit;
        }

        public EasySerializerConfigAttribute(int priority, bool allowInherit = false)
        {
            Priority = priority;
            AllowInherit = allowInherit;
        }
    }
}
