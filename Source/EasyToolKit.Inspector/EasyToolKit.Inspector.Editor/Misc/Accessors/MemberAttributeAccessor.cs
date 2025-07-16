using System;
using System.Collections.Generic;
using System.Reflection;

namespace EasyToolKit.Inspector.Editor
{
    public class MemberAttributeAccessor : IAttributeAccessor
    {
        private readonly MemberInfo _memberInfo;

        public MemberAttributeAccessor(MemberInfo memberInfo)
        {
            _memberInfo = memberInfo;
        }

        public IEnumerable<Attribute> GetAttributes()
        {
            return _memberInfo.GetCustomAttributes();
        }
    }
}
