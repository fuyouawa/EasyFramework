using System;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class EasyValueDrawer<T> : EasyDrawer
    {
        private IPropertyValueEntry<T> _valueEntry;

        public IPropertyValueEntry<T> ValueEntry
        {
            get
            {
                if (_valueEntry == null)
                {
                    _valueEntry = Property.ValueEntry as IPropertyValueEntry<T>;
                }

                return _valueEntry;
            }
        }

        protected sealed override bool CanDrawProperty(InspectorProperty property)
        {
            return property.ValueEntry != null &&
                   property.ValueEntry.ValueType == typeof(T) &&
                   CanDrawValueType(property.ValueEntry.ValueType) &&
                   CanDrawValueProperty(property);
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
