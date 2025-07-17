using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public interface IInspectorValueCollection : IReadOnlyList
    {
        InspectorProperty Property { get; }
        bool Dirty { get; }
        internal void Update();
        internal bool ApplyChanges();
    }

    public interface IInspectorValueCollection<T> : IInspectorValueCollection, IReadOnlyList<T>
    {
        new T this[int index] { get; set; }
        new int Count { get; }
    }
}
