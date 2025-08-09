using System;

namespace EasyToolKit.Inspector.Editor
{
    public static class PropertyValueEntryUtility
    {
        public static Type GetPreferencedValueType(this IPropertyValueEntry valueEntry)
        {
            var runtimeValueType = valueEntry.RuntimeValueType;
            if (runtimeValueType != null)
            {
                return runtimeValueType;
            }

            return valueEntry.BaseValueType;
        }
    }
}