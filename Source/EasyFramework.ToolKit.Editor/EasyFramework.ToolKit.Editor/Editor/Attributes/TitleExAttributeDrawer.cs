using EasyFramework.Editor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.ToolKit.Editor
{
    [DrawerPriority(1.0, 0.0, 0.0)]
    public class TitleExAttributeDrawer : OdinAttributeDrawer<TitleExAttribute>
    {
        private ValueResolver<string> titleResolver;

        private ValueResolver<string> subtitleResolver;

        private readonly TitleConfig _config = new TitleConfig();

        protected override void Initialize()
        {
            titleResolver = ValueResolver.GetForString(base.Property, base.Attribute.Title);
            subtitleResolver = ValueResolver.GetForString(base.Property, base.Attribute.Subtitle);
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (base.Property != base.Property.Tree.GetRootProperty(0))
            {
                EditorGUILayout.Space();
            }

            bool valid = true;
            if (titleResolver.HasError)
            {
                SirenixEditorGUI.ErrorMessageBox(titleResolver.ErrorMessage);
                valid = false;
            }

            if (subtitleResolver.HasError)
            {
                SirenixEditorGUI.ErrorMessageBox(subtitleResolver.ErrorMessage);
                valid = false;
            }

            if (valid)
            {
                _config.Title = titleResolver.GetValue();
                _config.Subtitle = subtitleResolver.GetValue();
                _config.TitleAlignment = (TextAlignment)Attribute.TitleAlignment;
                _config.HorizontalLine = Attribute.HorizontalLine;
                _config.BoldTitle = Attribute.BoldTitle;
                _config.TitleFontSize = Attribute.TitleFontSize;

                EasyEditorGUI.Title(_config);
            }

            CallNextDrawer(label);
        }
    }
}
