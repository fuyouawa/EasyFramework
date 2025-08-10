using System;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Value + 100)]
    public class GuidDrawer : EasyValueDrawer<Guid>
    {
        protected override void DrawProperty(GUIContent label)
        {
            var value = ValueEntry.SmartValue;

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField(label, value.ToString("D"));
            EditorGUI.EndDisabledGroup();
        }
    }
}
