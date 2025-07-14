using System;

namespace EasyToolKit.Inspector.Editor
{
    public class EasyAttributeDrawer<TAttribute> : EasyDrawer
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

        public override bool CanDrawProperty(InspectorProperty property)
        {
            if (property.ValueEntry != null && !ValueTypeFilter(property.ValueEntry.ValueType))
            {
                return false;
            }
            return property.GetAttribute<TAttribute>() != null && CanDrawAttributeProperty(property);
        }

        protected virtual bool ValueTypeFilter(Type valueType)
        {
            return true;
        }

        protected virtual bool CanDrawAttributeProperty(InspectorProperty property)
        {
            return true;
        }
    }
}
