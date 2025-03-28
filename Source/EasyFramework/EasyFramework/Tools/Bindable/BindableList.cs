using System.Collections;
using System.Collections.Generic;

namespace EasyFramework
{
    public interface IReadOnlyBindableList<T> : IReadOnlyList<T>
    {
        EasyEvent<T> OnAddedElement { get; }
        EasyEvent<T> OnRemovedElement { get; }
        EasyEvent OnClearElements { get; }

        EasyEvent OnElementChanged { get; }
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
            OnAddedElement.Invoke(item);
            OnElementChanged.Invoke();
        }

        public void Clear()
        {
            _list.Clear();
            OnClearElements.Invoke();
            OnElementChanged.Invoke();
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
                OnRemovedElement.Invoke(item);
                OnElementChanged.Invoke();
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
            OnRemovedElement.Invoke(item);
            OnElementChanged.Invoke();
        }

        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public EasyEvent<T> OnAddedElement { get; } = new EasyEvent<T>();
        public EasyEvent<T> OnRemovedElement { get; } = new EasyEvent<T>();
        public EasyEvent OnClearElements { get; } = new EasyEvent();
        public EasyEvent OnElementChanged { get; } = new EasyEvent();
    }
}
