using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector.Editor;

namespace EasyFramework.Editor
{
    public static class InspectorPropertyExtension
    {
        public static T WeakSmartValueT<T>(this IPropertyValueEntry valueEntry)
        {
            return (T)valueEntry.WeakSmartValue;
        }

        public static void SetAllWeakValues(this IPropertyValueEntry valueEntry, object value)
        {
            for (int i = 0; i < valueEntry.WeakValues.Count; i++)
            {
                valueEntry.WeakValues[i] = value;
            }
        }

        public static bool IsAllSameWeakValues(this IPropertyValueEntry valueEntry)
        {
            if (valueEntry.WeakValues.Count == 0)
                return true;

            var fst = valueEntry.WeakValues[0];
            for (int i = 1; i < valueEntry.WeakValues.Count; i++)
            {
                if (!fst.Equals(valueEntry.WeakValues[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static void DrawEx(this InspectorProperty property, string label)
        {
            property.Draw(EditorHelper.TempContent(label));
        }

        public static void DrawEx(this InspectorProperty property, string label, string tooltip)
        {
            property.Draw(EditorHelper.TempContent(label, tooltip));
        }
    }
}
