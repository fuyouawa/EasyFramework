using Sirenix.OdinInspector.Editor.Validation;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.OdinInspector;

[assembly: RegisterValidator(typeof(EasyFramework.ToolKit.Editor.InfoBoxExAttributeValidator))]

namespace EasyFramework.ToolKit.Editor
{
    [NoValidationInInspector]
    public class InfoBoxExAttributeValidator : AttributeValidator<InfoBoxExAttribute>
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
