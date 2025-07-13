using System;

namespace EasyToolKit.Inspector.Editor
{
    public interface IInspectorValueAccessor
    {
        bool IsReadonly { get; }
        Type OwnerType { get; }
        Type ValueType { get; }
        void SetValue(object owner, object value);
        object GetValue(object owner);
    }

    public abstract class InspectorValueAccessor : IInspectorValueAccessor
    {
        public virtual bool IsReadonly => false;

        public abstract Type OwnerType { get; }

        public abstract Type ValueType { get; }

        public abstract void SetValue(object owner, object value);
        public abstract object GetValue(object owner);
    }

    public abstract class InspectorValueAccessor<TOwner, TValue> : IInspectorValueAccessor
    {
        public virtual bool IsReadonly => false;
        public virtual Type OwnerType => typeof(TOwner);
        public virtual Type ValueType => typeof(TValue);

        public virtual void SetValue(object owner, object value)
        {
            var castOwner = (TOwner)owner;
            SetValue(ref castOwner, (TValue)value);
        }

        public virtual object GetValue(object owner)
        {
            var castOwner = (TOwner)owner;
            return GetValue(ref castOwner);
        }

        public abstract void SetValue(ref TOwner owner, TValue value);

        public abstract TValue GetValue(ref TOwner owner);
    }
}
