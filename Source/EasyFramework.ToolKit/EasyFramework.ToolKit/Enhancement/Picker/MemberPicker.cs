using System;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    [Serializable, InlineProperty, HideLabel, HideReferenceObjectPicker]
    public abstract class MemberPicker
    {
#pragma warning disable CS0649
        [SerializeField] private GameObject _targetObject;
        [SerializeField] private string _targetComponentName;
        [SerializeField] private string _targetMemberName;
#pragma warning restore CS0649

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
