using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class FoldoutGroupAttributeDrawer : EasyGroupAttributeDrawer<FoldoutGroupAttribute>
    {
        private static readonly GUIContent TempContent = new GUIContent();
        
        private ICodeValueResolver<string> _labelResolver;

        protected override void Initialize()
        {
            var targetType = Property.Parent.ValueEntry.ValueType;
            _labelResolver = CodeValueResolver.Create<string>(Attribute.Label, targetType, true);
        }

        protected override void DrawProperty(GUIContent label)
        {
            if (_labelResolver.HasError(out var error))
            {
                EditorGUILayout.HelpBox(error, MessageType.Error);
                return;
            }

            base.DrawProperty(label);
        }

        protected override void BeginDrawProperty(GUIContent label, ref bool foldout)
        {
            EasyEditorGUI.BeginBox();
            EasyEditorGUI.BeginBoxHeader();
            var labelText = _labelResolver.Resolve(Property.Parent.ValueEntry.WeakSmartValue);
            Property.State.Expanded = EasyEditorGUI.Foldout(Property.State.Expanded, TempContent.SetText(labelText));
            EasyEditorGUI.EndBoxHeader();

            foldout = Property.State.Expanded;
        }

        protected override void EndDrawProperty()
        {
            EasyEditorGUI.EndBox();
        }
    }
}
