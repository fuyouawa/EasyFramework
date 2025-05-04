using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Editor
{
    public class MemberPickerDrawer<T> : OdinValueDrawer<T>
        where T : MemberPicker
    {
        private readonly Dictionary<string, MemberInfo> _membersBySignature = new Dictionary<string, MemberInfo>();

        private InspectorProperty _propertyOfTargetObject;
        private InspectorProperty _propertyOfTargetComponent;
        private InspectorProperty _propertyOfTargetMember;

        private GameObject TargetObject
        {
            get => _propertyOfTargetObject.WeakSmartValue<GameObject>();
            set => _propertyOfTargetObject.SetWeakSmartValue(value);
        }

        private Component TargetComponent
        {
            get => _propertyOfTargetComponent.WeakSmartValue<Component>();
            set => _propertyOfTargetComponent.SetWeakSmartValue(value);
        }

        private MemberInfo TargetMember
        {
            get => _propertyOfTargetMember.WeakSmartValue<MemberInfo>();
            set => _propertyOfTargetMember.SetWeakSmartValue(value);
        }

        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            return false;
        }

        protected override void Initialize()
        {
            _propertyOfTargetObject = Property.Children["_targetObject"];
            _propertyOfTargetComponent = Property.Children["_targetComponent"];
            _propertyOfTargetMember = Property.Children["_targetMember"];

            _propertyOfTargetObject.ValueEntry.OnValueChanged += i => OnTargetObjectChanged();

            RefreshTargetMember(TargetComponent);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            EditorGUILayout.BeginHorizontal();

            TargetObject = (GameObject)SirenixEditorFields.UnityObjectField(
                GUIContent.none, TargetObject, typeof(GameObject), true);

            var btnLabel = TargetComponent != null
                ? EditorHelper.TempContent2(GetComponentName(TargetComponent))
                : EditorHelper.NoneSelectorBtnLabel;

            var targetComponents = new Component[] { };
            if (TargetObject != null)
                targetComponents = TargetObject.GetComponents<Component>().Where(c => c != null).ToArray();

            EasyEditorGUI.DrawSelectorDropdown(
                targetComponents,
                GUIContent.none,
                btnLabel,
                OnTargetComponentChanged,
                GetComponentName);

            EditorGUILayout.EndHorizontal();

            btnLabel = TargetMember != null
                ? EditorHelper.TempContent(GetMemberName(TargetMember))
                : EditorHelper.NoneSelectorBtnLabel;

            EasyEditorGUI.DrawSelectorDropdown(
                _membersBySignature.Values,
                GUIContent.none,
                btnLabel,
                OnTargetMemberChanged,
                GetMemberName);
        }

        private void OnTargetObjectChanged()
        {
            OnTargetComponentChanged(null);
        }

        private void OnTargetComponentChanged(Component component)
        {
            TargetComponent = component;

            _membersBySignature.Clear();
            RefreshTargetMember(component);

            OnTargetMemberChanged(null);
        }

        private void RefreshTargetMember(Component component)
        {
            if (component != null)
            {
                foreach (var member in component.GetType()
                             .GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!MemberFilter(member))
                        continue;

                    _membersBySignature.Add(member.GetSignature(), member);
                }
            }
        }

        protected virtual void OnTargetMemberChanged(MemberInfo member)
        {
            TargetMember = member;
        }

        protected virtual string GetComponentName(Component component)
        {
            return component.GetType().GetNiceName();
        }

        protected virtual bool MemberFilter(MemberInfo member)
        {
            return true;
        }

        protected virtual string GetMemberName(MemberInfo member)
        {
            if (member == null)
                return string.Empty;

            var name = $"{member.MemberType}/{member.Name}";
            if (member is MethodInfo m)
            {
                name += $"({m.GetMethodParametersSignature()})";
            }

            return name;
        }
    }
}
