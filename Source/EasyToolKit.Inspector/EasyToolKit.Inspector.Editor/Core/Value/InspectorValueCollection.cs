using System;
using System.Collections;
using System.Collections.Generic;
using EasyToolKit.Core;

namespace EasyToolKit.Inspector.Editor
{
    public sealed class InspectorValueCollection<TValue> : IInspectorValueCollection<TValue>
    {
        private readonly InspectorProperty _property;
        private readonly TValue[] _values;
        
        public bool Dirty { get; private set; }
        public int Count => _values.Length;

        public InspectorValueCollection(InspectorProperty property)
        {
            _property = property;
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
                _property.Tree.SetPropertyDirty(_property);
            }
        }

        private void ClearDirty()
        {
            Dirty = false;
        }

        void IInspectorValueCollection.Update()
        {
            for (int i = 0; i < _property.Tree.Targets.Length; i++)
            {
                var target = _property.Tree.Targets[i];
                var value = _property.Info.ValueAccessor.GetValue(target);
                _values[i] = (TValue)value;
            }

            ClearDirty();
        }

        bool IInspectorValueCollection.ApplyChanges()
        {
            if (!Dirty) return false;

            for (int i = 0; i < _values.Length; i++)
            {
                var target = _property.Tree.Targets[i];
                var value = _values[i];
                _property.Info.ValueAccessor.SetValue(target, value);
            }
            ClearDirty();
            return true;
        }
    }
}
