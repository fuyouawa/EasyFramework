using System.Collections.Generic;

namespace EasyFramework
{
    public interface IReadonlyBindableValue<T>
    {
        T Value { get; }
        EasyEvent<T> OnValueChanged { get; }
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

        public T Value => _value;

        public EasyEvent<T> OnValueChanged { get; } = new EasyEvent<T>();

        public void SetValue(T value)
        {
            if (value == null && _value == null) return;
            if (value != null && EqualityComparer<T>.Default.Equals(_value, value)) return;

            SetValueWithoutEvent(value);
            OnValueChanged.Invoke(value);
        }

        public void SetValueWithoutEvent(T value)
        {
            _value = value;
        }

        public override string ToString() => _value.ToString();
    }
}
