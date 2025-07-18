using System;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class InspectorCollectionResolver : InspectorPropertyResolver
    {
        public abstract Type ElementType { get; }
        public abstract void RemoveElement(int index);
        public abstract void InsertElement(int index);
        public abstract void MoveElement(int sourceIndex, int destinationIndex);
    }

    public abstract class InspectorCollectionResolver<TCollection> : InspectorCollectionResolver
    {
    }

    public abstract class InspectorCollectionResolver<TCollection, TElement> : InspectorCollectionResolver<TCollection>
        where TCollection : ICollection<TElement>
    {
        public override Type ElementType => typeof(TElement);
    }
}
