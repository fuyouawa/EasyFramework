using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class CompositeDrawer<T> : EasyValueDrawer<T>
    {
        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            return property.Children != null;
        }

        protected override void OnDrawProperty(GUIContent label)
        {
            if (label == null)
            {
                for (var i = 0; i < Property.Children!.Count; i++)
                {
                    var child = Property.Children[i];
                    child.Draw(child.Label);
                }
            }
            else
            {
                EditorGUI.indentLevel++;
                for (var i = 0; i < Property.Children!.Count; i++)
                {
                    var child = Property.Children[i];
                    child.Draw(child.Label);
                }

                EditorGUI.indentLevel--;
            }
        }
    }
}
