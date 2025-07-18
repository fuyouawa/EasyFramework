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

        public override void SetValue(object owner, object value)
        {
            _setter(ref owner, value);
        }

        public override object GetValue(object owner)
        {
            return _getter(ref owner);
        }
    }
}
