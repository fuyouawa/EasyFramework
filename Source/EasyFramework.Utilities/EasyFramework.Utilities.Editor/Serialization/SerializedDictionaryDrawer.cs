using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.Utilities
{
    public class SerializedDictionaryDrawer<TKey, TValue> : OdinValueDrawer<SerializedDictionary<TKey, TValue>>
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
