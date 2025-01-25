using JetBrains.Annotations;
using Sirenix.OdinInspector.Editor;

namespace EasyFramework.Editor
{
    public static class InspectorPropertyExtension
    {
        public static T WeakSmartValue<T>(this InspectorProperty property)
        {
            var val = property.ValueEntry.WeakSmartValue;
            if (val == null)
                return default;
            return (T)val;
        }

        public static void SetWeakSmartValue(this InspectorProperty property, [CanBeNull] object value)
        {
            property.ValueEntry.WeakSmartValue = value;
        }
    }
}
