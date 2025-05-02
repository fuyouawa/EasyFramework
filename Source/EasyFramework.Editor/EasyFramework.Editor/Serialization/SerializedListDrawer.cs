using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.Editor
{
    public class SerializedListDrawer<T> : OdinValueDrawer<SerializedList<T>>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Property.Children[0].Draw(label);
        }
    }
}
