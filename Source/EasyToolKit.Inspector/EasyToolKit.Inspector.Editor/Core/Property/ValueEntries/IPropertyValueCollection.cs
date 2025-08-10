using System;
using System.Collections;
using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public interface IPropertyValueCollection : IEnumerable, IDisposable
    {
        object this[int index] { get; set; }
        int Count { get; }

        InspectorProperty Property { get; }
        bool Dirty { get; }
        void ForceMakeDirty();

        void Update();
        bool ApplyChanges();
    }

    public interface IPropertyValueCollection<T> : IPropertyValueCollection, IReadOnlyList<T>
    {
        new T this[int index] { get; set; }
        new int Count { get; }
    }
}