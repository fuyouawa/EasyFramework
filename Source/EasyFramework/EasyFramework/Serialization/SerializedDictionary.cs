using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace EasyFramework
{
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : SerializedDictionaryBase<TKey, TValue>
        where TKey : notnull
    {
        public override Dictionary<TKey, TValue> Value
        {
            get => _collection;
            set => _collection = value;
        }

        [NonSerialized, ShowInInspector]
        private Dictionary<TKey, TValue> _collection;

        public SerializedDictionary()
        {
            _collection = new Dictionary<TKey, TValue>();
        }

        public SerializedDictionary(int capacity)
        {
            _collection = new Dictionary<TKey, TValue>(capacity);
        }

        public SerializedDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _collection = new Dictionary<TKey, TValue>(dictionary);
        }

        public SerializedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            _collection = new Dictionary<TKey, TValue>(collection);
        }
    }
}
