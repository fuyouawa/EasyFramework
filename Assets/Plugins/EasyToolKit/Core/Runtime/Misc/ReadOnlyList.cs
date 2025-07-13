using System.Collections;
using System.Collections.Generic;

namespace EasyToolKit.Core
{
    public interface IReadOnlyList : IEnumerable
    {
        int Count { get; }
        object this[int index] { get; set; }
    }

    public class ReadOnlyList : IReadOnlyList
    {
        private readonly IList _list;

        public ReadOnlyList(IList list)
        {
            _list = list;
        }

        public IEnumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int Count => _list.Count;

        public object this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }
    }
}
