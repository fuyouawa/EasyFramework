using System.Reflection;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    //TODO support parameters
    public class ButtonAttributeDrawer : EasyAttributeDrawer<ButtonAttribute>
    {
        private MethodInfo _methodInfo;
        private ICodeValueResolver<string> _buttonLabelResolver;

        protected override bool CanDrawAttributeProperty(InspectorProperty property)
        {
            return property.Info.TryGetMemberInfo() is MethodInfo;
        }

        protected override void Initialize()
        {
            _methodInfo = Property.Info.TryGetMemberInfo() as MethodInfo;
            var targetType = Property.Parent.ValueEntry.ValueType;
            _buttonLabelResolver = CodeValueResolver.Create<string>(Attribute.Label, targetType, true);
        }

        protected override void DrawProperty(GUIContent label)
        {
            if (_buttonLabelResolver.HasError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            var buttonLabel = _buttonLabelResolver.Resolve(Property.Parent.ValueEntry.WeakSmartValue);
            if (GUILayout.Button(buttonLabel))
            {
                foreach (var target in Property.Parent.ValueEntry.WeakValues)
                {
                    if (target == null)
                        continue;
                    _methodInfo.Invoke(target, null);
                }
            }
        }
    }
}