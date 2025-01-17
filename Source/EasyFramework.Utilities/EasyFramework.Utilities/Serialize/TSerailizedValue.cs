using System;
using UnityEngine;

namespace EasyGameFramework
{
    [Serializable]
    public abstract class TSerailizedValue<TValue, TSerializedData> : ISerializationCallbackReceiver
    {
        public abstract TValue Value { get; set; }

        [SerializeField, HideInInspector]
        private TSerializedData _serializedData;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            SerializeData(ref _serializedData);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            DeserializeData(ref _serializedData);
        }

        protected abstract void SerializeData(ref TSerializedData serializedData);
        protected abstract void DeserializeData(ref TSerializedData serializedData);
    }
}
