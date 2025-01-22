using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace EasyFramework.Utilities
{
    public class SerializedListDrawerSettings<T>
    {
        public Func<T> OnAddElement;
        public Action OnAddElementVoid;
    }


    [Serializable]
    public class SerializedList<T> : SerializedListBase<T>
    {
        public override List<T> Value
        {
            get => _collection;
            set => _collection = value;
        }

        public SerializedListDrawerSettings<T> DrawerSettings { get; } = new SerializedListDrawerSettings<T>();

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
                _collection.Add(DrawerSettings.OnAddElement());
            }
            else
            {
                _collection.Add(default);
            }
        }
    }
}
