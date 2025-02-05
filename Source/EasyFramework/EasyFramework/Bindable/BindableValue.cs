using System;
using System.Collections.Generic;

namespace EasyFramework
{
    public interface IReadonlyBindableValue<T>
    {
        T GetValue();
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

        public void SetValue(T value)
        {
            if (value == null && _value == null) return;
            if (value != null && EqualityComparer<T>.Default.Equals(_value, value)) return;

            IntervalSetValue(value);
            _onValueChanged?.Invoke(value);
        }

        public void SetValueWithoutEvent(T value)
        {
            IntervalSetValue(value);
        }

        public T GetValue()
        {
            return IntervalGetValue();
        }

        public IFromRegister Register(Action<T> onValueChanged)
        {
            _onValueChanged += onValueChanged;
            return new FromRegister(() => UnRegister(onValueChanged));
        }

        public void UnRegister(Action<T> onValueChanged)
        {
            _onValueChanged -= onValueChanged;
        }

        protected virtual void IntervalSetValue(T value)
        {
            _value = value;
        }

        protected virtual T IntervalGetValue()
        {
            return _value;
        }

        private event Action<T> _onValueChanged;

        public override string ToString() => _value.ToString();
    }
}
