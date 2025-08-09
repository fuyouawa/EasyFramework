using System;
using EasyToolKit.Core;
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

                    if (_valueEntry == null)
                    {
                        Property.Update(true);
                        _valueEntry = Property.ValueEntry as IPropertyValueEntry<T>;
                    }
                }

                return _valueEntry;
            }
        }

        protected sealed override bool CanDrawProperty(InspectorProperty property)
        {
            if (property.ValueEntry == null)
            {
                return false;
            }

            var valueType = property.ValueEntry.BaseValueType;
            return (valueType == typeof(T) || valueType.IsInheritsFrom<T>()) &&
                   CanDrawValueType(valueType) &&
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
