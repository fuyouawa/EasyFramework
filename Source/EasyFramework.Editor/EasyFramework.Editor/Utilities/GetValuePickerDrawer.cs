using System.Reflection;
using EasyFramework;
using Sirenix.OdinInspector.Editor;

namespace EasyFramework.Editor
{
    public class GetValuePickerDrawer : MemberPickerDrawer<GetValuePicker>
    {
        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            return true;
        }

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
    }

    public class GetValuePickerDrawer<TReturn> : MemberPickerDrawer<GetValuePicker<TReturn>>
    {
        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            return true;
        }

        protected override bool MemberFilter(MemberInfo member)
        {
            if (base.MemberFilter(member) && member.TryGetValueType(out var type))
            {
                return type == typeof(TReturn);
            }
            return false;
        }
    }
}
