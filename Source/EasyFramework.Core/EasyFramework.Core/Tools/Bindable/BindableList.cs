using System.Collections;
using System.Collections.Generic;

namespace EasyFramework.Core
{
    public interface IReadOnlyBindableList<T> : IReadOnlyList<T>
    {
        IEasyEvent<T> OnAddedElement { get; }
        IEasyEvent<T> OnRemovedElement { get; }
        IEasyEvent OnClearElements { get; }

        IEasyEvent OnElementChanged { get; }
    }

    public interface IBindableList<T> : IReadOnlyBindableList<T>, IList<T>
    {
    }

    public class BindableList<T> : IBindableList<T>
    {
        private readonly EasyEvent<T> _onAddedElement = new EasyEvent<T>();
        public IEasyEvent<T> OnAddedElement => _onAddedElement;

        private readonly EasyEvent<T> _onRemovedElement = new EasyEvent<T>();
        public IEasyEvent<T> OnRemovedElement => _onRemovedElement;

        private readonly EasyEvent _onClearElements = new EasyEvent();
        public IEasyEvent OnClearElements => _onClearElements;

        private readonly EasyEvent _onElementChanged = new EasyEvent();
        public IEasyEvent OnElementChanged => _onElementChanged;

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
            _onAddedElement.Invoke(item);
            _onElementChanged.Invoke();
        }

        public void Clear()
        {
            _list.Clear();
            _onClearElements.Invoke();
            _onElementChanged.Invoke();
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
                _onRemovedElement.Invoke(item);
                _onElementChanged.Invoke();
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
            _onRemovedElement.Invoke(item);
            _onElementChanged.Invoke();
        }

        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }
    }
}
