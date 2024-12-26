using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Linq;
using System.Reflection;
using EasyFramework;
using UnityEngine;

namespace EasyGameFramework
{
    [Serializable]
    public abstract class MemberPicker
    {
        [HideLabel]
        [HorizontalGroup("Picker")]
        [OnValueChanged("OnTargetObjectChanged")]
        public GameObject TargetObject;

        [HideLabel]
        [HorizontalGroup("Picker")]
        [ValueDropdown("GetTargetComponentNamesDropdown")]
        [OnValueChanged("OnTargetComponentNameChanged")]
        public string TargetComponentName;

        [HideLabel]
        [ValueDropdown("GetComponentMemberNamesDropdown")]
        [OnValueChanged("OnTargetMemberNameChanged")]
        public string TargetMemberName = string.Empty;

        private Component _targetComponent;

        public Component GetTargetComponent()
        {
            if (_targetComponent != null)
                return _targetComponent;

            if (string.IsNullOrWhiteSpace(TargetComponentName) || TargetObject == null)
                return null;
            _targetComponent = TargetObject.GetComponents<Component>()
                .FirstOrDefault(c => c.GetType().Name == TargetComponentName);
            return _targetComponent;
        }

        private MemberInfo _targetMember;

        public MemberInfo GetTargetMember()
        {
            if (_targetMember != null)
                return _targetMember;

            var c = GetTargetComponent();
            if (c == null)
                return null;
            _targetMember = c.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(m => m.GetSignature() == TargetMemberName);

            return _targetMember;
        }

        // public override string ToString()
        // {
        //     if (TargetComponent == null || string.IsNullOrEmpty(TargetMemberName))
        //         return "None Member";
        //     var member = TargetMember;
        //     if (TargetMember == null)
        //     {
        //         if (!TryGetMember(out member))
        //         {
        //             return "None Member";
        //         }
        //     }
        //     return $"{TargetComponent.GetType().Name}.{member.Name}[{member.MemberType}]";
        // }
#if UNITY_EDITOR
        protected virtual void OnTargetComponentNameChanged()
        {
            _targetComponent = null;
            TargetMemberName = string.Empty;
            OnTargetMemberNameChanged();
        }

        protected virtual void OnTargetObjectChanged()
        {
            TargetComponentName = string.Empty;
            OnTargetComponentNameChanged();
        }

        protected virtual void OnTargetMemberNameChanged()
        {
            _targetMember = null;
        }

        private IEnumerable GetTargetComponentNamesDropdown()
        {
            var total = new ValueDropdownList<string>() { { "None", string.Empty } };
            if (TargetObject == null)
                return total;
            int missingCount = 1;
            foreach (var comp in TargetObject.GetComponents<Component>())
            {
                if (comp == null)
                {
                    total.Add(new ValueDropdownItem<string>($"Missing<{missingCount}>", string.Empty));
                    missingCount++;
                }
                else
                {
                    var n = comp.GetType().Name;
                    total.Add(new ValueDropdownItem<string>(n, n));
                }
            }
            return total;
        }

        private IEnumerable GetComponentMemberNamesDropdown()
        {
            var total = new ValueDropdownList<string>() { { "None", string.Empty } };
            if (string.IsNullOrWhiteSpace(TargetComponentName))
                return total;

            var c = GetTargetComponent();
            if (c == null)
                return total;

            total.AddRange(c.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(MemberFilter)
                .Select(m => new ValueDropdownItem<string>(
                    GetMemberValueDropdownName(m),
                    m.GetSignature())));
            return total;
        }

        protected virtual bool MemberFilter(MemberInfo member)
        {
            return true;
        }

        protected virtual string GetMemberValueDropdownName(MemberInfo member)
        {
            var name = $"{member.MemberType}/{member.Name}";
            if (member is MethodInfo m)
            {
                name += $"({m.GetMethodParametersSignature()})";
            }
            return name;
        }
#endif
    }
}
