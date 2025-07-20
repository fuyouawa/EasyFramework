using System;
using EasyToolKit.Core;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Super)]
    public class ValueConflictDrawer<T> : EasyValueDrawer<T>
    {
        protected override bool CanDrawValueType(Type valueType)
        {
            return valueType.IsBasic() || valueType.IsSubclassOf(typeof(UnityEngine.Object));
        }

        protected override void DrawProperty(GUIContent label)
        {
            if (ValueEntry.IsConflicted())
            {
                EditorGUI.showMixedValue = true;
            }

            CallNextDrawer(label);
            EditorGUI.showMixedValue = false;
        }
    }
}
