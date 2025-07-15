using System;
using System.Collections;
using System.Collections.Generic;

namespace EasyToolKit.Core
{
    public interface IReadOnlyBindableList<T> : IReadOnlyList<T>
    {
        event Action<T> OnAddedElement;
        event Action<T> OnRemovedElement;
        event Action OnClearElements;

        event Action OnElementChanged;
    }

    public interface IBindableList<T> : IReadOnlyBindableList<T>, IList<T>
    {
    }

    public class BindableList<T> : IBindableList<T>
    {

        private List<T> _list;

        public BindableList()
        {
            _list = new List<T>();
        }

        public BindableList(int capacity)
        {
            _list = new List<T>(capacity);
        }

        public int Count => _list.Count;

        public event Action<T> OnAddedElement;
        public event Action<T> OnRemovedElement;
        public event Action OnClearElements;
        public event Action OnElementChanged;

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _list.Add(item);
            OnAddedElement?.Invoke(item);
            OnElementChanged?.Invoke();
        }

        public void Clear()
        {
            _list.Clear();
            OnClearElements?.Invoke();
            OnElementChanged?.Invoke();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var suc = _list.Remove(item);
            if (suc)
            {
                OnRemovedElement?.Invoke(item);
                OnElementChanged?.Invoke();
            }
            return suc;
        }

        bool ICollection<T>.IsReadOnly => false;

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            var item = _list[index];
            _list.RemoveAt(index);
            OnRemovedElement?.Invoke(item);
            OnElementChanged?.Invoke();
        }

        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }
    }
}
