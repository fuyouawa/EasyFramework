using System;

namespace EasyToolKit.Inspector.Editor
{
    public interface IValueAccessor
    {
        bool IsReadonly { get; }
        Type OwnerType { get; }
        Type ValueType { get; }
        void SetValue(object owner, object value);
        object GetValue(object owner);
    }

    public interface IValueAccessor<TOwner, TValue> : IValueAccessor
    {
        void SetValue(ref TOwner owner, TValue value);
        TValue GetValue(ref TOwner owner);
    }

    public abstract class ValueAccessor : IValueAccessor
    {
        public virtual bool IsReadonly => false;

        public abstract Type OwnerType { get; }

        public abstract Type ValueType { get; }

        public abstract void SetValue(object owner, object value);
        public abstract object GetValue(object owner);
    }

    public abstract class ValueAccessor<TOwner, TValue> : IValueAccessor<TOwner, TValue>
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
