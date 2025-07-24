using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class AwesomeFoldoutGroupAttributeDrawer : EasyAttributeDrawer<AwesomeFoldoutGroupAttribute>
    {
        private static readonly GUIContent TempContent = new GUIContent();

        private static readonly GUIStyle FoldoutStyle = new GUIStyle(EasyGUIStyles.Foldout)
        {
            fontStyle = FontStyle.Bold,
            fontSize = EasyGUIStyles.Foldout.fontSize + 1,
        };

        private ICodeValueResolver<string> _labelResolver;
        private ICodeValueResolver<Texture> _iconTextureGetterResolver;

        protected override void Initialize()
        {
            var targetType = Property.Parent.ValueEntry.ValueType;
            _labelResolver = CodeValueResolverUtility.Create<string>(Attribute.Label, targetType, true);
            _iconTextureGetterResolver = CodeValueResolverUtility.Create<Texture>(Attribute.IconTextureGetter, targetType);
        }

        protected override void DrawProperty(GUIContent label)
        {
            if (_labelResolver.HasError(out var error))
            {
                EditorGUILayout.HelpBox(error, MessageType.Error);
                return;
            }

            if (Attribute.IconTextureGetter.IsNotNullOrEmpty() && _iconTextureGetterResolver.HasError(out error))
            {
                EditorGUILayout.HelpBox(error, MessageType.Error);
                return;
            }

            EasyEditorGUI.BeginBox();

            GUILayout.Space(-3);
            EditorGUILayout.BeginHorizontal("Button", GUILayout.ExpandWidth(true), GUILayout.Height(30));

            EasyGUIHelper.PushColor(Attribute.SideLineColor);
            GUILayout.Box(GUIContent.none, EasyGUIStyles.WhiteBoxStyle, GUILayout.Width(3), GUILayout.Height(30));
            EasyGUIHelper.PopColor();

            if (Attribute.IconTextureGetter.IsNotNullOrEmpty())
            {
                var iconTexture = _iconTextureGetterResolver.Resolve(Property.Parent.ValueEntry.WeakSmartValue);
                GUILayout.Label(iconTexture, GUILayout.Width(30), GUILayout.Height(30));
            }

            EditorGUILayout.BeginVertical();
            GUILayout.Space(7);

            var labelText = _labelResolver.Resolve(Property.Parent.ValueEntry.WeakSmartValue);
            Property.State.Expanded = EasyEditorGUI.Foldout(Property.State.Expanded, TempContent.SetText(labelText), FoldoutStyle);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();

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
