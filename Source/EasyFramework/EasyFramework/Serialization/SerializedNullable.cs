using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EasyFramework
{
    [Serializable]
    public class SerializedNullable<T> : GenericSerailizedValue<T>
    {
        [Serializable]
        private struct ValueStore
        {
            public bool HasValue;
            public T Value;

            public override int GetHashCode()
            {
                return HasValue ? Value.GetHashCode() : 0;
            }
        }

        [NonSerialized, ShowInInspector] private ValueStore _store;

        public SerializedNullable(T value)
        {
            AssignValue(value);
        }

        public bool HasValue => _store.HasValue;

        public override T Value
        {
            get
            {
                if (!_store.HasValue)
                {
                    throw new InvalidOperationException("No value!");
                }

                return _store.Value;
            }
            set { AssignValue(value); }
        }

        public T GetValueOrDefault()
        {
            return _store.Value;
        }

        public T GetValueOrDefault(T defaultValue)
        {
            return _store.HasValue ? _store.Value : defaultValue;
        }

        public override bool Equals(object other)
        {
            if (!_store.HasValue)
                return other == null;

            if (other == null)
                return false;
            return _store.Value.Equals(other);
        }

        public override string ToString()
        {
            return _store.HasValue ? _store.Value.ToString() : "";
        }

        public static implicit operator SerializedNullable<T>(T value)
        {
            return new SerializedNullable<T>(value);
        }

        public static explicit operator T(SerializedNullable<T> value)
        {
            return value!.Value;
        }

        private void AssignValue(T value)
        {
            if (typeof(T).IsPrimitive)
            {
                _store.HasValue = true;
            }
            else
            {
                _store.HasValue = value == null;
            }

            _store.Value = value;
        }

        protected override void OnSerializeData(out byte[] serializedData, out List<Object> serializedUnityObjects)
        {
            serializedData = SerializationUtility.SerializeValue(_store, DataFormat.Binary, out serializedUnityObjects);
        }

        protected override void OnDeserializeData(ref byte[] serializedData, ref List<Object> serializedUnityObjects)
        {
            _store = SerializationUtility.DeserializeValue<ValueStore>(serializedData, DataFormat.Binary,
                serializedUnityObjects);
        }
    }
}
