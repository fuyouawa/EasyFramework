using System;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    public interface ICollectionResolver
    {
        Type ElementType { get; }

        void InsertElement(object value);
        void RemoveElement(object value);
    }
    
    public abstract class CollectionResolverBase : PropertyResolver, ICollectionResolver
    {
        public abstract Type ElementType { get; }

        public abstract void InsertElement(object value);
        public abstract void RemoveElement(object value);
    }
}
