using System;
using System.Collections.Generic;
using System.Linq;
using EasyFramework.Core;
using JetBrains.Annotations;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEngine;

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

        public static GUIContent GetSmartContent(this InspectorProperty property)
        {
            return property.GetSmartContent(o => o.ToString());
        }

        private static readonly GUIContent SmartLabelCache = new GUIContent();
        public static GUIContent GetSmartContent(this InspectorProperty property,
            Func<object, string> textGetter, string conflictText = "一")
        {
            if (property.ValueEntry.WeakValues.Cast<object>().AllSame())
            {
                var value = property.ValueEntry.WeakSmartValue;

                if (value == null)
                {
                    SmartLabelCache.text = EasyEditorGUI.NoneSelectorBtnLabel.text;
                    SmartLabelCache.tooltip = EasyEditorGUI.NoneSelectorBtnLabel.tooltip;
                    SmartLabelCache.image = EasyEditorGUI.NoneSelectorBtnLabel.image;
                }
                else
                {
                    SmartLabelCache.text = textGetter(value);
                    SmartLabelCache.tooltip = null;

                    if (value is Type type)
                    {
                        SmartLabelCache.image = GUIHelper.GetAssetThumbnail(null, type, false);
                    }
                    else
                    {
                        SmartLabelCache.image = null;
                    }
                }
            }
            else
            {
                SmartLabelCache.text = "一";
                SmartLabelCache.tooltip = null;
                SmartLabelCache.image = null;
            }

            return SmartLabelCache;
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
