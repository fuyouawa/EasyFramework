using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class EasyEventDrawer : OdinValueDrawer<EasyEvent>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Property.Children[0].Draw(label);
        }
    }

    public class EasyEventDrawer<T> : OdinValueDrawer<EasyEvent<T>>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Property.Children[0].Draw(label);
        }
    }

    public class EasyEventDrawer<T1, T2> : OdinValueDrawer<EasyEvent<T1, T2>>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Property.Children[0].Draw(label);
        }
    }

    public class EasyEventDrawer<T1, T2, T3> : OdinValueDrawer<EasyEvent<T1, T2, T3>>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Property.Children[0].Draw(label);
        }
    }
}
