using System.Collections.Generic;
using System.Reflection;
using EasyFramework.Generic;
using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Utilities.Editor
{
    public class MemberPickerDrawer<T> : OdinValueDrawer<T>
        where T : MemberPicker
    {
        private readonly Dictionary<string, Component> _targetComponents = new Dictionary<string, Component>();
        private readonly Dictionary<string, MemberInfo> _targetMembers = new Dictionary<string, MemberInfo>();

        private InspectorProperty _targetObject;
        private InspectorProperty _targetComponentName;
        private InspectorProperty _targetMemberName;

        private GameObject TargetObject
        {
            get => _targetObject.ValueEntry.WeakSmartValue as GameObject;
            set => _targetObject.ValueEntry.WeakSmartValue = value;
        }

        private string TargetComponentName
        {
            get => _targetComponentName.ValueEntry.WeakSmartValue as string;
            set => _targetComponentName.ValueEntry.WeakSmartValue = value;
        }

        private string TargetMemberName
        {
            get => _targetMemberName.ValueEntry.WeakSmartValue as string;
            set => _targetMemberName.ValueEntry.WeakSmartValue = value;
        }

        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            return false;
        }

        protected override void Initialize()
        {
            _targetObject = Property.Children["_targetObject"];
            _targetComponentName = Property.Children["_targetComponentName"];
            _targetMemberName = Property.Children["_targetMemberName"];

            base.Initialize();

            RefreshTargetComponents();
            RefreshTargetMember();
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            TargetObject =
                (GameObject)SirenixEditorFields.UnityObjectField(GUIContent.none, TargetObject, typeof(GameObject),
                    true);
            if (EditorGUI.EndChangeCheck())
            {
                OnTargetObjectChanged();
            }

            EditorGUI.BeginChangeCheck();
            EasyEditorGUI.DrawSelectorDropdown(new SelectorDropdownConfig<string>(
                GUIContent.none,
                EditorHelper.TempContent(TargetComponentName),
                _targetComponents.Keys,
                str => TargetComponentName = str));
            if (EditorGUI.EndChangeCheck())
            {
                OnTargetComponentChanged();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();

            var member = GetTargetMember();
            EasyEditorGUI.DrawSelectorDropdown(new SelectorDropdownConfig<MemberInfo>(
                GUIContent.none,
                EditorHelper.TempContent(GetMemberName(member)),
                _targetMembers.Values,
                str => TargetMemberName = str.GetSignature())
            {
                MenuItemNameGetter = GetMemberName
            });

            if (EditorGUI.EndChangeCheck())
            {
                OnTargetMemberChanged();
            }
        }

        private void OnTargetObjectChanged()
        {
            _targetComponents.Clear();

            RefreshTargetComponents();

            TargetComponentName = string.Empty;

            OnTargetComponentChanged();
        }

        private void RefreshTargetComponents()
        {
            if (TargetObject != null)
            {
                int i = 0;
                foreach (var component in TargetObject.GetComponents<Component>())
                {
                    _targetComponents.Add(component == null
                        ? $"Missing Component<{i}>"
                        : component.GetType().Name, component);
                    i++;
                }
            }
        }

        private void OnTargetComponentChanged()
        {
            _targetMembers.Clear();

            RefreshTargetMember();

            TargetMemberName = string.Empty;

            OnTargetMemberChanged();
        }

        private void RefreshTargetMember()
        {
            var comp = GetTargetComponent();
            if (comp != null)
            {
                foreach (var member in comp.GetType()
                             .GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!MemberFilter(member))
                        continue;

                    _targetMembers.Add(member.GetSignature(), member);
                }
            }
        }

        protected virtual void OnTargetMemberChanged()
        {
        }

        protected Component GetTargetComponent()
        {
            if (TargetComponentName.IsNullOrEmpty())
                return null;
            return _targetComponents[TargetComponentName];
        }

        protected MemberInfo GetTargetMember()
        {
            if (TargetMemberName.IsNullOrEmpty())
                return null;
            return _targetMembers[TargetMemberName];
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
