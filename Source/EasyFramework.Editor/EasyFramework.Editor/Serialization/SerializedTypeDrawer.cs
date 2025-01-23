using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.Editor
{
    public class SerializedTypeDrawer : OdinValueDrawer<SerializedType>
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
