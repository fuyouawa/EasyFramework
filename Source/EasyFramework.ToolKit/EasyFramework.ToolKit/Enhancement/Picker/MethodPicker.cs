using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public class MethodPickerSettingsAttribute : PropertyAttribute
    {
        public int LimitParameterCount { get; set; }
        public string LimitParameterTypesGetter { get; set; }

        public MethodPickerSettingsAttribute()
        {
            LimitParameterCount = int.MaxValue;
        }
    }

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

        public object Invoke()
        {
            var m = GetTargetMethod();
            if (m == null)
            {
                throw new ArgumentException("Invoke failed: Target method is null!");
            }

            object[] invokeParams = null;
            if (m.GetParameters().Length != 0)
            {
                invokeParams = _parameters.Select(p => p.Value.GetRawObject()).ToArray();
            }

            return m.Invoke(GetTargetComponent(), invokeParams);
        }
    }
}
