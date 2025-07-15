using System;
using System.Collections.Generic;

namespace EasyToolKit.Inspector.Editor
{
    public interface IAttributeAccessor
    {
        IEnumerable<Attribute> GetAttributes();
    }
}
