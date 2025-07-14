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

        public override bool CanDrawProperty(InspectorProperty property)
        {
            return property.ValueEntry != null && property.ValueEntry.ValueType == typeof(T) &&
                   ValueTypeFilter(property.ValueEntry.ValueType) && CanDrawValueProperty(property);
        }

        protected virtual bool ValueTypeFilter(Type valueType)
        {
            return true;
        }


        protected virtual bool CanDrawValueProperty(InspectorProperty property)
        {
            return true;
        }
    }
}
