using System;

namespace EasyToolKit.Inspector.Editor
{
    public class PropertyValueEntryWrapper<TValue, TBaseValue> : IPropertyValueEntry<TValue>
    {
        private IPropertyValueEntry<TBaseValue> _valueEntry;

        public PropertyValueEntryWrapper(IPropertyValueEntry<TBaseValue> valueEntry)
        {
            _valueEntry = valueEntry;
        }

        public TValue SmartValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IPropertyValueCollection<TValue> Values => throw new NotImplementedException();

        public object WeakSmartValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Type BaseValueType => throw new NotImplementedException();

        public Type RuntimeValueType => throw new NotImplementedException();

        public IPropertyValueCollection WeakValues => throw new NotImplementedException();

        public InspectorProperty Property => throw new NotImplementedException();

        public bool IsWrapper => true;

        public event Action<int> OnValueChanged;

        public bool ApplyChanges()
        {
            throw new NotImplementedException();
        }

        public bool IsConflicted()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}