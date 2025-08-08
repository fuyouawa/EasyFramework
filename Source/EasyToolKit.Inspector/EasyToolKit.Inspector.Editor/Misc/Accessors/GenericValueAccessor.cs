using System;
using EasyToolKit.ThirdParty.OdinSerializer.Utilities;

namespace EasyToolKit.Inspector.Editor
{
    public class GenericValueAccessor : ValueAccessor
    {
        public override Type OwnerType { get; }
        public override Type ValueType { get; }

        private readonly WeakValueGetter _getter;
        private readonly WeakValueSetter _setter;

        public GenericValueAccessor(Type ownerType, Type valueType, WeakValueGetter getter, WeakValueSetter setter)
        {
            OwnerType = ownerType;
            ValueType = valueType;
            _getter = getter;
            _setter = setter;
        }

        public override void SetWeakValue(object owner, object value)
        {
            _setter(ref owner, value);
        }

        public override object GetWeakValue(object owner)
        {
            return _getter(ref owner);
        }
    }

    public class GenericValueAccessor<TOwner, TValue> : ValueAccessor<TOwner, TValue>
    {
        private readonly Func<TOwner, TValue> _getter;
        private readonly Action<TOwner, TValue> _setter;

        public GenericValueAccessor(Func<TOwner, TValue> getter, Action<TOwner, TValue> setter)
        {
            _getter = getter;
            _setter = setter;
        }

        public override TValue GetValue(ref TOwner owner)
        {
            return _getter(owner);
        }

        public override void SetValue(ref TOwner owner, TValue value)
        {
            _setter(owner, value);
        }
    }
}
