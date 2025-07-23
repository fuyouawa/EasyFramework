using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Value + 1)]
    public class ReferenceDrawer<T> : EasyValueDrawer<T>
    {
        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            return property.Children != null;
        }

        protected override void DrawProperty(GUIContent label)
        {
            if (label == null)
            {
                CallNextDrawer(null);
                return;
            }
            
            Property.State.Expanded = EasyEditorGUI.Foldout(Property.State.Expanded, label);

            if (Property.State.Expanded)
            {
                EditorGUI.indentLevel++;
                CallNextDrawer(null);
                EditorGUI.indentLevel--;
            }
        }
    }
}
