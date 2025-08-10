using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    public class MetroFoldoutGroupAttributeDrawer : EasyGroupAttributeDrawer<MetroFoldoutGroupAttribute>
    {
        private static readonly GUIContent TempContent = new GUIContent();

        private static GUIStyle s_foldoutStyle;

        public static GUIStyle FoldoutStyle
        {
            get
            {
                if (s_foldoutStyle == null)
                {
                    s_foldoutStyle = new GUIStyle(EasyGUIStyles.Foldout)
                    {
                        fontSize = EasyGUIStyles.Foldout.fontSize + 1,
                        alignment = TextAnchor.MiddleLeft,
                    };
                    s_foldoutStyle.margin.top += 4;
                }
                return s_foldoutStyle;
            }
        }

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

            var labelText = _labelResolver.Resolve(Property.Parent.ValueEntry.WeakSmartValue);

            var foldoutRect = EditorGUILayout.GetControlRect(true, 30, FoldoutStyle);
            Property.State.Expanded = EasyEditorGUI.Foldout(foldoutRect, Property.State.Expanded, TempContent.SetText(labelText), FoldoutStyle);

            EditorGUILayout.EndHorizontal();

            foldout = Property.State.Expanded;
        }

        protected override void EndDrawProperty()
        {
            EasyEditorGUI.EndBox();
        }
    }
}
