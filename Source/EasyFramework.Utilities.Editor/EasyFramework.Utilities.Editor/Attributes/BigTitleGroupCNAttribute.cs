using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyGameFramework.Editor
{
    public sealed class BigTitleGroupCNAttributeDrawer : OdinGroupDrawer<BigTitleGroupCNAttribute>
    {
        public ValueResolver<string> TitleHelper;

        public ValueResolver<string> SubtitleHelper;

        protected override void Initialize()
        {
            TitleHelper = ValueResolver.GetForString(base.Property, base.Attribute.GroupName);
            SubtitleHelper = ValueResolver.GetForString(base.Property, base.Attribute.Subtitle);
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            InspectorProperty property = base.Property;
            BigTitleGroupCNAttribute attribute = base.Attribute;
            if (property != property.Tree.GetRootProperty(0))
            {
                EditorGUILayout.Space();
            }

            EasyEditorGUI.BigTitle(
                TitleHelper.GetValue(),
                SubtitleHelper.GetValue(),
                (TextAlignment)attribute.Alignment,
                attribute.HorizontalLine,
                attribute.BoldTitle);

            GUIHelper.PushIndentLevel(EditorGUI.indentLevel + (attribute.Indent ? 1 : 0));
            for (int i = 0; i < property.Children.Count; i++)
            {
                InspectorProperty child = property.Children[i];
                child.Draw(child.Label);
            }

            GUIHelper.PopIndentLevel();
        }
    }
}
