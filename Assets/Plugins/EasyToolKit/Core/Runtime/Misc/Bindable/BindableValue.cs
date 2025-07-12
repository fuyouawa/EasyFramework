using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace EasyToolKit.Core
{
    public interface IReadonlyBindableValue<T>
    {
        T Value { get; }
        event Action<T> OnBeforeValueChange;
        event Action<T> OnValueChanged;
    }

    public interface IBindableValue<T> : IReadonlyBindableValue<T>
    {
        void SetValue(T value);
        void SetValueWithoutEvent(T value);
    }

    public class BindableValue<T> : IBindableValue<T>
    {
        private T _value;

        public BindableValue(T defaultValue = default)
        {
            _value = defaultValue;
        }

        public T Value => _value;
        public event Action<T> OnBeforeValueChange;
        public event Action<T> OnValueChanged;

        public void SetValue(T value)
        {
            if (value == null && _value == null) return;
            if (value != null && EqualityComparer<T>.Default.Equals(_value, value)) return;

            OnBeforeValueChange?.Invoke(value);
            SetValueWithoutEvent(value);
            OnValueChanged?.Invoke(value);
        }

        public void SetValueWithoutEvent(T value)
        {
            _value = value;
        }

        public override string ToString() => _value.ToString();
    }
}
