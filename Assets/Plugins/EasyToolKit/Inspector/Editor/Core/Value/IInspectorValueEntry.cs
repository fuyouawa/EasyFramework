using System;

namespace EasyToolKit.Inspector.Editor
{
    public interface IInspectorValueEntry
    {
        object WeakSmartValue { get; set; }
        Type ValueType { get; }
        IInspectorValueCollection WeakValues { get; }
        InspectorProperty Property { get; }

        event Action<int> OnValueChanged;

        internal void Update();
        internal bool ApplyChanges();
    }

    public interface IInspectorValueEntry<TValue> : IInspectorValueEntry
    {
        TValue SmartValue { get; set; }
        IInspectorValueCollection<TValue> Values { get; }
    }
}
