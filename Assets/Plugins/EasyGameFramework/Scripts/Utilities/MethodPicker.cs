using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyFramework;
using Sirenix.OdinInspector;

namespace EasyGameFramework
{
    [Serializable]
    public class MethodPicker : MemberPicker
    {
        [PropertySpace(SpaceBefore = 4)]
        [HideIf("HideParameters")]
        [ListDrawerSettings(IsReadOnly = true)]
        public List<VisualObject> Parameters = new List<VisualObject>();

        public MethodInfo GetTargetMethod() => GetTargetMember() as MethodInfo;

        public bool TryInvoke(out object returnValue)
        {
            returnValue = null;
            var m = GetTargetMethod();
            if (m == null) return false;
            returnValue = m.Invoke(GetTargetComponent(), Parameters.Select(p => p.GetRawValue()).ToArray());
            return true;
        }

#if UNITY_EDITOR
        protected override bool MemberFilter(MemberInfo member)
        {
            if (member.MemberType != MemberTypes.Method)
                return false;
            var method = (MethodInfo)member;
            return method.GetParameters().All(p => VisualObject.IsAcceptedType(p.ParameterType));
        }


        private bool HideParameters
        {
            get
            {
                var m = GetTargetMethod();
                if (m == null) return true;
                return m.GetParameters().Length == 0;
            }
        }

        protected override string GetMemberValueDropdownName(MemberInfo member)
        {
            var m = (MethodInfo)member;
            return $"{member.Name} ({m.GetMethodParametersSignature()})";
        }

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            var newMethod = GetTargetMethod();
            if (newMethod != null)
            {
                var ps = newMethod.GetParameters();
                if (ps.Length > Parameters.Count)
                {
                    var count = ps.Length - Parameters.Count;
                    for (int i = 0; i < count; i++)
                    {
                        Parameters.Add(new VisualObject());
                    }
                }
                else if(ps.Length < Parameters.Count)
                {
                    Parameters.RemoveRange(ps.Length, Parameters.Count - ps.Length);
                }
                for (int i = 0; i < ps.Length; i++)
                {
                    var p = ps[i];
                    Parameters[i].Setup(p.ParameterType, p.Name);
                }
            }
        }

        protected override void OnTargetMemberNameChanged()
        {
            base.OnTargetMemberNameChanged();
            OnInspectorInit();
        }
#endif
    }
}
