using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using JetBrains.Annotations;
using System;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    public class TerrainDefinitionDrawer : EasyValueDrawer<TerrainDefinition>
    {
        private static readonly GUIContent TempContent = new GUIContent();
        private static readonly Color SelectedButtonColor = new Color(0, 0.7f, 1f, 1);

        [CanBeNull] public static Guid? SelectedGuid { get; private set; }
        public static DrawMode SelectedDrawMode { get; private set; }

        protected override void DrawProperty(GUIContent label)
        {
            EasyEditorGUI.BeginBox();

            GUILayout.Space(-3);
            EditorGUILayout.BeginHorizontal("Button", GUILayout.ExpandWidth(true), GUILayout.Height(30));

            EasyGUIHelper.PushColor(ValueEntry.SmartValue.DebugCubeColor.SetA(1f));
            GUILayout.Box(GUIContent.none, EasyGUIStyles.WhiteBoxStyle, GUILayout.Width(3), GUILayout.Height(30));
            EasyGUIHelper.PopColor();

            var foldoutRect = EditorGUILayout.GetControlRect(true, 30, MetroFoldoutGroupAttributeDrawer.FoldoutStyle);

            var title = ValueEntry.SmartValue.Name.DefaultIfNullOrEmpty("TODO");
            Property.State.Expanded = EasyEditorGUI.Foldout(
                foldoutRect, Property.State.Expanded,
                TempContent.SetText(title).SetTooltip(ValueEntry.SmartValue.Guid.ToString("D")),
                MetroFoldoutGroupAttributeDrawer.FoldoutStyle);

            EditorGUILayout.EndHorizontal();

            if (Property.State.Expanded)
            {
                CallNextDrawer(label);

                EasyEditorGUI.Title("绘制工具");
                EasyEditorGUI.BeginBox();
                EditorGUILayout.BeginHorizontal();

                Button(DrawMode.Brush);
                EditorGUILayout.Space(3, false);
                Button(DrawMode.Eraser);
                EditorGUILayout.Space(3, false);
                Button(DrawMode.LineBrush);
                EditorGUILayout.Space(3, false);
                Button(DrawMode.RectangleBrush);

                EditorGUILayout.EndHorizontal();
                EasyEditorGUI.EndBox();
            }

            EasyEditorGUI.EndBox();
        }

        private bool IsSelected(DrawMode drawMode)
        {
            if (SelectedGuid == null || SelectedGuid != ValueEntry.SmartValue.Guid)
                return false;

            return SelectedDrawMode == drawMode;
        }

        private bool Button(DrawMode drawMode)
        {
            var isSelected = IsSelected(drawMode);
            if (isSelected)
            {
                EasyGUIHelper.PushColor(SelectedButtonColor);
            }
            var btnRect = GUILayoutUtility.GetRect(30, 30, GUILayout.ExpandWidth(false));
            var clicked = GUI.Button(btnRect, GUIContent.none);

            if (isSelected)
            {
                EasyGUIHelper.PopColor();
            }

            if (Event.current.type == EventType.Repaint)
            {
                var icon = TileWorldIcons.Instance.GetDrawModeIcon(drawMode);
                GUI.DrawTexture(btnRect.AlignCenter(25, 25), icon);
            }

            if (clicked)
            {
                if (SelectedGuid != null &&
                    SelectedGuid == ValueEntry.SmartValue.Guid &&
                    SelectedDrawMode == drawMode)
                {
                    SelectedGuid = null;
                }
                else
                {
                    SelectedGuid = ValueEntry.SmartValue.Guid;
                }

                SelectedDrawMode = drawMode;
            }

            return clicked;
        }
    }
}
