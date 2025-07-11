using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(0.1)]
    public class CompositeDrawer : EasyDrawer
    {
        protected override void OnDrawProperty(GUIContent label)
        {
            if (Property.Children.Count == 0)
            {
                if (label != null)
                {
                    var rect = EditorGUILayout.GetControlRect();
                    GUI.Label(rect, label);
                }

                return;
            }

            
            if (label == null)
            {
                foreach (var child in Property.Children)
                {
                    child.Draw(child.Label);
                }
            }
            else
            {
                EditorGUI.indentLevel++;
                foreach (var child in Property.Children)
                {
                    child.Draw(child.Label);
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}
