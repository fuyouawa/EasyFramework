using System;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework
{
    [Serializable, InlineProperty, HideLabel, HideReferenceObjectPicker]
    public abstract class MemberPicker
    {
        [SerializeField, UsedImplicitly] private GameObject _targetObject;
        [SerializeField, UsedImplicitly] private string _targetComponentName;
        [SerializeField, UsedImplicitly] private string _targetMemberName;

        public Component GetTargetComponent()
        {
            if (string.IsNullOrWhiteSpace(_targetComponentName) || _targetObject == null)
                return null;
            int i = 0;
            foreach (var component in _targetObject.GetComponents<Component>())
            {
                var name = component == null ? $"Missing Component<{i}>" : component.GetType().Name;
                if (name == _targetComponentName)
                {
                    return component;
                }
            }

            return null;
        }

        public MemberInfo GetTargetMember()
        {
            var c = GetTargetComponent();
            if (c == null)
                return null;

            foreach (var method in c.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (method.GetSignature() == _targetMemberName)
                {
                    return method;
                }
            }

            return null;
        }
    }
}
