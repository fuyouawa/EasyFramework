using System;
using JetBrains.Annotations;
using UnityEngine;

namespace EasyFramework
{
    [Serializable]
    public class SerializedVariant
    {
        [SerializeField] private SerializedType _type = new SerializedType();
        [SerializeField] private SerializedAny _any = new SerializedAny();

        public Type Type
        {
            get => _type.Value;
            set => _type.Value = value;
        }

        public SerializedVariant()
        {
        }

        public SerializedVariant([CanBeNull] object value)
        {
            SetRawObject(value);
        }

        public bool IsEmpty()
        {
            return _type.Value == null && _any.IsEmpty();
        }

        public void SetNull()
        {
            _type.Value = null;
            _any.SetNull();
        }

        public void Set<T>(T value)
        {
            _type.Value = typeof(T);
            _any.Set(value);
        }

        private bool TypeCheck(Type exceptedType, bool allocBaseType)
        {
            if (_type.Value == exceptedType)
                return true;

            if (allocBaseType)
            {
                if (!_type.Value.IsSubclassOf(exceptedType))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public T Get<T>(bool allowBaseType = false)
        {
            if (!TypeCheck(typeof(T), allowBaseType))
            {
                throw new ArgumentException(
                    $"Variant types do not match, " +
                    $"current is \"{_type.Value}\", " +
                    $"but excepted is \"{typeof(T)}\"");
            }

            return _any.Get<T>();
        }

        public void SetRawObject([CanBeNull] object obj)
        {
            _type.Value = obj?.GetType();
            _any.SetRawObject(obj);
        }

        public object GetRawObject()
        {
            if (_type.Value == null)
            {
                if (!_any.IsEmpty())
                {
                    throw new Exception("Unknown type!");
                }

                return null;
            }

            return _any.GetRawObject(_type.Value);
        }
    }
}
