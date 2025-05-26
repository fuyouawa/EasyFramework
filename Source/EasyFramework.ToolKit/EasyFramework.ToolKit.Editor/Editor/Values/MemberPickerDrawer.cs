using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyFramework.Core;
using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class MemberPickerDrawer<T> : OdinValueDrawer<T>
        where T : MemberPicker
    {
        private readonly Dictionary<string, MemberInfo> _membersBySignature = new Dictionary<string, MemberInfo>();

        private InspectorProperty _targetObjectProperty;
        private InspectorProperty _targetComponentProperty;
        private InspectorProperty _targetMemberProperty;

        private GameObject TargetObject
        {
            get => _targetObjectProperty.ValueEntry.WeakSmartValueT<GameObject>();
            set => _targetObjectProperty.ValueEntry.WeakSmartValue = value;
        }

        private Component TargetComponent
        {
            get => _targetComponentProperty.ValueEntry.WeakSmartValueT<Component>();
            set => _targetComponentProperty.ValueEntry.WeakSmartValue = value;
        }

        private MemberInfo TargetMember
        {
            get => _targetMemberProperty.ValueEntry.WeakSmartValueT<MemberInfo>();
            set => _targetMemberProperty.ValueEntry.WeakSmartValue = value;
        }

        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            return false;
        }

        protected override void Initialize()
        {
            _targetObjectProperty = Property.Children["_targetObject"];
            _targetComponentProperty = Property.Children["_targetComponent"];
            _targetMemberProperty = Property.Children["_targetMember"];

            _targetObjectProperty.ValueEntry.OnValueChanged += i => OnTargetObjectChanged();

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

            EasyEditorGUI.DrawSelectorDropdown(
                () => TargetObject != null
                    ? TargetObject.GetComponents<Component>().Where(c => c != null).ToArray()
                    : new Component[]{},
                GUIContent.none,
                btnLabel,
                OnTargetComponentChanged,
                GetComponentName);

            EditorGUILayout.EndHorizontal();

            btnLabel = TargetMember != null
                ? EditorHelper.TempContent(GetMemberName(TargetMember))
                : EditorHelper.NoneSelectorBtnLabel;

            EasyEditorGUI.DrawSelectorDropdown(
                () => _membersBySignature.Values,
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
