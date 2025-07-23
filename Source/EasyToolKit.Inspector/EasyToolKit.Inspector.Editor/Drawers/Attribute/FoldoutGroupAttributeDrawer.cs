using EasyToolKit.Core.Editor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class FoldoutGroupAttributeDrawer : EasyAttributeDrawer<FoldoutGroupAttribute>
    {
        private static readonly GUIContent TempContent = new GUIContent();

        private InspectorText _labelText;

        protected override void Initialize()
        {
            _labelText = new InspectorText(Property, Attribute.Label);
        }

        protected override void DrawProperty(GUIContent label)
        {
            EasyEditorGUI.BeginBox();
            EasyEditorGUI.BeginBoxHeader();
            Property.State.Expanded = EasyEditorGUI.Foldout(Property.State.Expanded, TempContent.SetText(_labelText.Render()));
            EasyEditorGUI.EndBoxHeader();

            if (Property.State.Expanded)
            {
                CallNextDrawer(label);
                foreach (var groupProperty in Property.GetGroupProperties())
                {
                    groupProperty.Draw();
                    groupProperty.SkipDrawCount++;
                }
            }
            else
            {
                foreach (var groupProperty in Property.GetGroupProperties())
                {
                    groupProperty.SkipDrawCount++;
                }
            }

            EasyEditorGUI.EndBox();
        }
    }
}
