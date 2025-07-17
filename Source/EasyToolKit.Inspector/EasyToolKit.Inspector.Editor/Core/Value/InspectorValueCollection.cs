using System;
using System.Collections;
using System.Collections.Generic;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public sealed class InspectorValueCollection<TValue> : IInspectorValueCollection<TValue>
    {
        public InspectorProperty Property { get; private set; }
        private readonly TValue[] _values;

        public bool Dirty { get; private set; }

        public InspectorValueCollection(InspectorProperty property)
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

        void IInspectorValueCollection.Update()
        {
            for (int i = 0; i < Property.Tree.Targets.Length; i++)
            {
                var target = Property.Tree.Targets[i];
                var value = Property.Info.ValueAccessor.GetValue(target);
                _values[i] = (TValue)value;
            }

            ClearDirty();
        }

        bool IInspectorValueCollection.ApplyChanges()
        {
            if (!Dirty) return false;

            for (int i = 0; i < _values.Length; i++)
            {
                var target = Property.Tree.Targets[i];
                var value = _values[i];
                Property.Info.ValueAccessor.SetValue(target, value);
            }

            ClearDirty();
            return true;
        }
    }
}
