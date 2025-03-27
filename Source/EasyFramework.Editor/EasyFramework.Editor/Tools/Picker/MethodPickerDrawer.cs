using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace EasyFramework.Editor
{
    public class MethodPickerDrawer : MemberPickerDrawer<MethodPicker>
    {
        private InspectorProperty _propertyOfParameters;
        private MethodPickerSettingsAttribute _settings;
        private string _error;

        private ValueResolver<object> _limitParameterTypesResolver;

        protected override void Initialize()
        {
            _propertyOfParameters = Property.Children["_parameters"];
            _settings = Property.GetAttribute<MethodPickerSettingsAttribute>()
                        ?? new MethodPickerSettingsAttribute();
            _limitParameterTypesResolver = ValueResolver.Get<object>(Property, _settings.LimitParameterTypesGetter);
            _error = _limitParameterTypesResolver.ErrorMessage;

            base.Initialize();
        }

        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            return true;
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (_error.IsNotNullOrEmpty())
            {
                SirenixEditorGUI.ErrorMessageBox(_error);
            }

            base.DrawPropertyLayout(label);

            if (_propertyOfParameters.WeakSmartValue<List<MethodPicker.Parameter>>().IsNotNullOrEmpty())
            {
                _propertyOfParameters.Draw();
            }
        }

        protected override bool MemberFilter(MemberInfo member)
        {
            if (member.MemberType != MemberTypes.Method)
                return false;
            var method = (MethodInfo)member;
            var parameters = method.GetParameters();
            if (parameters.Length == 0)
            {
                return true;
            }

            if (_settings.LimitParameterCount < parameters.Length)
                return false;

            if (_error.IsNullOrEmpty())
            {
                var limitParameterTypesSource = _limitParameterTypesResolver.GetValue();
                if (limitParameterTypesSource != null)
                {
                    var limitParameterTypes = ((IEnumerable)limitParameterTypesSource)
                        .Cast<object>()
                        .Select(o => (Type)o)
                        .ToArray();

                    if (limitParameterTypes.Length != parameters.Length)
                        return false;

                    for (int i = 0; i < limitParameterTypes.Length; i++)
                    {
                        if (limitParameterTypes[i] != parameters[i].ParameterType)
                            return false;
                    }
                }
            }

            return true;
        }

        protected override string GetMemberName(MemberInfo member)
        {
            if (member == null)
                return string.Empty;

            var m = (MethodInfo)member;
            return $"{member.Name} ({m.GetMethodParametersSignature()})";
        }

        protected override void OnTargetMemberChanged(MemberInfo member)
        {
            base.OnTargetMemberChanged(member);

            var newMethod = member as MethodInfo;

            var parameters = _propertyOfParameters.WeakSmartValue<List<MethodPicker.Parameter>>();
            if (newMethod != null)
            {
                var ps = newMethod.GetParameters();
                if (ps.Length > parameters.Count)
                {
                    var count = ps.Length - parameters.Count;
                    for (int i = 0; i < count; i++)
                    {
                        parameters.Add(new MethodPicker.Parameter());
                    }
                }
                else if (ps.Length < parameters.Count)
                {
                    parameters.RemoveRange(ps.Length, parameters.Count - ps.Length);
                }

                for (int i = 0; i < ps.Length; i++)
                {
                    var p = ps[i];
                    var p2 = parameters[i];
                    p2.Name = p.Name;
                    p2.Value.SetNull();
                    p2.Value.Type = p.ParameterType;
                }
            }
            else
            {
                parameters.Clear();
            }
        }
    }
}
