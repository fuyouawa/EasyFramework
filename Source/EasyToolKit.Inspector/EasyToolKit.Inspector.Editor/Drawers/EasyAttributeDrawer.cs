using System;

namespace EasyToolKit.Inspector.Editor
{
    public abstract class EasyAttributeDrawer<TAttribute> : EasyDrawer
        where TAttribute : Attribute
    {
        private TAttribute _attribute;

        public TAttribute Attribute
        {
            get
            {
                if (_attribute == null)
                {
                    _attribute = Property.GetAttribute<TAttribute>();
                }
                return _attribute;
            }
        }

        protected sealed override bool CanDrawProperty(InspectorProperty property)
        {
            if (property.ValueEntry != null && !CanDrawValueType(property.ValueEntry.ValueType))
            {
                return false;
            }
            return property.GetAttribute<TAttribute>() != null && CanDrawAttributeProperty(property);
        }

        protected virtual bool CanDrawValueType(Type valueType)
        {
            return true;
        }

        protected virtual bool CanDrawAttributeProperty(InspectorProperty property)
        {
            return true;
        }
    }

    public abstract class EasyAttributeDrawer<TAttribute, TValue> : EasyAttributeDrawer<TAttribute>
        where TAttribute : Attribute
    {
        protected override bool CanDrawAttributeProperty(InspectorProperty property)
        {
            return property.ValueEntry != null &&
                   property.ValueEntry.ValueType == typeof(TValue) &&
                   CanDrawAttributeValueProperty(property);
        }

        protected virtual bool CanDrawAttributeValueProperty(InspectorProperty property)
        {
            return true;
        }
    }
}
