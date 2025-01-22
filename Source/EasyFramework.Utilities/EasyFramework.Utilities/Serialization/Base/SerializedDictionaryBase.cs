using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace EasyFramework.Utilities
{
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public abstract class SerializedDictionaryBase<TKey, TValue> :
        GenericSerailizedValue<Dictionary<TKey, TValue>>,
        IEnumerable<KeyValuePair<TKey, TValue>>
        where TKey : notnull
    {
        public void Clear()
        {
            Value.Clear();
        }

        public int Count => Value.Count;

        public void Add(TKey key, TValue value)
        {
            Value.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return Value.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return Value.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return Value.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get => Value[key];
            set => Value[key] = value;
        }

        public ICollection<TKey> Keys => Value.Keys;

        public ICollection<TValue> Values => Value.Values;

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Value.GetEnumerator();
        }
    }
}
