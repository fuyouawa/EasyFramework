using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace EasyFramework.Utilities
{
    [Serializable]
    public class MethodPicker : MemberPicker
    {
        [SerializeField] private ReadOnlyVariantList _parameters = new ReadOnlyVariantList();

        public MethodInfo GetTargetMethod() => GetTargetMember() as MethodInfo;

        public bool TryInvoke(out object returnValue)
        {
            returnValue = null;
            var m = GetTargetMethod();
            if (m == null) return false;

            returnValue = m.Invoke(GetTargetComponent(),
                m.GetParameters().Length != 0
                    ? _parameters.Select(p => p.GetRawValue()).ToArray()
                    : null);
            return true;
        }
    }
}
