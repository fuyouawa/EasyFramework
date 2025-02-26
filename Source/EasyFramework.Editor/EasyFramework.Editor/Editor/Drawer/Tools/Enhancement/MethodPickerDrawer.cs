using System.Collections.Generic;
using System.Reflection;
using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EasyFramework.Editor.Drawer
{


    public class MethodPickerDrawer : MemberPickerDrawer<MethodPicker>
    {
        private InspectorProperty _parameters;

        protected override void Initialize()
        {
            _parameters = Property.Children["_parameters"];
            base.Initialize();
        }

        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            return true;
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            base.DrawPropertyLayout(label);

            if (_parameters.WeakSmartValue<List<MethodPicker.Parameter>>().IsNotNullOrEmpty())
            {
                _parameters.Draw();
            }
        }

        protected override bool MemberFilter(MemberInfo member)
        {
            if (member.MemberType != MemberTypes.Method)
                return false;
            var method = (MethodInfo)member;
            return true;
        }

        protected override string GetMemberName(MemberInfo member)
        {
            if (member == null)
                return string.Empty;

            var m = (MethodInfo)member;
            return $"{member.Name} ({m.GetMethodParametersSignature()})";
        }

        protected override void OnTargetMemberChanged()
        {
            base.OnTargetMemberChanged();

            var newMethod = GetTargetMember() as MethodInfo;

            var parameters = _parameters.WeakSmartValue<List<MethodPicker.Parameter>>();
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
