using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    [Serializable, InlineProperty, HideReferenceObjectPicker]
    public abstract class TSerializedValue<TValue, TSerializedData> : ISerializationCallbackReceiver,
        IEquatable<TSerializedValue<TValue, TSerializedData>>
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

        public virtual bool Equals(TSerializedValue<TValue, TSerializedData> other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;

            return EqualityComparer<TValue>.Default.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (obj is TSerializedValue<TValue, TSerializedData> other)
                return Equals(other);
            return false;
        }

        public static bool operator ==(TSerializedValue<TValue, TSerializedData> a,
            TSerializedValue<TValue, TSerializedData> b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) return false;

            return a.Equals(b);
        }

        public static bool operator !=(TSerializedValue<TValue, TSerializedData> a,
            TSerializedValue<TValue, TSerializedData> b)
        {
            return !(a == b);
        }
    }
}
