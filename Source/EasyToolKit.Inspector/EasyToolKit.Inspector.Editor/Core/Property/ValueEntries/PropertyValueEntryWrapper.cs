using System;

namespace EasyToolKit.Inspector.Editor
{
    public class PropertyValueEntryWrapper<TValue, TBaseValue> : IPropertyValueEntryWrapper<TValue, TBaseValue>
        where TValue : TBaseValue
    {
        private IPropertyValueEntry<TBaseValue> _valueEntry;
        private IPropertyValueCollectionWrapper<TValue, TBaseValue> _valuesWrapper;

        public PropertyValueEntryWrapper(IPropertyValueEntry<TBaseValue> valueEntry)
        {
            _valueEntry = valueEntry;
            _valuesWrapper = new PropertyValueCollectionWrapper<TValue, TBaseValue>(valueEntry.Values);
        }

        public TValue SmartValue
        {
            get => (TValue)_valueEntry.WeakSmartValue;
            set => _valueEntry.WeakSmartValue = value;
        }

        public IPropertyValueCollection<TValue> Values => _valuesWrapper;

        public object WeakSmartValue { get => _valueEntry.WeakSmartValue; set => _valueEntry.WeakSmartValue = value; }

        public Type BaseValueType => _valueEntry.BaseValueType;

        public Type RuntimeValueType => _valueEntry.RuntimeValueType;

        public IPropertyValueCollection WeakValues => _valueEntry.WeakValues;

        public InspectorProperty Property => _valueEntry.Property;

        public Type ValueType => typeof(TValue);

        public event Action<int> OnValueChanged
        {
            add => _valueEntry.OnValueChanged += value;
            remove => _valueEntry.OnValueChanged -= value;
        }

        public bool ApplyChanges()
        {
            return _valueEntry.ApplyChanges();
        }

        public bool IsConflicted()
        {
            return _valueEntry.IsConflicted();
        }

        public void Update()
        {
            _valueEntry.Update();
        }

        public void Dispose()
        {
            _valuesWrapper.Dispose();
        }
    }
}