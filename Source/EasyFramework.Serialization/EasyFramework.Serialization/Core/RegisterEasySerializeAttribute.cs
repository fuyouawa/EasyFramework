using System;

namespace EasyFramework.Serialization
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterEasySerializerAttribute : Attribute
    {
        public int Priority { get; }
        public Type SerializerType { get; }

        public RegisterEasySerializerAttribute(Type serializerType, int priority = 0)
        {
            SerializerType = serializerType;
            Priority = priority;
        }
    }
}
