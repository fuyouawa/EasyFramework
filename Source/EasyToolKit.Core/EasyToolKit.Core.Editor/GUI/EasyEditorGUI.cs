using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Core.Editor
{
    public class EasyEditorGUI
    {
        private static readonly GUIScopeStack<Rect> verticalListBorderRects = new GUIScopeStack<Rect>();
        private static readonly List<int> currentListItemIndecies = new List<int>();
        private static float currentDrawingToolbarHeight;
        private static int currentScope = 0;

        /// <summary>
        /// Begins a vertical indentation. Remember to end with <see cref="EndIndentedVertical"/>.
        /// </summary>
        /// <param name="style">The style of the indentation.</param>
        /// <param name="options">The GUI layout options.</param>
        public static Rect BeginIndentedVertical(GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(GUIStyle.none);
            if (EditorGUI.indentLevel != 0)
            {
                var lblWith = EasyGUIHelper.BetterLabelWidth - EasyGUIHelper.CurrentIndentAmount;
                var overflow = 0f;
                if (lblWith < 1)
                {
                    lblWith = 1;
                    overflow = 1 - lblWith;
                }

                GUILayout.Space(overflow);
                EasyGUIHelper.PushLabelWidth(lblWith);
                IndentSpace();
            }

            EasyGUIHelper.PushIndentLevel(0);
            return EditorGUILayout.BeginVertical(style ?? GUIStyle.none, options);
        }


        /// <summary>
        /// Ends a identation vertical layout group started by <see cref="BeginIndentedVertical"/>.
        /// </summary>
        public static void EndIndentedVertical()
        {
            EditorGUILayout.EndVertical();
            EasyGUIHelper.PopIndentLevel();
            GUILayout.EndHorizontal();

            if (EditorGUI.indentLevel != 0)
            {
                EasyGUIHelper.PopLabelWidth();
            }
        }


        /// <summary>
        /// Indents by the current indent value, <see cref="EasyGUIHelper.CurrentIndentAmount"/>.
        /// </summary>
        public static void IndentSpace()
        {
            GUILayout.Space(EasyGUIHelper.CurrentIndentAmount);
        }

        public static Rect BeginHorizontalToolbar(float height = 22)
        {
            var rect = BeginHorizontalToolbar(EasyGUIStyles.ToolbarBackground, height);
            return rect;
        }

        public static Rect BeginHorizontalToolbar(GUIStyle style, float height = 22)
        {
            currentDrawingToolbarHeight = height;
            var rect = EditorGUILayout.BeginHorizontal(style, GUILayout.Height(height), GUILayout.ExpandWidth(false));
            EasyGUIHelper.PushHierarchyMode(false);
            EasyGUIHelper.PushIndentLevel(0);
            return rect;
        }

        public static void EndHorizontalToolbar()
        {
            if (Event.current.type == EventType.Repaint)
            {
                var rect = EasyGUIHelper.GetCurrentLayoutRect();
                rect.yMin -= 1;
                DrawBorders(rect, 1);
            }

            EasyGUIHelper.PopIndentLevel();
            EasyGUIHelper.PopHierarchyMode();
            EditorGUILayout.EndHorizontal();
        }

        public static bool ToolbarButton(EditorIcon icon, bool ignoreGUIEnabled = false)
        {
            var rect = GUILayoutUtility.GetRect(currentDrawingToolbarHeight, currentDrawingToolbarHeight,
                GUILayout.ExpandWidth(false));
            if (GUI.Button(rect, GUIContent.none, EasyGUIStyles.ToolbarButton))
            {
                EasyGUIHelper.RemoveFocusControl();
                EasyGUIHelper.RequestRepaint();
                return true;
            }

            if (Event.current.type == EventType.Repaint)
            {
                rect.y -= 1;
                icon.Draw(rect.AlignCenter(16, 16));
            }

            if (ignoreGUIEnabled)
            {
                if (Event.current.button == 0 && Event.current.rawType == EventType.MouseDown)
                {
                    if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        EasyGUIHelper.RemoveFocusControl();
                        EasyGUIHelper.RequestRepaint();
                        EasyGUIHelper.PushGUIEnabled(true);
                        Event.current.Use();
                        EasyGUIHelper.PopGUIEnabled();
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool ToolbarButton(GUIContent content, bool selected = false)
        {
            if (GUILayout.Button(content, selected ? EasyGUIStyles.ToolbarButtonSelected : EasyGUIStyles.ToolbarButton,
                    GUILayout.Height(currentDrawingToolbarHeight), GUILayout.ExpandWidth(false)))
            {
                EasyGUIHelper.RemoveFocusControl();
                EasyGUIHelper.RequestRepaint();
                return true;
            }

            return false;
        }

        public static bool ToolbarButton(string label, bool selected = false)
        {
            return ToolbarButton(EasyGUIHelper.TempContent(label), selected);
        }

        /// <summary>
        /// Begins drawing a box with toolbar style header. Remember to end with <seealso cref="EndToolbarBox"/>.
        /// </summary>
        /// <param name="options">GUILayout options.</param>
        /// <returns>The rect of the box.</returns>
        public static Rect BeginToolbarBox(params GUILayoutOption[] options)
        {
            BeginIndentedVertical(EasyGUIStyles.BoxContainer, options);
            EasyGUIHelper.PushHierarchyMode(false);
            EasyGUIHelper.PushLabelWidth(EasyGUIHelper.BetterLabelWidth - 4);
            return EasyGUIHelper.GetCurrentLayoutRect();
        }

        /// <summary>
        /// Ends the drawing a box with a toolbar style header started by <see cref="BeginToolbarBox(GUILayoutOption[])"/>.
        /// </summary>
        public static void EndToolbarBox()
        {
            EasyGUIHelper.PopLabelWidth();
            EasyGUIHelper.PopHierarchyMode();
            EndIndentedVertical();
        }
        
        /// <summary>
        /// Begins drawing a toolbar style box header. Remember to end with <see cref="EndToolbarBoxHeader"/>.
        /// </summary>
        /// <returns>The rect of the box.</returns>
        public static Rect BeginToolbarBoxHeader()
        {
            GUILayout.Space(-3);
            var headerBgRect = EditorGUILayout.BeginHorizontal(EasyGUIStyles.BoxHeaderStyle, GUILayout.ExpandWidth(true));
            GUILayout.Space(0);

            if (Event.current.type == EventType.Repaint)
            {
                var rect = headerBgRect;
                rect.x -= 3;
                rect.width += 6;
                //rect.y -= 1;
                //rect.height += 2;
                EasyGUIStyles.ToolbarBackground.Draw(rect, GUIContent.none, 0);
            }
            return headerBgRect;
        }

        /// <summary>
        /// Ends the drawing of a toolbar style box header started by <see cref="BeginToolbarBoxHeader"/>.
        /// </summary>
        public static void EndToolbarBoxHeader()
        {
            EditorGUILayout.EndHorizontal();
        }

        public static bool IconButton(EditorIcon icon, GUIStyle style, int width = 18, int height = 18,
            string tooltip = "")
        {
            Rect rect = GUILayoutUtility.GetRect(icon.HighlightedContent, style, GUILayout.ExpandWidth(false),
                GUILayout.Width(width), GUILayout.Height(height));
            return IconButton(rect, icon, style, tooltip);
        }

        public static bool IconButton(Rect rect, EditorIcon icon, GUIStyle style = null, string tooltip = "")
        {
            style ??= EasyGUIStyles.IconButton;
            if (GUI.Button(rect, GUIContent.none, style))
            {
                EasyGUIHelper.RemoveFocusControl();
                return true;
            }

            if (Event.current.type == EventType.Repaint)
            {
                float size = Mathf.Min(rect.height, rect.width);
                icon.Draw(rect.AlignCenter(size, size));
            }

            return false;
        }

        public static Rect BeginVerticalList(bool drawBorder = true, bool drawDarkBg = true,
            params GUILayoutOption[] options)
        {
            currentScope++;
            currentListItemIndecies.Resize(Mathf.Max(currentListItemIndecies.Count, currentScope + 1));
            currentListItemIndecies[currentScope] = 0;

            if (Event.current.type == EventType.MouseMove)
            {
                EasyGUIHelper.RequestRepaint();
            }

            var rect = EditorGUILayout.BeginVertical(options);

            if (drawDarkBg)
            {
                DrawSolidRect(rect, EasyGUIStyles.ListItemDragBgColor);
            }

            if (drawBorder)
            {
                verticalListBorderRects.Push(rect);
            }
            else
            {
                verticalListBorderRects.Push(new Rect(-1, rect.y, rect.width, rect.height));
            }

            return rect;
        }

        public static void EndVerticalList()
        {
            currentScope--;
            var rect = verticalListBorderRects.Pop();
            if (rect.x > 0)
            {
                rect.y -= 1;
                rect.height += 1;
                DrawBorders(rect, 1, 1, 1, 1);
            }

            EditorGUILayout.EndVertical();
        }

        public static Rect BeginListItem(bool allowHover, GUIStyle style, params GUILayoutOption[] options)
        {
            currentListItemIndecies.Resize(Mathf.Max(currentListItemIndecies.Count, currentScope));
            int i = currentListItemIndecies[currentScope];
            currentListItemIndecies[currentScope] = i + 1;

            GUILayout.BeginVertical(style ?? EasyGUIStyles.ListItem, options);
            var rect = EasyGUIHelper.GetCurrentLayoutRect();
            var isMouseOver = rect.Contains(Event.current.mousePosition);

            if (Event.current.type == EventType.Repaint)
            {
                Color color = i % 2 == 0 ? EasyGUIStyles.ListItemColorEven : EasyGUIStyles.ListItemColorOdd;
                Color hover = color;
                if ( /* DragAndDropManager.IsDragInProgress == false && */allowHover)
                {
                    hover = i % 2 == 0 ? EasyGUIStyles.ListItemColorHoverEven : EasyGUIStyles.ListItemColorHoverOdd;
                }

                DrawSolidRect(rect, isMouseOver ? hover : color);
            }

            return rect;
        }

        public static void EndListItem()
        {
            GUILayout.EndVertical();
        }

        public static void DrawBorders(Rect rect, int left, int right, int top, int bottom, bool usePlayModeTint = true)
        {
            DrawBorders(rect, left, right, top, bottom, EasyGUIStyles.BorderColor, usePlayModeTint);
        }

        public static void DrawBorders(Rect rect, int borderWidth, bool usePlayModeTint = true)
        {
            DrawBorders(rect, borderWidth, borderWidth, borderWidth, borderWidth, EasyGUIStyles.BorderColor,
                usePlayModeTint);
        }

        public static void DrawBorders(Rect rect, int left, int right, int top, int bottom, Color color,
            bool usePlayModeTint = true)
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (left > 0)
                {
                    var borderRect = rect;
                    borderRect.width = left;
                    DrawSolidRect(borderRect, color, usePlayModeTint);
                }

                if (top > 0)
                {
                    var borderRect = rect;
                    borderRect.height = top;
                    DrawSolidRect(borderRect, color, usePlayModeTint);
                }

                if (right > 0)
                {
                    var borderRect = rect;
                    borderRect.x += rect.width - right;
                    borderRect.width = right;
                    DrawSolidRect(borderRect, color, usePlayModeTint);
                }

                if (bottom > 0)
                {
                    var borderRect = rect;
                    borderRect.y += rect.height - bottom;
                    borderRect.height = bottom;
                    DrawSolidRect(borderRect, color, usePlayModeTint);
                }
            }
        }


        public static void DrawSolidRect(Rect rect, Color color, bool usePlayModeTint = true)
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (usePlayModeTint)
                {
                    EditorGUI.DrawRect(rect, color);
                }
                else
                {
                    EasyGUIHelper.PushColor(color);
                    GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
                    EasyGUIHelper.PopColor();
                }
            }
        }
    }
}
