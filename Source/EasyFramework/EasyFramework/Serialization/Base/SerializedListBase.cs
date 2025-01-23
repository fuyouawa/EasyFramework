using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace EasyFramework
{
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public abstract class SerializedListBase<T> :
        GenericSerailizedValue<List<T>>,
        IEnumerable<T>
    {
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        public void Add(T item)
        {
            Value.Add(item);
        }

        public void Clear()
        {
            Value.Clear();
        }

        public bool Contains(T item)
        {
            return Value.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Value.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return Value.Remove(item);
        }

        public int Count => Value.Count;

        public int IndexOf(T item)
        {
            return Value.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            Value.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Value.RemoveAt(index);
        }

        public T this[int index]
        {
            get => Value[index];
            set => Value[index] = value;
        }
    }
}
