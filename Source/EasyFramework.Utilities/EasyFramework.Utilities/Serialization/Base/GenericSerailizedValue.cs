using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EasyFramework.Utilities
{
    [Serializable]
    public abstract class GenericSerailizedValue<T> : TSerailizedValue<T, byte[]>
    {
        [SerializeField, HideInInspector]
        private List<Object> _serializedUnityObjects = new List<Object>();

        protected override void SerializeData(ref byte[] serializedData)
        {
            serializedData = SerializationUtility.SerializeValue(Value, DataFormat.Binary, out _serializedUnityObjects);
        }

        protected override void DeserializeData(ref byte[] serializedData)
        {
            Value = SerializationUtility.DeserializeValue<T>(serializedData, DataFormat.Binary, _serializedUnityObjects);
        }
    }
}
