using System;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class EasyValueDrawer<T> : EasyDrawer
    {
        private IInspectorValueEntry<T> _valueEntry;

        public IInspectorValueEntry<T> ValueEntry
        {
            get
            {
                if (_valueEntry == null)
                {
                    _valueEntry = Property.ValueEntry as IInspectorValueEntry<T>;
                }

                return _valueEntry;
            }
        }

        public sealed override bool CanDrawProperty(InspectorProperty property)
        {
            return property.ValueEntry != null && property.ValueEntry.ValueType == typeof(T) &&
                   CanDrawValueType(property.ValueEntry.ValueType) && CanDrawValueProperty(property);
        }

        protected virtual bool CanDrawValueType(Type valueType)
        {
            return true;
        }


        protected virtual bool CanDrawValueProperty(InspectorProperty property)
        {
            return true;
        }
    }
}
