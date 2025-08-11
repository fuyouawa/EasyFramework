using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class ListResolver<TElement> : OrderedCollectionResolverBase<TElement>
    {
        private int _minLength;
        private readonly Dictionary<int, InspectorPropertyInfo> _propertyInfosByIndex =
            new Dictionary<int, InspectorPropertyInfo>();

        protected override void Initialize()
        {
            _minLength = int.MaxValue;
            foreach (var value in Property.ValueEntry.WeakValues)
            {
                if (value == null)
                {
                    _minLength = 0;
                    break;
                }
                _minLength = Mathf.Min(_minLength, ((IList<TElement>)value).Count);
            }
        }

        public override int ChildNameToIndex(string name)
        {
            throw new NotSupportedException();
        }

        public override int GetChildCount()
        {
            return _minLength;
        }

        public override InspectorPropertyInfo GetChildInfo(int childIndex)
        {
            if (_propertyInfosByIndex.TryGetValue(childIndex, out var info))
            {
                return info;
            }

            info = InspectorPropertyInfo.CreateForValue(
                ElementType,
                $"Index {childIndex}",  //TODO use unity property name
                new GenericValueAccessor<IList<TElement>, TElement>(
                    (list) => list[childIndex],
                    (list, value) => list[childIndex] = value
                )
            );

            _propertyInfosByIndex[childIndex] = info;
            return info;
        }

        private IList<TElement> GetCollection(int targetIndex)
        {
            return (IList<TElement>)Property.ValueEntry.WeakValues[targetIndex];
        }

        protected override void InsertElement(int targetIndex, TElement value)
        {
            var collection = GetCollection(targetIndex);
            collection.Add(value);
        }

        protected override void InsertElementAt(int targetIndex, int index, TElement value)
        {
            var collection = GetCollection(targetIndex);
            collection.Insert(index, value);
        }

        protected override void MoveElemenetAt(int targetIndex, int sourceIndex, int destinationIndex)
        {
            throw new NotImplementedException();
        }

        protected override void RemoveElement(int targetIndex, TElement value)
        {
            var collection = GetCollection(targetIndex);
            collection.Remove(value);
        }

        protected override void RemoveElementAt(int targetIndex, int index)
        {
            var collection = GetCollection(targetIndex);
            collection.RemoveAt(index);
        }
    }
}