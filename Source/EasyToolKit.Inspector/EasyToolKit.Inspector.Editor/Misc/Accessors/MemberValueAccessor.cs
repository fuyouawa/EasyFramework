using System;
using System.Reflection;
using EasyToolKit.ThirdParty.OdinSerializer.Utilities;

namespace EasyToolKit.Inspector.Editor
{
    public class MemberValueAccessor<TOwner, TValue> : ValueAccessor<TOwner, TValue>
    {
        private readonly MemberInfo _memberInfo;
        private readonly ValueGetter<TOwner, TValue> _getter;
        private readonly ValueSetter<TOwner, TValue> _setter;

        public MemberValueAccessor(MemberInfo memberInfo)
        {
            if (memberInfo is FieldInfo fieldInfo)
            {
                _getter = EmitUtilities.CreateInstanceFieldGetter<TOwner, TValue>(fieldInfo);
                _setter = EmitUtilities.CreateInstanceFieldSetter<TOwner, TValue>(fieldInfo);
            }
            else if (memberInfo is PropertyInfo propertyInfo)
            {
                _getter = EmitUtilities.CreateInstancePropertyGetter<TOwner, TValue>(propertyInfo);
                _setter = EmitUtilities.CreateInstancePropertySetter<TOwner, TValue>(propertyInfo);
            }
            else
            {
                throw new NotSupportedException();  //TODO 异常信息
            }

            _memberInfo = memberInfo;
        }

        public override void SetValue(ref TOwner target, TValue value)
        {
            _setter(ref target, value);
        }

        public override TValue GetValue(ref TOwner target)
        {
            return _getter(ref target);
        }
    }
}
