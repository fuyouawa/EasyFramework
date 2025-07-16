using System;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class EnumDrawer<T> : EasyValueDrawer<T>
    {
        protected override bool CanDrawValueType(Type valueType)
        {
            return valueType.IsEnum;
        }

        protected override void OnDrawProperty(GUIContent label)
        {
            var value = ValueEntry.SmartValue;
            EditorGUI.BeginChangeCheck();
            if (value.GetType().IsDefined(typeof(FlagsAttribute), false))
            {
                value = (T)(object)EditorGUILayout.EnumFlagsField(label, (Enum)(object)value);
            }
            else
            {
                value = (T)(object)EditorGUILayout.EnumPopup(label, (Enum)(object)value);
            }
            if (EditorGUI.EndChangeCheck())
            {
                ValueEntry.SmartValue = value;
            }
        }
    }
}
