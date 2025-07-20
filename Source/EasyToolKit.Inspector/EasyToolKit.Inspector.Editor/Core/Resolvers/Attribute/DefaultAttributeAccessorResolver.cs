using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    public class DefaultAttributeAccessorResolver : AttributeAccessorResolver
    {
        private IAttributeAccessor[] _attributes;

        public override IAttributeAccessor[] GetAttributeAccessors()
        {
            if (_attributes != null)
            {
                return _attributes;
            }
            var accessors = new List<IAttributeAccessor>
            {
                new MemberAttributeAccessor(Property.Info.MemberInfo)
            };
            if (Property.Info.TypeOfProperty != null)
            {
                accessors.Add(new TypeAttributeAccessor(Property.Info.TypeOfProperty));
            }

            _attributes = accessors.ToArray();
            return _attributes;
        }
    }
}
