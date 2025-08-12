using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public static bool ToolbarButton(Rect rect, EditorIcon icon, bool ignoreGUIEnabled = false)
        {
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

        public static bool ToolbarButton(EditorIcon icon, bool ignoreGUIEnabled = false)
        {
            var rect = GUILayoutUtility.GetRect(currentDrawingToolbarHeight, currentDrawingToolbarHeight,
                GUILayout.ExpandWidth(false));
            return ToolbarButton(rect, icon, ignoreGUIEnabled);
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
        /// Begins drawing a box with toolbar style header. Remember to end with <seealso cref="EndBox"/>.
        /// </summary>
        /// <param name="options">GUILayout options.</param>
        /// <returns>The rect of the box.</returns>
        public static Rect BeginBox(params GUILayoutOption[] options)
        {
            BeginIndentedVertical(EasyGUIStyles.BoxContainer, options);
            EasyGUIHelper.PushHierarchyMode(false);
            EasyGUIHelper.PushLabelWidth(EasyGUIHelper.BetterLabelWidth - 4);
            return EasyGUIHelper.GetCurrentLayoutRect();
        }

        /// <summary>
        /// Ends the drawing a box with a toolbar style header started by <see cref="BeginBox"/>.
        /// </summary>
        public static void EndBox()
        {
            EasyGUIHelper.PopLabelWidth();
            EasyGUIHelper.PopHierarchyMode();
            EndIndentedVertical();
        }

        public static Rect BeginBoxHeader()
        {
            GUILayout.Space(-3);
            var headerBgRect = EditorGUILayout.BeginHorizontal(EasyGUIStyles.BoxHeaderStyle, GUILayout.ExpandWidth(true));

            if (Event.current.type == EventType.Repaint)
            {
                headerBgRect.x -= 3;
                headerBgRect.width += 6;
                EasyGUIHelper.PushColor(EasyGUIStyles.HeaderBoxBackgroundColor);
                GUI.DrawTexture(headerBgRect, Texture2D.whiteTexture);
                EasyGUIHelper.PopColor();
                DrawBorders(headerBgRect, 0, 0, 0, 1, new Color(0, 0, 0, 0.4f));
            }

            EasyGUIHelper.PushLabelWidth(EasyGUIHelper.BetterLabelWidth - 4);
            return headerBgRect;
        }

        /// <summary>
        /// Ends drawing a box header started by <see cref="BeginBoxHeader"/>,
        /// </summary>
        public static void EndBoxHeader()
        {
            EasyGUIHelper.PopLabelWidth();
            EditorGUILayout.EndHorizontal();
        }

        public static bool IconButton(EditorIcon icon, GUIStyle style = null, int width = 18, int height = 18,
            string tooltip = "")
        {
            style ??= EasyGUIStyles.IconButton;
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

        /// <summary>
        /// Draws a foldout field where clicking on the label toggles to the foldout too.
        /// </summary>
        /// <param name="isVisible">The current state of the foldout.</param>
        /// <param name="label">The label of the foldout.</param>
        /// <param name="style">The GUI style.</param>
        public static bool Foldout(bool isVisible, GUIContent label, GUIStyle style = null)
        {
            var tmp = EditorGUIUtility.fieldWidth;
            EditorGUIUtility.fieldWidth = 10;
            var rect = EditorGUILayout.GetControlRect(false);
            EditorGUIUtility.fieldWidth = tmp;

            return Foldout(rect, isVisible, label, style);
        }
        /// <summary>
        /// Draws a foldout field where clicking on the label toggles to the foldout too.
        /// </summary>
        /// <param name="isVisible">The current state of the foldout.</param>
        /// <param name="label">The label of the foldout.</param>
        /// <param name="valueRect">The value rect.</param>
        /// <param name="style">The GUI style.</param>
        public static bool Foldout(bool isVisible, GUIContent label, out Rect valueRect, GUIStyle style = null)
        {
            Rect labelRect = EditorGUILayout.GetControlRect(false);
            valueRect = labelRect;

            if (label == null)
            {
                label = new GUIContent(" ");

                if (EditorGUIUtility.hierarchyMode)
                {
                    labelRect.width = 2;
                }
                else
                {
                    labelRect.width = 18;
                    valueRect.xMin += 18;
                }
            }
            else
            {
                var indent = EasyGUIHelper.CurrentIndentAmount;
                labelRect = new Rect(labelRect.x, labelRect.y, EasyGUIHelper.BetterLabelWidth - indent, labelRect.height);
                valueRect.xMin = labelRect.xMax;
            }

            return Foldout(labelRect, isVisible, label);
        }

        /// <summary>
        /// Draws a foldout field where clicking on the label toggles to the foldout too.
        /// </summary>
        /// <param name="rect">The rect to draw the foldout field in.</param>
        /// <param name="isVisible">The current state of the foldout.</param>
        /// <param name="label">The label of the foldout.</param>
        /// <param name="style">The style.</param>
        public static bool Foldout(Rect rect, bool isVisible, GUIContent label, GUIStyle style = null)
        {
            style = style ?? EasyGUIStyles.Foldout;

            var e = Event.current.type;
            bool isHovering = false;
            if (e != EventType.Layout)
            {
                // Swallow foldout icon as well
                //rect.x -= 9;
                //rect.width += 9;
                isHovering = rect.Contains(Event.current.mousePosition);
                //rect.width -= 9;
                //rect.x += 9;
            }

            if (isHovering)
            {
                EasyGUIHelper.PushLabelColor(EasyGUIStyles.HighlightedTextColor);
            }

            if (isHovering && e == EventType.MouseMove)
            {
                EasyGUIHelper.RequestRepaint();
            }

            if (e == EventType.MouseDown && isHovering && Event.current.button == 0)
            {
                // Foldout works when GUI.enabled = false
                // Enable GUI, in order to Use() the the event properly.
                isVisible = !isVisible;
                EasyGUIHelper.RequestRepaint();
                EasyGUIHelper.PushGUIEnabled(true);
                Event.current.Use();
                EasyGUIHelper.PopGUIEnabled();
                EasyGUIHelper.RemoveFocusControl();
            }

            isVisible = EditorGUI.Foldout(rect, isVisible, label, style);

            if (isHovering)
            {
                EasyGUIHelper.PopLabelColor();
            }

            return isVisible;
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

        /// <summary>
        /// Draws a horizontal line separator.
        /// </summary>
        /// <param name="color">The color of the line.</param>
        /// <param name="lineWidth">The size of the line.</param>
        public static void HorizontalLineSeparator(Color color, int lineWidth = 1)
        {
            Rect rect = GUILayoutUtility.GetRect(lineWidth, lineWidth, GUILayout.ExpandWidth(true));
            DrawSolidRect(rect, color, true);
        }

        public static void Title(string title, string subtitle = null, TextAlignment textAlignment = TextAlignment.Left, bool horizontalLine = true, bool boldLabel = true)
        {
            GUIStyle titleStyle = null;
            GUIStyle subtitleStyle = null;

            switch (textAlignment)
            {
                case TextAlignment.Left:
                    titleStyle = boldLabel ? EasyGUIStyles.BoldTitle : EasyGUIStyles.Title;
                    subtitleStyle = EasyGUIStyles.Subtitle;
                    break;

                case TextAlignment.Center:
                    titleStyle = boldLabel ? EasyGUIStyles.BoldTitleCentered : EasyGUIStyles.TitleCentered;
                    subtitleStyle = EasyGUIStyles.SubtitleCentered;
                    break;

                case TextAlignment.Right:
                    titleStyle = boldLabel ? EasyGUIStyles.BoldTitleRight : EasyGUIStyles.TitleRight;
                    subtitleStyle = EasyGUIStyles.SubtitleRight;
                    break;

                default:
                    // Hidden feature by calling: Title("title", "subTitle", (TextAlignment)3, true, true);
                    // This hidden feature is added because the TitleAlignment enum located in the Sirenix.OdinInspector.Attribute assembly have an extra split option.
                    // But we don't have access to the assembly from here.
                    titleStyle = boldLabel ? EasyGUIStyles.BoldTitle : EasyGUIStyles.Title;
                    subtitleStyle = EasyGUIStyles.SubtitleRight;
                    break;
            }

            Rect rect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, 19, titleStyle));
            GUI.Label(rect, title, titleStyle);

            if (subtitle != null && !subtitle.IsNullOrWhiteSpace())
            {
                rect = EditorGUI.IndentedRect(GUILayoutUtility.GetRect(EasyGUIHelper.TempContent(subtitle), subtitleStyle));
                GUI.Label(rect, subtitle, subtitleStyle);
            }

            if (horizontalLine)
            {
                DrawSolidRect(rect.AlignBottom(1), EasyGUIStyles.LightBorderColor);
                GUILayout.Space(3f);
            }
        }

        public static void ShowValueDropdownMenu<T>(Rect position, IReadOnlyList<T> values, Action<T> onSelected)
        {
            ShowValueDropdownMenu(position, values, onSelected, value => new GUIContent(value.ToString()));
        }

        public static void ShowValueDropdownMenu<T>(Rect position, IReadOnlyList<T> values, Action<T> onSelected, Func<T, GUIContent> optionContentGetter)
        {
            ShowValueDropdownMenu(position, values, onSelected, (index, value) => optionContentGetter(value));
        }

        public static void ShowValueDropdownMenu<T>(Rect position, IReadOnlyList<T> values, Action<T> onSelected, Func<int, T, GUIContent> optionContentGetter)
        {
            var options = values.Select((value, index) => optionContentGetter(index, value)).ToArray();
            EditorUtility.DisplayCustomMenu(position, options, -1, (userData, options, selected) =>
            {
                onSelected(values[selected]);
            }, null);
        }

        public static void MessageBox(string message, MessageType messageType)
        {
            MessageBox(message, messageType, EasyGUIStyles.MessageBox);
        }

        public static void MessageBox(string message, MessageType messageType, GUIStyle messageStyle)
        {
            var icon = EasyGUIHelper.GetHelpIcon(messageType);
            var content = EasyGUIHelper.TempContent(message, image: icon);
            var rect = GUILayoutUtility.GetRect(content, messageStyle);
            rect = EditorGUI.IndentedRect(rect);
            GUI.Label(rect, content, messageStyle);
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
