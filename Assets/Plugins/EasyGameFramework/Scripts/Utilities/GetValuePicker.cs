using System;
using System.Reflection;

namespace EasyGameFramework
{
    [Serializable]
    public class GetValuePicker : MemberPicker
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
    }

    [Serializable]
    public class GetValuePicker<TReturn> : GetValuePicker
    {
        public new TReturn GetRawValue()
        {
            return (TReturn)base.GetRawValue();
        }
    }
}
