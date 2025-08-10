using System.Collections;
using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public class PropertyValueCollectionWrapper<TValue, TBaseValue> : IPropertyValueCollectionWrapper<TValue, TBaseValue>
        where TValue : TBaseValue
    {
        private readonly IPropertyValueCollection<TBaseValue> _collection;

        public PropertyValueCollectionWrapper(IPropertyValueCollection<TBaseValue> collection)
        {
            _collection = collection;
        }

        public TValue this[int index]
        {
            get => (TValue)_collection[index];
            set => _collection[index] = value;
        }

        object IPropertyValueCollection.this[int index]
        {
            get => (_collection as IPropertyValueCollection)[index];
            set => (_collection as IPropertyValueCollection)[index] = value;
        }

        public int Count => _collection.Count;

        public InspectorProperty Property => _collection.Property;

        public bool Dirty => _collection.Dirty;

        public bool ApplyChanges()
        {
            return _collection.ApplyChanges();
        }

        public void ForceMakeDirty()
        {
            _collection.ForceMakeDirty();
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (TBaseValue item in _collection)
            {
                yield return (TValue)item;
            }
        }

        public void Update()
        {
            _collection.Update();
        }

        public void Dispose()
        {
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}