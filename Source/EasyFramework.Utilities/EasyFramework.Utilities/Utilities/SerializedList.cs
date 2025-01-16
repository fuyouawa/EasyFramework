using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace EasyGameFramework
{
    [Serializable, InlineProperty, HideReferenceObjectPicker]
    public class SerializedList<T> : IEnumerable<T>, ISerializationCallbackReceiver
    {
        public Func<T> OnAddElement;
        public Action OnAddElementVoid;

        [NonSerialized, ShowInInspector]
        [ListDrawerSettings(CustomAddFunction = "InternalOnAddElement")]
        public List<T> Collection;

        public SerializedList()
        {
            Collection = new List<T>();
        }

        public SerializedList(int capacity)
        {
            Collection = new List<T>(capacity);
        }

        public SerializedList(IEnumerable<T> collection)
        {
            Collection = new List<T>(collection);
        }

        public void Add([CanBeNull] T item)
        {
            Collection.Add(item);
        }

        public void Remove([CanBeNull] T item)
        {
            Collection.Remove(item);
        }

        public int IndexOf([CanBeNull] T item)
        {
            return Collection.IndexOf(item);
        }

        public void RemoveAt(int index)
        {
            Collection.RemoveAt(index);
        }

        private void InternalOnAddElement()
        {
            if (OnAddElementVoid != null)
            {
                OnAddElementVoid();
            }
            else if (OnAddElement != null)
            {
                Collection.Add(OnAddElement());
            }
            else
            {
                Collection.Add(default);
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return Collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Collection.GetEnumerator();
        }

        [SerializeField] private byte[] _serializedCollection;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _serializedCollection = SerializationUtility.SerializeValue(Collection, DataFormat.Binary);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Collection = SerializationUtility.DeserializeValue<List<T>>(_serializedCollection, DataFormat.Binary);
        }
    }
}
