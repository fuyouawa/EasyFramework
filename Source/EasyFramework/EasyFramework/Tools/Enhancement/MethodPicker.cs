using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework
{
    [Serializable]
    public class MethodPicker : MemberPicker
    {
        [Serializable]
        public class Parameter
        {
            public string Name;
            public SerializedVariant Value = new SerializedVariant();
        }
        
        [SerializeField]
        [ListDrawerSettings(IsReadOnly = true)]
        private List<Parameter> _parameters = new List<Parameter>();

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
                    ? _parameters.Select(p => p.Value.GetRawObject()).ToArray()
                    : null);
        }
    }
}
