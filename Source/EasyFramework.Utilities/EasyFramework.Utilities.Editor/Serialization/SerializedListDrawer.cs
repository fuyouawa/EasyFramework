using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyGameFramework
{
    public class SerializedListDrawer<T> : OdinValueDrawer<SerializedList<T>>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            foreach (var child in Property.Children)
            {
                child.Draw(label);
            }
        }
    }
}
