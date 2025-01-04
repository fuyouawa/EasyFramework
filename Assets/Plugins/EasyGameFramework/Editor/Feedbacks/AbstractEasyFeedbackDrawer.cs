using System.Linq;
using EasyFramework;
using UnityEditor;
using UnityEngine;

namespace EasyGameFramework.Editor
{
    public class AbstractEasyFeedbackDrawer : FoldoutValueDrawer<AbstractEasyFeedback>
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

        protected override string GetRightLabel(GUIContent label)
        {
            var attr = ValueEntry.SmartValue.GetType().GetCustomAttribute<AddEasyFeedbackMenuAttribute>();
            if (attr != null)
            {
                return $"[{attr.Path.Split("/").Last()}]";
            }
            return base.GetRightLabel(label);
        }

        protected override string GetLabel(GUIContent label)
        {
            var value = ValueEntry.SmartValue;

            return "      " + (value.Label.IsNullOrEmpty() ? "TODO" : value.Label);
        }
    }
}
