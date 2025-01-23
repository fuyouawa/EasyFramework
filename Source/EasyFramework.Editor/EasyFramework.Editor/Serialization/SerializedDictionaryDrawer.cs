using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework
{
    public class SerializedDictionaryDrawer<TKey, TValue> : OdinValueDrawer<SerializedDictionary<TKey, TValue>>
    {
        private InspectorProperty _collection;
        protected override void Initialize()
        {
            base.Initialize();
            _collection = Property.Children[0];
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var val = ValueEntry.SmartValue;

            var attr = _collection.GetAttribute<DictionaryDrawerSettings>();
            attr.KeyLabel = val.DrawerSettings.KeyLabel;
            attr.ValueLabel = val.DrawerSettings.ValueLabel;

            _collection.Draw(label);
        }
    }
}
