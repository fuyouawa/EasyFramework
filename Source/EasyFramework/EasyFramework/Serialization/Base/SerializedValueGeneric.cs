using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EasyFramework
{
    [Serializable]
    public abstract class SerializedValueGeneric<T> : TSerializedValue<T, byte[]>
    {
        [SerializeField, HideInInspector]
        private List<Object> _serializedUnityObjects = new List<Object>();

        protected override void OnSerializeData(out byte[] serializedData)
        {
            OnSerializeData(out serializedData, out _serializedUnityObjects);
        }

        protected override void OnDeserializeData(ref byte[] serializedData)
        {
            OnDeserializeData(ref serializedData, ref _serializedUnityObjects);
        }

        protected virtual void OnSerializeData(out byte[] serializedData, out List<Object> serializedUnityObjects)
        {
            serializedData = SerializationUtility.SerializeValue(Value, DataFormat.Binary, out serializedUnityObjects);
        }

        protected virtual void OnDeserializeData(ref byte[] serializedData, ref List<Object> serializedUnityObjects)
        {
            Value = SerializationUtility.DeserializeValue<T>(serializedData, DataFormat.Binary, serializedUnityObjects);
        }
    }
}
