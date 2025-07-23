using System;
using System.Collections.Generic;
using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    public class DefaultAttributeResolver : AttributeResolver
    {
        private Attribute[] _attributes;

        public override Attribute[] GetAttributes()
        {
            if (_attributes != null)
            {
                return _attributes;
            }

            var total = new List<Attribute>();

            var memberInfo = Property.Info.TryGetMemberInfo();
            if (memberInfo != null)
            {
                total.AddRange(memberInfo.GetCustomAttributes());
            }

            if (Property.Info.PropertyType != null)
            {
                total.AddRange(Property.Info.PropertyType.GetCustomAttributes());
            }

            _attributes = total.ToArray();
            return _attributes;
        }
    }
}
