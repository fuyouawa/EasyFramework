using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;

namespace EasyFramework.Utilities
{
    [Serializable]
    public class SerializedList<T> : BinarySerailizedValue<List<T>>, IEnumerable<T>
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

        public void Add([CanBeNull] T item)
        {
            _collection.Add(item);
        }

        public void Remove([CanBeNull] T item)
        {
            _collection.Remove(item);
        }

        public int IndexOf([CanBeNull] T item)
        {
            return _collection.IndexOf(item);
        }

        public void RemoveAt(int index)
        {
            _collection.RemoveAt(index);
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

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }
    }
}
