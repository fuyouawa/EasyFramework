using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace EasyFramework.Utilities
{
    [Serializable]
    public class SerializedList<T> : SerializedListBase<T>
    {
        public Func<T> OnAddElement;
        public Action OnAddElementVoid;

        public override List<T> Value
        {
            get => _collection;
            set => _collection = value;
        }

        [NonSerialized, ShowInInspector]
        [ListDrawerSettings(CustomAddFunction = "InternalOnAddElement")]
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
            if (OnAddElementVoid != null)
            {
                OnAddElementVoid();
            }
            else if (OnAddElement != null)
            {
                _collection.Add(OnAddElement());
            }
            else
            {
                _collection.Add(default);
            }
        }
    }
}
