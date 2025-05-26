using EasyFramework.Editor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    [DrawerPriority(0.0, 10001.0, 0.0)]
    public class InfoBoxExAttributeDrawer : OdinAttributeDrawer<InfoBoxExAttribute>
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

            if (Event.current.type == EventType.Layout)
            {
                _drawMessageBox = _visibleIfResolver.GetValue();
            }

            if (_drawMessageBox)
            {
                string value = _messageResolver.GetValue();
                EasyEditorGUI.MessageBox(value, _messageType);
            }

            if (base.Attribute.GUIAlwaysEnabled)
            {
                GUIHelper.PopGUIEnabled();
            }

            CallNextDrawer(label);
        }
    }
}
