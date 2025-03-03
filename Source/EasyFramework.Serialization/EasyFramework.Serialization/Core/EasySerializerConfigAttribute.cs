using System;

namespace EasyFramework.Serialization
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EasySerializerConfigAttribute : Attribute
    {
        public int Priority { get; }

        public EasySerializerConfigAttribute()
            : this(EasySerializerProiority.Custom)
        {
        }

        public EasySerializerConfigAttribute(EasySerializerProiority priority)
        {
            Priority = (int)priority;
        }

        public EasySerializerConfigAttribute(int priority)
        {
            Priority = priority;
        }
    }
}
