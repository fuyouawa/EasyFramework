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
            var val = ValueEntry.SmartValue;
            if (val.GetType().IsDefined(typeof(FlagsAttribute), false))
            {
                val = (T)(object)EditorGUILayout.EnumFlagsField(label, (Enum)(object)ValueEntry.SmartValue);
            }
            else
            {
                val = (T)(object)EditorGUILayout.EnumPopup(label, (Enum)(object)ValueEntry.SmartValue);
            }
            ValueEntry.SmartValue = val;
        }
    }
}
