using System;
using System.Collections.Generic;
using System.Text;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyGameFramework
{
    public class SerializedListDrawer<T> : OdinValueDrawer<SerializedList<T>>
    {
        private InspectorProperty _collection;
        protected override void Initialize()
        {
            base.Initialize();

            _collection = Property.Children["Collection"];
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            _collection.Draw(label);
        }
    }
}
