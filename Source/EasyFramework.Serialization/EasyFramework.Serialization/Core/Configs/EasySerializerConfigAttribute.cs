using System;

namespace EasyFramework.Serialization
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EasySerializerConfigAttribute : Attribute
    {
        public static readonly EasySerializerConfigAttribute Default = new EasySerializerConfigAttribute();

        public int Priority { get; set; }
        public bool AllowInherit { get; set; }

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
