using Sirenix.Serialization;
using System;

namespace EasyFramework.Utilities
{
    [Serializable]
    public abstract class BinarySerailizedValue<T> : TSerailizedValue<T, byte[]>
    {
        protected override void SerializeData(ref byte[] serializedData)
        {
            serializedData = SerializationUtility.SerializeValue(Value, DataFormat.Binary);
        }

        protected override void DeserializeData(ref byte[] serializedData)
        {
            Value = SerializationUtility.DeserializeValue<T>(serializedData, DataFormat.Binary);
        }
    }
}
