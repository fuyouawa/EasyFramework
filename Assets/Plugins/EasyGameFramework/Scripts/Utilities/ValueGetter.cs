using System;
using System.Reflection;
using EasyFramework;

namespace EasyGameFramework
{
    [Serializable]
    public class ValueGetter : MemberPicker
    {
        public object GetRawValue()
        {
            var c = GetTargetComponent();
            var m = GetTargetMember();
            switch (m)
            {
                case FieldInfo field:
                    return field.GetValue(c);
                case PropertyInfo property:
                    return property.GetValue(c);
                case MethodInfo method:
                    return method.Invoke(c, null);
                default:
                    throw new NotSupportedException();
            }
        }

#if UNITY_EDITOR
        protected override bool MemberFilter(MemberInfo member)
        {
            if (!member.TryGetValueType(out var type))
                return false;
            if (type == typeof(void))
                return false;
            if (member is MethodInfo method)
                return method.GetParameters().Length == 0 && !method.IsSpecialName;
            return true;
        }
#endif
    }

    [Serializable]
    public class ValueGetter<TReturn> : ValueGetter
    {
        public new TReturn GetRawValue()
        {
            return (TReturn)base.GetRawValue();
        }

#if UNITY_EDITOR
        protected override bool MemberFilter(MemberInfo member)
        {
            if (base.MemberFilter(member) && member.TryGetValueType(out var type))
            {
                return type == typeof(TReturn);
            }
            return false;
        }
#endif
    }
}
