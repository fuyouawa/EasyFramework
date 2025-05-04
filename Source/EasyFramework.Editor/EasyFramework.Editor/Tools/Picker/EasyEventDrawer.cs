using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.Editor
{
    public class EasyEventBaseDrawer : OdinValueDrawer<EasyEventBase>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Property.Children[0].Draw(label);
        }
    }
}
