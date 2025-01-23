using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace EasyFramework
{
    [Serializable]
    public class MethodPicker : MemberPicker
    {
        [SerializeField] private ReadOnlyVariantList _parameters = new ReadOnlyVariantList();

        public MethodInfo GetTargetMethod() => GetTargetMember() as MethodInfo;

        public bool TryInvoke(out object returnValue)
        {
            try
            {
                returnValue = Invoke();
                return true;
            }
            catch (Exception)
            {
                returnValue = null;
                return false;
            }
        }

        public object Invoke()
        {
            var m = GetTargetMethod();
            if (m == null)
            {
                throw new ArgumentException("Invoke failed: Target method is null!");
            }

            return m.Invoke(GetTargetComponent(),
                m.GetParameters().Length != 0
                    ? _parameters.Select(p => p.GetRawValue()).ToArray()
                    : null);
        }
    }
}
