using System;

namespace EasyFramework.Serialization
{
    public class EasySerializationException : Exception
    {
        public EasySerializationException(string message)
            : base(message)
        {
        }
    }
}
