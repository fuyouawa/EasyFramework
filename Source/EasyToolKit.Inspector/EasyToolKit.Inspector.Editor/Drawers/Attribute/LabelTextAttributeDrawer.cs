using EasyToolKit.Core;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Super)]
    public class LabelTextAttributeDrawer : EasyAttributeDrawer<LabelTextAttribute>
    {
        private ICodeValueResolver<string> _labelResolver;

        protected override void Initialize()
        {
            var targetType = Property.Parent.ValueEntry.ValueType;
            _labelResolver = CodeValueResolverUtility.Create<string>(Attribute.Label, targetType, true);
        }

        protected override void DrawProperty(GUIContent label)
        {
            if (_labelResolver.HasError(out var error))
            {
                EditorGUILayout.HelpBox(error, MessageType.Error);
                return;
            }

            label.text = _labelResolver.Resolve(Property.Parent.ValueEntry.WeakSmartValue);
            CallNextDrawer(label);
        }
    }
}
