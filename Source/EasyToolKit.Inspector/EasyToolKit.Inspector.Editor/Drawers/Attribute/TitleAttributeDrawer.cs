using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Attribute + 100)]
    public class TitleAttributeDrawer : EasyAttributeDrawer<TitleAttribute>
    {
        private ICodeValueResolver<string> _titleResolver;
        private ICodeValueResolver<string> _subtitleResolver;

        protected override void Initialize()
        {
            var targetType = Property.Parent.ValueEntry.ValueType;
            _titleResolver = CodeValueResolver.Create<string>(Attribute.Title, targetType, true);
            _subtitleResolver = CodeValueResolver.Create<string>(Attribute.Subtitle, targetType, true);
        }

        protected override void DrawProperty(GUIContent label)
        {
            if (_titleResolver.HasError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }
            if (_subtitleResolver.HasError(out error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            var titleText = _titleResolver.Resolve(Property.Parent.ValueEntry.WeakSmartValue);
            var subtitleText = _subtitleResolver.Resolve(Property.Parent.ValueEntry.WeakSmartValue);
            EasyEditorGUI.Title(titleText, subtitleText, Attribute.TextAlignment, Attribute.HorizontalLine, Attribute.BoldTitle);
            
            CallNextDrawer(label);
        }
    }
}