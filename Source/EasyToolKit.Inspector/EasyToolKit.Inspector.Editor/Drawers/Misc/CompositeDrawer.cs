using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(-1)]
    public class CompositeDrawer : EasyDrawer
    {
        public override bool CanDrawProperty(InspectorProperty property)
        {
            return property.Children != null && property.Children.Count > 0;
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
