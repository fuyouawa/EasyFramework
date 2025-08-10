using EasyToolKit.Core;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Attribute + 100)]
    public class HeaderAttributeDrawer : EasyAttributeDrawer<HeaderAttribute>
    {
        private ICodeValueResolver<string> _headerResolver;

        protected override void Initialize()
        {
            var targetType = Property.Parent.ValueEntry.ValueType;
            _headerResolver = CodeValueResolver.Create<string>(Attribute.header, targetType, true);
        }

        protected override void DrawProperty(GUIContent label)
        {
            if (_headerResolver.HasError(out var error))
            {
                EditorGUILayout.HelpBox(error, MessageType.Error);
                return;
            }

            var headerText = _headerResolver.Resolve(Property.Parent.ValueEntry.WeakSmartValue);
            EditorGUILayout.LabelField(headerText, EditorStyles.boldLabel);
            CallNextDrawer(label);
        }
    }
}
