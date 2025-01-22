using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace EasyFramework.Utilities
{
    public class SerializedListDrawerSettings
    {
        public Func<object> OnAddElement;
        public Action OnAddElementVoid;
        public bool IsReadOnly;
    }

    [Serializable]
    public class SerializedList<T> : SerializedListBase<T>
    {
        public override List<T> Value
        {
            get => _collection;
            set => _collection = value;
        }

        public SerializedListDrawerSettings DrawerSettings { get; } = new SerializedListDrawerSettings();

        [NonSerialized, ShowInInspector]
        [ListDrawerSettings(CustomAddFunction = nameof(InternalOnAddElement))]
        private List<T> _collection;

        public SerializedList()
        {
            _collection = new List<T>();
        }

        public SerializedList(int capacity)
        {
            _collection = new List<T>(capacity);
        }

        public SerializedList(IEnumerable<T> collection)
        {
            _collection = new List<T>(collection);
        }

        private void InternalOnAddElement()
        {
            if (DrawerSettings.OnAddElementVoid != null)
            {
                DrawerSettings.OnAddElementVoid();
            }
            else if (DrawerSettings.OnAddElement != null)
            {
                _collection.Add((T)DrawerSettings.OnAddElement());
            }
            else
            {
                _collection.Add(default);
            }
        }
    }
}
