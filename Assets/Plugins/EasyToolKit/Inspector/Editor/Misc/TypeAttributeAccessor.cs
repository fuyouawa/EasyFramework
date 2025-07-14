using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyToolKit.Inspector.Editor
{
    public class TypeAttributeAccessor : IAttributeAccessor
    {
        private readonly Type _type;

        public TypeAttributeAccessor(Type type)
        {
            _type = type;
        }

        public IEnumerable<Attribute> GetAttributes()
        {
            return _type.GetCustomAttributes(true).Cast<Attribute>();
        }
    }
}
