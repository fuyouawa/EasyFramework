using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    public sealed class TitleGroupExAttributeDrawer : OdinGroupDrawer<TitleGroupExAttribute>
    {
        public ValueResolver<string> TitleHelper;
        public ValueResolver<string> SubtitleHelper;
        
        private readonly TitleConfig _config = new TitleConfig();

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
            var attribute = base.Attribute;
            if (property != property.Tree.GetRootProperty(0))
            {
                EditorGUILayout.Space();
            }
            
            _config.Title = TitleHelper.GetValue();
            _config.Subtitle = SubtitleHelper.GetValue();
            _config.TitleAlignment = (TextAlignment)Attribute.TitleAlignment;
            _config.HorizontalLine = Attribute.HorizontalLine;
            _config.BoldTitle = Attribute.BoldTitle;
            _config.TitleFontSize = 13;

            EasyEditorGUI.Title(_config);

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
