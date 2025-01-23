using EasyFramework.Inspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Editor
{
    [DrawerPriority(1.0, 0.0, 0.0)]
    public class BigTitleCNAttributeDrawer : OdinAttributeDrawer<BigTitleCNAttribute>
    {
        private ValueResolver<string> titleResolver;

        private ValueResolver<string> subtitleResolver;

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
                EasyEditorGUI.BigTitle(
                    titleResolver.GetValue(),
                    subtitleResolver.GetValue(),
                    (TextAlignment)base.Attribute.TitleAlignment,
                    base.Attribute.HorizontalLine,
                    base.Attribute.Bold);
            }

            CallNextDrawer(label);
        }
    }
}
