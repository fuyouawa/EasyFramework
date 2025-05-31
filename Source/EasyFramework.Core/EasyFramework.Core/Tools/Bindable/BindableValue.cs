using System.Collections.Generic;

namespace EasyFramework.Core
{
    public interface IReadonlyBindableValue<T>
    {
        T Value { get; }
        IEasyEvent<T> OnBeforeValueChange { get; }
        IEasyEvent<T> OnValueChanged { get; }
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
        private readonly EasyEvent<T> _onBeforeValueChange = new EasyEvent<T>();
        public IEasyEvent<T> OnBeforeValueChange => _onBeforeValueChange;

        private readonly EasyEvent<T> _onValueChanged = new EasyEvent<T>();
        public IEasyEvent<T> OnValueChanged => _onValueChanged;

        public void SetValue(T value)
        {
            if (value == null && _value == null) return;
            if (value != null && EqualityComparer<T>.Default.Equals(_value, value)) return;

            _onBeforeValueChange.Invoke(value);
            SetValueWithoutEvent(value);
            _onValueChanged.Invoke(value);
        }

        public void SetValueWithoutEvent(T value)
        {
            _value = value;
        }

        public override string ToString() => _value.ToString();
    }
}
