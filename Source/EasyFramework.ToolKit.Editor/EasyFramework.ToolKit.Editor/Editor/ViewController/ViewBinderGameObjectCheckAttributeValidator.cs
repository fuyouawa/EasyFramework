using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.Validation;
using UnityEngine;

[assembly: RegisterValidator(typeof(EasyFramework.ToolKit.Editor.ViewBinderGameObjectCheckAttributeValidator))]

namespace EasyFramework.ToolKit.Editor
{
    public class ViewBinderGameObjectCheckAttributeValidator : AttributeValidator<ViewBinderGameObjectCheckAttribute, GameObject>
    {
        protected override void Initialize()
        {
        }

        protected override void Validate(ValidationResult result)
        {
            var go = ValueEntry.SmartValue;
            if (go == null)
            {
                result.AddWarning("绑定对象不能为空！");
            }
            else if (go.GetComponent<IViewBinder>() == null)
            {
                result.AddError("绑定对象必须有绑定器组件！");
            }
        }
    }
}
