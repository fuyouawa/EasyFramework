using System;

namespace EasyFramework.Serialization
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EasySerializerPriorityAttribute : Attribute
    {
        public int Priority { get; }

        public EasySerializerPriorityAttribute(int priority = EasySerializerProiority.Default)
        {
            Priority = priority;
        }
    }
}
