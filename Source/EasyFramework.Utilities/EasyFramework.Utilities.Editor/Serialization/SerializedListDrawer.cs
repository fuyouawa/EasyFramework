using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.Utilities
{
    public class SerializedListDrawer<T> : OdinValueDrawer<SerializedList<T>>
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

            var attr = _collection.GetAttribute<ListDrawerSettingsAttribute>();
            attr.IsReadOnly = val.DrawerSettings.IsReadOnly;

            _collection.Draw(label);
        }
    }
}
