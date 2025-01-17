using System;

namespace EasyFramework.Generic
{
    public interface IReadonlyBindableProperty<T>
    {
        T Value { get; }

        IUnRegister RegisterWithInitValue(Action<T> action);
        void UnRegister(Action<T> onValueChanged);
        IUnRegister Register(Action<T> onValueChanged);
    }

    public interface IBindableProperty<T> : IReadonlyBindableProperty<T>
    {
        new T Value { get; set; }
        void SetValueWithoutEvent(T newValue);
    }

    public class BindableProperty<T> : IBindableProperty<T>
    {
        public BindableProperty(T defaultValue = default)
        {
            PureValue = defaultValue;
        }

        protected T PureValue;

        public static Func<T, T, bool> Comparer { get; set; } = (a, b) => a.Equals(b);

        public BindableProperty<T> WithComparer(Func<T, T, bool> comparer)
        {
            Comparer = comparer;
            return this;
        }

        public T Value
        {
            get => GetValue();
            set
            {
                if (value == null && PureValue == null) return;
                if (value != null && Comparer(value, PureValue)) return;

                SetValue(value);
                _valueChangedEvent?.Invoke(value);
            }
        }

        protected virtual void SetValue(T newValue) => PureValue = newValue;

        protected virtual T GetValue() => PureValue;

        public void SetValueWithoutEvent(T newValue) => PureValue = newValue;

        private event Action<T> _valueChangedEvent;

        public IUnRegister Register(Action<T> onValueChanged)
        {
            _valueChangedEvent += onValueChanged;
            return new CustomUnRegister(() => _valueChangedEvent -= onValueChanged);
        }

        public IUnRegister RegisterWithInitValue(Action<T> onValueChanged)
        {
            onValueChanged(PureValue);
            return Register(onValueChanged);
        }

        public void UnRegister(Action<T> onValueChanged)
        {
            _valueChangedEvent -= onValueChanged;
        }

        public override string ToString() => Value.ToString();
    }
}