using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

namespace EasyToolKit.Inspector.Editor
{
    public class AwesomeBoxGroupAttributeDrawer : EasyGroupAttributeDrawer<AwesomeBoxGroupAttribute>
    {
        private static readonly GUIContent TempContent = new GUIContent();

        private static GUIStyle s_boxStyle;

        public static GUIStyle BoxHeaderStyle
        {
            get
            {
                if (s_boxStyle == null)
                {
                    s_boxStyle = new GUIStyle(EasyGUIStyles.BoxHeaderStyle)
                    {
                    };
                }
                return s_boxStyle;
            }
        }

        private static GUIStyle s_boxHeaderLabelStyle;
        public static GUIStyle BoxHeaderLabelStyle
        {
            get
            {
                if (s_boxHeaderLabelStyle == null)
                {
                    s_boxHeaderLabelStyle = new GUIStyle(GUI.skin.label)
                    {
                        fontSize = EasyGUIStyles.Foldout.fontSize + 1,
                        alignment = TextAnchor.MiddleLeft,
                    };
                    s_boxHeaderLabelStyle.margin.top += 4;
                }
                return s_boxHeaderLabelStyle;
            }
        }

        public static readonly GUIStyle BoxContainerStyle = new GUIStyle("TextArea")
        {
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

            base.DrawProperty(label);
        }

        protected override void BeginDrawProperty(GUIContent label, ref bool foldout)
        {
            EasyEditorGUI.BeginIndentedVertical(BoxContainerStyle);

            GUILayout.Space(-3);
            var headerRect = EditorGUILayout.BeginHorizontal(BoxHeaderStyle, GUILayout.ExpandWidth(true), GUILayout.Height(30));
            
            if (Attribute.IconTextureGetter.IsNotNullOrEmpty())
            {
                var iconTexture = _iconTextureGetterResolver.Resolve(Property.Parent.ValueEntry.WeakSmartValue);
                GUILayout.Label(iconTexture, GUILayout.Width(30), GUILayout.Height(30));
            }

            var labelText = _labelResolver.Resolve(Property.Parent.ValueEntry.WeakSmartValue);
            GUILayout.Label(TempContent.SetText(labelText), BoxHeaderLabelStyle, GUILayout.Height(30));

            EasyEditorGUI.DrawBorders(headerRect, 0, 0, 0, 1, EasyGUIStyles.BorderColor);
            EditorGUILayout.EndHorizontal();
        }

        protected override void EndDrawProperty()
        {
            EasyEditorGUI.EndIndentedVertical();
        }
    }
}
