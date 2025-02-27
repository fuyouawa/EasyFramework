using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    [Serializable, InlineProperty, HideReferenceObjectPicker]
    public abstract class TSerializedValue<TValue, TSerializedData> : ISerializationCallbackReceiver
    {
        public abstract TValue Value { get; set; }

        [SerializeField, HideInInspector]
        private TSerializedData _serializedData;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            OnSerializeData(out _serializedData);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            OnDeserializeData(ref _serializedData);
        }

        protected abstract void OnSerializeData(out TSerializedData serializedData);
        protected abstract void OnDeserializeData(ref TSerializedData serializedData);
    }
}
