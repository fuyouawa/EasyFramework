using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public interface IPropertyValueCollection : IReadOnlyList
    {
        
    }

    public interface IPropertyValueCollection<T> : IPropertyValueCollection, IReadOnlyList<T>
    {
        new T this[int index] { get; set; }
    }
}
