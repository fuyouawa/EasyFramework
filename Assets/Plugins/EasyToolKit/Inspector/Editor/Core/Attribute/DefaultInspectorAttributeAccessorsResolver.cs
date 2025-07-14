using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    public class DefaultInspectorAttributeAccessorsResolver : InspectorAttributeAccessorsResolver
    {
        public static readonly DefaultInspectorAttributeAccessorsResolver Instance = new DefaultInspectorAttributeAccessorsResolver();

        public override IAttributeAccessor[] GetAttributeAccessors(InspectorProperty property)
        {
            var accessors = new List<IAttributeAccessor>
            {
                new MemberAttributeAccessor(property.Info.MemberInfo)
            };
            if (property.Info.PropertyType != null)
            {
                accessors.Add(new TypeAttributeAccessor(property.Info.PropertyType));
            }

            return accessors.ToArray();
        }
    }
}
