using EasyFramework.Core;
using EasyFramework.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public class IFeedbackDrawer<T> : FoldoutValueDrawer<T>
        where T : IFeedback
    {
        private static readonly float IconWidth = EditorGUIUtility.singleLineHeight;

        protected override void OnCoveredTitleBarGUI(Rect headerRect)
        {
            var value = ValueEntry.SmartValue;

            var buttonRect = new Rect(headerRect)
            {
                x = headerRect.x + 17,
                width = IconWidth,
                height = IconWidth
            };

            value.Enable = EditorGUI.Toggle(buttonRect, value.Enable);
        }

        private readonly LabelConfig _labelConfig = new LabelConfig(
            new GUIContent(),
            Color.yellow);

        protected override LabelConfig GetRightLabelConfig(GUIContent label)
        {
            var attr = Property.GetAttribute<AddFeedbackMenuAttribute>();
            _labelConfig.Content.text = $"[{attr.Path.Replace("/", " - ")}]";
            return _labelConfig;
        }

        protected override GUIContent GetLabel(GUIContent label)
        {
            var value = ValueEntry.SmartValue;

            return new GUIContent("      " + (value.Label.IsNullOrEmpty() ? "TODO" : value.Label));
        }

        protected override void Initialize()
        {
            base.Initialize();

            var style = new GUIStyle(SirenixGUIStyles.BoldLabel);
            style.fontSize += 1;
            style.padding.bottom += 2;
            style.padding.left -= 9;
            _labelConfig.Style = style;
        }

        protected override void OnContentGUI(Rect headerRect)
        {
            var value = ValueEntry.SmartValue;

            if (value.Tip.IsNotNullOrWhiteSpace())
            {
                EasyEditorGUI.MessageBox(value.Tip, MessageType.Info);
            }

            base.OnContentGUI(headerRect);
        }
    }
}
