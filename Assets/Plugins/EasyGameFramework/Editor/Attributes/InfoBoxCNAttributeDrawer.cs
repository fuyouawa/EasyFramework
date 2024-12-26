[assembly: Sirenix.OdinInspector.Editor.Validation.RegisterValidator(typeof(EasyGameFramework.Editor.InformationValidator))]

namespace EasyGameFramework.Editor
{
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.OdinInspector.Editor.Validation;
    using Sirenix.OdinInspector.Editor.ValueResolvers;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    [DrawerPriority(0.0, 10001.0, 0.0)]
    public class InfoBoxCNAttributeDrawer : OdinAttributeDrawer<InfoBoxCNAttribute>
    {
        private bool _drawMessageBox;
        private ValueResolver<bool> _visibleIfResolver;
        private ValueResolver<string> _messageResolver;
        private ValueResolver<Color> _iconColorResolver;
        private MessageType _messageType;

        protected override void Initialize()
        {
            _visibleIfResolver = ValueResolver.Get(base.Property, base.Attribute.VisibleIf, fallbackValue: true);
            _messageResolver = ValueResolver.GetForString(base.Property, base.Attribute.Message);
            _iconColorResolver = ValueResolver.Get<Color>(base.Property, base.Attribute.IconColor,
                EditorStyles.label.normal.textColor);

            _drawMessageBox = _visibleIfResolver.GetValue();
            switch (Attribute.InfoMessageType)
            {
                case InfoMessageType.Info:
                    _messageType = MessageType.Info;
                    break;
                case InfoMessageType.Warning:
                    _messageType = MessageType.Warning;
                    break;
                case InfoMessageType.Error:
                    _messageType = MessageType.Error;
                    break;
                case InfoMessageType.None:
                default:
                    _messageType = MessageType.None;
                    break;
            }
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            bool flag = true;
            if (_visibleIfResolver.HasError)
            {
                SirenixEditorGUI.ErrorMessageBox(_visibleIfResolver.ErrorMessage);
                flag = false;
            }

            if (_messageResolver.HasError)
            {
                SirenixEditorGUI.ErrorMessageBox(_messageResolver.ErrorMessage);
                flag = false;
            }

            if (_iconColorResolver.HasError)
            {
                SirenixEditorGUI.ErrorMessageBox(_iconColorResolver.ErrorMessage);
                flag = false;
            }

            if (!flag)
            {
                CallNextDrawer(label);
                return;
            }

            if (base.Attribute.GUIAlwaysEnabled)
            {
                GUIHelper.PushGUIEnabled(enabled: true);
            }

            if (UnityEngine.Event.current.type == EventType.Layout)
            {
                _drawMessageBox = _visibleIfResolver.GetValue();
            }

            if (_drawMessageBox)
            {
                string value = _messageResolver.GetValue();
                SirenixEditorGUI.MessageBox(value, _messageType, EasyGUIStyles.InfoBoxCN, true);
            }

            if (base.Attribute.GUIAlwaysEnabled)
            {
                GUIHelper.PopGUIEnabled();
            }

            CallNextDrawer(label);
        }
    }

    [NoValidationInInspector]
    public class InformationValidator : AttributeValidator<InfoBoxCNAttribute>
    {
        private ValueResolver<bool> _showMessageGetter;

        private ValueResolver<string> _messageGetter;

        protected override void Initialize()
        {
            _showMessageGetter = ValueResolver.Get(base.Property, base.Attribute.VisibleIf, fallbackValue: true);
            _messageGetter = ValueResolver.GetForString(base.Property, base.Attribute.Message);
        }

        protected override void Validate(ValidationResult result)
        {
            if (_showMessageGetter != null)
            {
                if (_showMessageGetter.HasError || _messageGetter.HasError)
                {
                    result.Message = ValueResolver.GetCombinedErrors(_showMessageGetter, _messageGetter);
                    result.ResultType = ValidationResultType.Error;
                }
                else if (_showMessageGetter.GetValue())
                {
                    switch (Attribute.InfoMessageType)
                    {
                        case InfoMessageType.Warning:
                            result.ResultType = ValidationResultType.Warning;
                            break;
                        case InfoMessageType.Error:
                            result.ResultType = ValidationResultType.Error;
                            break;
                        case InfoMessageType.None:
                        case InfoMessageType.Info:
                        default:
                            result.ResultType = ValidationResultType.Valid;
                            break;
                    }
                    result.Message = _messageGetter.GetValue();
                }
            }
        }
    }
}
