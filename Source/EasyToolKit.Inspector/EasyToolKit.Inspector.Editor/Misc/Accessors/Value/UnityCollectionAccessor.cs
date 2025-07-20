using EasyToolKit.Core.Editor;
using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using UnityEditor;

namespace EasyToolKit.Inspector.Editor
{
    //TODO UnityCollectionAccessor性能不好，待重构
    public class UnityCollectionAccessor<TOwner, TCollection, TElement> : ValueAccessor<TOwner, TCollection>
        where TCollection : IList<TElement>, new()
    {
        private static readonly Func<SerializedProperty, TElement> ElementValueGetter = SerializedPropertyUtility.GetValueGetter<TElement>();
        private static readonly Action<SerializedProperty, TElement> ElementValueSetter = SerializedPropertyUtility.GetValueSetter<TElement>();

        private readonly SerializedProperty _serializedProperty;
        private TCollection _collectionCache;

        public UnityCollectionAccessor(SerializedProperty serializedProperty)
        {
            _serializedProperty = serializedProperty.Copy();
        }

        public override void SetValue(ref TOwner owner, TCollection collection)
        {
            for (int i = 0; i < collection.Count - _serializedProperty.arraySize; i++)
            {
                _serializedProperty.InsertArrayElementAtIndex(0);
            }

            for (int i = 0; i < collection.Count; i++)
            {
                var element = _serializedProperty.GetArrayElementAtIndex(i);
                ElementValueSetter(element, collection[i]);
            }
        }

        public override TCollection GetValue(ref TOwner owner)
        {
            if (_collectionCache == null || _collectionCache.Count != _serializedProperty.arraySize)
            {
                ResizeCollection(_serializedProperty.arraySize);
            }

            for (int i = 0; i < _serializedProperty.arraySize; i++)
            {
                var element = _serializedProperty.GetArrayElementAtIndex(i);
                _collectionCache[i] = ElementValueGetter(element);
            }
            return _collectionCache;
        }

        private void ResizeCollection(int size)
        {
            if (typeof(TCollection).IsArray)
            {
                var array = new TElement[size];
                _collectionCache = (TCollection)(object)array;
            }
            else
            {
                if (_collectionCache == null)
                {
                    _collectionCache = new TCollection();
                }
                _collectionCache.Resize(size);
            }
        }
    }
}
