using System;
using System.Collections.Generic;

namespace EasyFramework.ToolKit
{
    public interface IReadonlyBindableValue<T>
    {
        T Value { get; }
        void UnRegister(Action<T> onValueChanged);
        IFromRegister Register(Action<T> onValueChanged);
    }

    public interface IBindableValue<T> : IReadonlyBindableValue<T>
    {
        void SetValue(T value);
        void SetValueWithoutEvent(T value);
    }

    public class BindableValue<T> : IBindableValue<T>
    {
        public BindableValue(T defaultValue = default)
        {
            _value = defaultValue;
        }

        private T _value;

        public virtual T Value => _value;

        public void SetValue(T value)
        {
            if (value == null && _value == null) return;
            if (value != null && EqualityComparer<T>.Default.Equals(_value, value)) return;
            
            SetValueWithoutEvent(value);
            OnValueChanged?.Invoke(value);
        }

        public void SetValueWithoutEvent(T value)
        {
            _value = value;
        }

        public IFromRegister Register(Action<T> onValueChanged)
        {
            OnValueChanged += onValueChanged;
            return new FromRegisterGeneric(() => UnRegister(onValueChanged));
        }


        public void UnRegister(Action<T> onValueChanged)
        {
            OnValueChanged -= onValueChanged;
        }

        private event Action<T> OnValueChanged;

        public override string ToString() => _value.ToString();
    }
}
