using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class MetroBoxGroupAttributeDrawer : EasyGroupAttributeDrawer<MetroBoxGroupAttribute>
    {
        private static readonly GUIContent TempContent = new GUIContent();

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

        public static readonly Color HeaderBoxBackgroundColor = EasyGUIStyles.HeaderBoxBackgroundColor * 0.9f;

        private ICodeValueResolver<string> _labelResolver;
        private ICodeValueResolver<Texture> _iconTextureGetterResolver;

        protected override void Initialize()
        {
            var targetType = Property.Parent.ValueEntry.ValueType;
            _labelResolver = CodeValueResolver.Create<string>(Attribute.Label, targetType, true);
            _iconTextureGetterResolver = CodeValueResolver.Create<Texture>(Attribute.IconTextureGetter, targetType);
        }

        protected override void DrawProperty(GUIContent label)
        {
            if (_labelResolver.HasError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            if (Attribute.IconTextureGetter.IsNotNullOrEmpty() && _iconTextureGetterResolver.HasError(out error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            base.DrawProperty(label);
        }

        protected override void BeginDrawProperty(GUIContent label, ref bool foldout)
        {
            Texture iconTexture = null;
            if (Attribute.IconTextureGetter.IsNotNullOrEmpty())
            {
                iconTexture = _iconTextureGetterResolver.Resolve(Property.Parent.ValueEntry.WeakSmartValue);
                GUILayout.Label(iconTexture, GUILayout.Width(30), GUILayout.Height(30));
            }

            var labelText = _labelResolver.Resolve(Property.Parent.ValueEntry.WeakSmartValue);

            BeginDraw(TempContent.SetText(labelText), iconTexture);
        }

        protected override void EndDrawProperty()
        {
            EndDraw();
        }

        public static void BeginDraw(GUIContent label, Texture iconTexture)
        {
            EasyEditorGUI.BeginIndentedVertical(BoxContainerStyle);
            
            GUILayout.Space(-3);
            var headerBgRect = EditorGUILayout.BeginHorizontal(EasyGUIStyles.BoxHeaderStyle, GUILayout.ExpandWidth(true), GUILayout.Height(30));

            if (Event.current.type == EventType.Repaint)
            {
                headerBgRect.x -= 3;
                headerBgRect.width += 6;
                EasyGUIHelper.PushColor(HeaderBoxBackgroundColor);
                GUI.DrawTexture(headerBgRect, Texture2D.whiteTexture);
                EasyGUIHelper.PopColor();
                EasyEditorGUI.DrawBorders(headerBgRect, 0, 0, 0, 1, EasyGUIStyles.BorderColor);
            }
            
            if (iconTexture != null)
            {
                GUILayout.Label(iconTexture, GUILayout.Width(30), GUILayout.Height(30));
            }

            GUILayout.Label(label, BoxHeaderLabelStyle, GUILayout.Height(30));

            EditorGUILayout.EndHorizontal();
        }

        public static void EndDraw()
        {
            EasyEditorGUI.EndIndentedVertical();
        }
    }
}
