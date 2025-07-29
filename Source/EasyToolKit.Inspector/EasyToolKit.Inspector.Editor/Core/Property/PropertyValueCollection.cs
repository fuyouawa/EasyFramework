using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public interface IPropertyValueCollection : IReadOnlyList
    {
        InspectorProperty Property { get; }
        bool Dirty { get; }
        internal void Update();
        internal bool ApplyChanges();
    }

    public interface IPropertyValueCollection<T> : IPropertyValueCollection, IReadOnlyList<T>
    {
        new T this[int index] { get; set; }
        new int Count { get; }
    }

    public sealed class PropertyValueCollection<TValue> : IPropertyValueCollection<TValue>
    {
        public InspectorProperty Property { get; private set; }
        private readonly TValue[] _values;
        private bool _firstUpdated = false;

        public bool Dirty { get; private set; }

        public PropertyValueCollection(InspectorProperty property)
        {
            Property = property;
            _values = new TValue[property.Tree.Targets.Length];
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return ((IEnumerable<TValue>)_values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _values.Length;

        object IReadOnlyList.this[int index]
        {
            get => this[index];
            set => this[index] = (TValue)value;
        }

        public TValue this[int index]
        {
            get => _values[index];
            set
            {
                if (!EqualityComparer<TValue>.Default.Equals(_values[index], value))
                {
                    if (Property.IsReadOnly)
                    {
                        Debug.LogWarning($"Property '{Property.Info.PropertyPath}' cannot be edited.");
                        return;
                    }

                    _values[index] = value;
                    MakeDirty();
                }
            }
        }

        private void MakeDirty()
        {
            if (!Dirty)
            {
                Dirty = true;
                Property.Tree.SetPropertyDirty(Property);
            }
        }

        private void ClearDirty()
        {
            Dirty = false;
        }

        void IPropertyValueCollection.Update()
        {
            if (Property.Info.IsLogicRoot)
            {
                if (!_firstUpdated)
                {
                    for (int i = 0; i < Property.Tree.Targets.Length; i++)
                    {
                        _values[i] = (TValue)(object)Property.Tree.Targets[i];
                    }
                }
            }
            else
            {
                Assert.IsNotNull(Property.Parent.ValueEntry);
                Assert.IsNotNull(Property.Info.ValueAccessor);

                for (int i = 0; i < Property.Tree.Targets.Length; i++)
                {
                    var owner = Property.Parent.ValueEntry.WeakValues[i];
                    var value = (TValue)Property.Info.ValueAccessor.GetWeakValue(owner);
                    _values[i] = value;
                }
            }

            ClearDirty();

            _firstUpdated = true;
        }

        bool IPropertyValueCollection.ApplyChanges()
        {
            if (!Dirty) return false;
            
            Assert.IsNotNull(Property.Parent.ValueEntry);
            Assert.IsNotNull(Property.Info.ValueAccessor);

            for (int i = 0; i < _values.Length; i++)
            {
                var owner = Property.Parent.ValueEntry.WeakValues[i];
                var value = _values[i];
                Property.Info.ValueAccessor.SetWeakValue(owner, value);
            }

            ClearDirty();
            return true;
        }
    }
}
