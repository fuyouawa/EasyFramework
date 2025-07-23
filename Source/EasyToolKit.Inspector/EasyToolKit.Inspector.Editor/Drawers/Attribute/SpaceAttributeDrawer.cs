using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Super)]
    public class SpaceAttributeDrawer : EasyAttributeDrawer<SpaceAttribute>
    {
        protected override void DrawProperty(GUIContent label)
        {
            if (Attribute.height == 0)
            {
                EditorGUILayout.Space();
            }
            else
            {
                GUILayout.Space(Attribute.height);
            }

            CallNextDrawer(label);
        }
    }
}
