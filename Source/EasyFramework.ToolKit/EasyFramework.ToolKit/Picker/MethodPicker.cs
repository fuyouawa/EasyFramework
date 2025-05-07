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
        /// <summary>
        /// <para>获取参数的限制类型</para>
        /// <para>匹配时会先判断返回的Type数组长度是否与参数数量相同，然后逐个参数匹配是否符合，如果有Type为null代表不限制该参数的类型。</para>
        /// </summary>
        public string LimitParameterTypesGetter { get; set; }

        public MethodPickerSettingsAttribute()
        {
        }
    }

    [Serializable]
    public class MethodPicker : MemberPicker
    {
        [Serializable, HideLabel, HideReferenceObjectPicker, InlineProperty]
        public class Parameter
        {
            public string Name;
            public SerializedVariant Value = new SerializedVariant();
        }

        [SerializeField]
        [ListDrawerSettings(IsReadOnly = true)]
        private List<Parameter> _parameters = new List<Parameter>();

        public MethodInfo TargetMethod => TargetMember as MethodInfo;

        public object Invoke()
        {
            var m = TargetMethod;
            if (m == null)
            {
                throw new ArgumentException("Invoke failed: Target method is null!");
            }

            object[] invokeParams = null;
            if (m.GetParameters().Length != 0)
            {
                invokeParams = _parameters.Select(p => p.Value.GetRawObject()).ToArray();
            }

            return m.Invoke(TargetComponent, invokeParams);
        }
    }
}
