using Sirenix.Serialization;
using System;
using System.Collections.Generic;

namespace EasyGameFramework
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
