using System;
using JetBrains.Annotations;

namespace EasyToolKit.Inspector.Editor
{
    public interface IPropertyValueEntry : IDisposable
    {
        object WeakSmartValue { get; set; }
        Type ValueType { get; }
        Type BaseValueType { get; }
        [CanBeNull] Type RuntimeValueType { get; }
        IPropertyValueCollection WeakValues { get; }
        InspectorProperty Property { get; }

        event Action<int> OnValueChanged;

        bool IsConflicted();

        void Update();
        bool ApplyChanges();
    }

    public interface IPropertyValueEntry<TValue> : IPropertyValueEntry
    {
        TValue SmartValue { get; set; }
        IPropertyValueCollection<TValue> Values { get; }
    }
}