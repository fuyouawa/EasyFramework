using System;
using EasyToolKit.Core;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Super)]
    public class PrimitiveValueConflictDrawer<T> : EasyValueDrawer<T>
    {
        protected override bool CanDrawValueType(Type valueType)
        {
            return valueType.IsBasic() || valueType.IsSubclassOf(typeof(UnityEngine.Object));
        }

        protected override void OnDrawProperty(GUIContent label)
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
