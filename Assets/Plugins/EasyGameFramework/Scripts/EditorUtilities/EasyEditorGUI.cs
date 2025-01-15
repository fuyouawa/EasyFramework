#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using EasyFramework;
using Sirenix.OdinInspector;
using System.Reflection;
using System.Globalization;

namespace EasyGameFramework
{
    public delegate void OnCoveredTitleBarGUIDelegate(Rect headerRect);

    public delegate void OnTitleBarGUIDelegate(Rect headerRect);

    public delegate void OnContentGUIDelegate(Rect headerRect);

    public delegate void OnBeforeFoldoutGUIDelegate(Rect headerRect);

    public class PopupSelectorConfig<T>
    {
        public IEnumerable<T> Collection;
        public Action<T> OnConfirmed;
        public Func<T, string> MenuItemNameGetter = null;
        public string Title = null;
        public bool SupportsMultiSelect = false;

        public PopupSelectorConfig(IEnumerable<T> collection, Action<T> onConfirmed)
        {
            Collection = collection;
            OnConfirmed = onConfirmed;
        }
    }

    public class SelectorDropdownConfig<T> : PopupSelectorConfig<T>
    {
        public GUIContent Label;
        public GUIContent BtnLabel;
        public bool ReturnValuesOnSelectionChange = true;
        public GUIStyle Style;

        public SelectorDropdownConfig(string label, string btnLabel, IEnumerable<T> collection,
            Action<T> onConfirmed)
            : base(collection, onConfirmed)
        {
            Label = new GUIContent(label);
            BtnLabel = new GUIContent(btnLabel);
        }

        public SelectorDropdownConfig(GUIContent label, GUIContent btnLabel, IEnumerable<T> collection,
            Action<T> onConfirmed)
            : base(collection, onConfirmed)
        {
            Label = label;
            BtnLabel = btnLabel;
        }
    }

    public class FoldoutHeaderConfig
    {
        public GUIContent Label;
        public bool Expand;
        public bool Expandable = true;
        public Color? BoxColor;
        public OnCoveredTitleBarGUIDelegate OnCoveredTitleBarGUI;
        public GUIContent RightLabel = GUIContent.none;

        public FoldoutHeaderConfig(string label, bool expand = true)
        {
            Label = new GUIContent(label);
            Expand = expand;
        }

        public FoldoutHeaderConfig(GUIContent label, bool expand = true)
        {
            Label = label;
            Expand = expand;
        }
    }

    public class FoldoutGroupConfig : FoldoutHeaderConfig
    {
        public object Key;
        public OnTitleBarGUIDelegate OnTitleBarGUI;
        public OnContentGUIDelegate OnContentGUI;

        public FoldoutGroupConfig(object key, string label, bool expand = true)
            : this(key, new GUIContent(label), expand)
        {
        }

        public FoldoutGroupConfig(object key, GUIContent label, bool expand = true)
            : base(label, expand)
        {
            Key = key;
        }
    }

    public class BoxGroupConfig
    {
        public GUIContent Label;
        public Color? BoxColor;
        public GUIContent RightLabel = GUIContent.none;
        public OnCoveredTitleBarGUIDelegate OnCoveredTitleBarGUI;
        public OnTitleBarGUIDelegate OnTitleBarGUI;
        public OnContentGUIDelegate OnContentGUI;

        public BoxGroupConfig(string label)
        {
            Label = new GUIContent(label);
        }

        public BoxGroupConfig(GUIContent label)
        {
            Label = label;
        }
    }

    public class FoldoutToolbarConfig
    {
        public GUIContent Label;
        public bool Expand;
        public bool ShowFoldout = true;

        public FoldoutToolbarConfig(string label, bool expand = true)
        {
            Label = new GUIContent(label);
            Expand = expand;
        }

        public FoldoutToolbarConfig(GUIContent label, bool expand = true)
        {
            Label = label;
            Expand = expand;
        }
    }

    public class WindowLikeToolbarConfig : FoldoutToolbarConfig
    {
        public Action OnMaximize = null;
        public Action OnMinimize = null;
        public string ExpandButtonTooltip = "展开所有";
        public string CollapseButtonTooltip = "折叠所有";

        public WindowLikeToolbarConfig(string label, bool expand = true) : base(label, expand)
        {
        }

        public WindowLikeToolbarConfig(GUIContent label, bool expand = true) : base(label, expand)
        {
        }
    }

    public class WindowLikeToolGroupConfig : WindowLikeToolbarConfig
    {
        public object Key;
        public Action<Rect> OnTitleBarGUI = null;
        public Action OnContentGUI = null;

        public WindowLikeToolGroupConfig(object key, string label, bool expand = true) : base(label, expand)
        {
            Key = key;
        }

        public WindowLikeToolGroupConfig(object key, GUIContent label, bool expand = true) : base(label, expand)
        {
            Key = key;
        }
    }

    public class TreeNodeState
    {
        public delegate void OnExpandChangedDelegate(bool expand);

        public bool Expand = false;
        public bool? Expandable;
        public Color? BoxColor;

        public OnExpandChangedDelegate OnExpandChanged;
    }

    public struct TreeNodeInfo
    {
        public bool IsLastNode;
        public TreeNodeState State;
    }

    public class TreeGroupConfig<TElement>
    {
        public delegate string NodeLabelGetterDelegate(TElement node);

        public delegate IList<TElement> NodeChildrenGetterDelegate(TElement node);

        public delegate TreeNodeState NodeStateGetterDelegate(TElement node);

        public delegate void OnNodeTitleBarGUIDelegate(TElement node, Rect headerRect, TreeNodeInfo info);

        public delegate void OnNodeConveredTitleBarGUIDelegate(TElement node, Rect headerRect, TreeNodeInfo info);

        public delegate void OnBeforeChildrenContentGUIDelegate(TElement node, Rect headerRect, TreeNodeInfo info);

        public delegate void OnAfterChildrenContentGUIDelegate(TElement node, Rect headerRect, TreeNodeInfo info);

        public object Key;
        public GUIContent Label;
        public bool Expand;

        public NodeLabelGetterDelegate NodeLabelGetter;
        public NodeChildrenGetterDelegate NodeChildrenGetter;
        public NodeStateGetterDelegate NodeStateGetter;

        public OnTitleBarGUIDelegate OnTitleBarGUI;
        public OnNodeTitleBarGUIDelegate OnNodeTitleBarGUI;
        public OnNodeConveredTitleBarGUIDelegate OnNodeConveredTitleBarGUI;

        public OnBeforeChildrenContentGUIDelegate OnBeforeChildrenContentGUI;
        public OnAfterChildrenContentGUIDelegate OnAfterChildrenContentGUI;

        public TreeGroupConfig(object key, string label,
            NodeLabelGetterDelegate nodeLabelGetter,
            NodeChildrenGetterDelegate nodeChildrenGetter,
            NodeStateGetterDelegate nodeStateGetter,
            bool expand = true) : this(key, new GUIContent(label), nodeLabelGetter, nodeChildrenGetter,
            nodeStateGetter, expand)
        {
        }

        public TreeGroupConfig(object key, GUIContent label,
            NodeLabelGetterDelegate nodeLabelGetter,
            NodeChildrenGetterDelegate nodeChildrenGetter,
            NodeStateGetterDelegate nodeStateGetter,
            bool expand = true)
        {
            Key = key;
            NodeLabelGetter = nodeLabelGetter;
            NodeChildrenGetter = nodeChildrenGetter;
            NodeStateGetter = nodeStateGetter;
            Label = label;
            Expand = expand;
        }
    }


    public static class EasyEditorGUI
    {
        #region Internal

        private static readonly GUIContent _text = new GUIContent();
        private static readonly GUIContent _text2 = new GUIContent();

        private static readonly Dictionary<Type, Stack<object>> InfoStack = new Dictionary<Type, Stack<object>>();

        private static void PushContext(Type infoType, object info)
        {
            if (InfoStack.Count > 1024)
            {
                throw new Exception("Stack leak of EasyEditorGUI.Begin - End api!");
            }

            if (!InfoStack.TryGetValue(infoType, out var stack))
            {
                stack = new Stack<object>();
                InfoStack[infoType] = stack;
            }

            stack.Push(info);
        }

        private static void PushContext<T>(T info)
        {
            PushContext(typeof(T), info);
        }

        private static T PopContext<T>()
        {
            return (T)PopContext(typeof(T));
        }

        private static object PopContext(Type infoType)
        {
            if (!InfoStack.TryGetValue(infoType, out var stack))
            {
                throw new ArgumentException(
                    $"The type({infoType.Name}) cannot be found in stack, perhaps because End does not match Begin");
            }

            return stack.Pop();
        }

        #endregion

        #region Extension

        public static bool HasKeyboardFocus(int controlId)
        {
            return (bool)typeof(EditorGUI).InvokeMethod("HasKeyboardFocus", null, controlId);
        }

        public static void EndEditingActiveTextField()
        {
            typeof(EditorGUI).InvokeMethod("EndEditingActiveTextField", null);
        }

        public static bool ToolbarButton(GUIContent content, float width, bool selected = false)
        {
            var w = SirenixEditorGUI.currentDrawingToolbarHeight;

            if (GUILayout.Button(content, selected
                    ? SirenixGUIStyles.ToolbarButtonSelected
                    : SirenixGUIStyles.ToolbarButton, GUILayoutOptions.Height(w).ExpandWidth(false).Width(width)))
            {
                GUIHelper.RemoveFocusControl();
                GUIHelper.RequestRepaint();
                return true;
            }

            return false;
        }

        public static Rect GetIndentControlRect(params GUILayoutOption[] options)
        {
            return GetIndentControlRect(true, 18, options);
        }

        public static Rect GetIndentControlRect(
            bool hasLabel,
            params GUILayoutOption[] options)
        {
            return GetIndentControlRect(hasLabel, 18, options);
        }

        public static Rect GetIndentControlRect(
            bool hasLabel,
            float height,
            params GUILayoutOption[] options)
        {
            return GetIndentControlRect(hasLabel, height, EditorStyles.layerMaskField, options);
        }

        public static Rect GetIndentControlRect(
            bool hasLabel,
            float height,
            GUIStyle style,
            params GUILayoutOption[] options)
        {
            return EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(hasLabel, height, style, options));
        }

        #endregion

        #region Title

        public static void BigTitle(string title, string subtitle = null,
            TextAlignment textAlignment = TextAlignment.Left, bool horizontalLine = true,
            bool boldLabel = true)
        {
            Title(title, subtitle, textAlignment, horizontalLine, boldLabel,
                16, null);
        }

        public static void Title(string title, string subtitle = null, TextAlignment textAlignment = TextAlignment.Left,
            bool horizontalLine = true,
            bool boldLabel = true)
        {
            Title(title, subtitle, textAlignment, horizontalLine, boldLabel, 13,
                null);
        }

        public static void Title(string title, string subtitle, TextAlignment textAlignment, bool horizontalLine,
            bool boldLabel, int fontSize, Font font)
        {
            GUIStyle titleStyle = null;
            GUIStyle subtitleStyle = null;
            switch ((int)textAlignment)
            {
                case 0:
                    titleStyle = (boldLabel ? SirenixGUIStyles.BoldTitle : SirenixGUIStyles.Title);
                    subtitleStyle = SirenixGUIStyles.Subtitle;
                    break;
                case 1:
                    titleStyle = (boldLabel ? SirenixGUIStyles.BoldTitleCentered : SirenixGUIStyles.TitleCentered);
                    subtitleStyle = SirenixGUIStyles.SubtitleCentered;
                    break;
                case 2:
                    titleStyle = (boldLabel ? SirenixGUIStyles.BoldTitleRight : SirenixGUIStyles.TitleRight);
                    subtitleStyle = SirenixGUIStyles.SubtitleRight;
                    break;
                default:
                    titleStyle = (boldLabel ? SirenixGUIStyles.BoldTitle : SirenixGUIStyles.Title);
                    subtitleStyle = SirenixGUIStyles.SubtitleRight;
                    break;
            }

            titleStyle = new GUIStyle(titleStyle)
            {
                font = font,
                fontSize = fontSize
            };
            Rect rect;
            if ((int)textAlignment > 2)
            {
                rect = GUILayoutUtility.GetRect(0f, 18f, titleStyle, GUILayoutOptions.ExpandWidth());
                GUI.Label(rect, title, titleStyle);
                rect.y += 3f;
                GUI.Label(rect, subtitle, subtitleStyle);
                if (horizontalLine)
                {
                    SirenixEditorGUI.HorizontalLineSeparator(SirenixGUIStyles.LightBorderColor);
                    GUILayout.Space(3f);
                }

                return;
            }

            rect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false));
            GUI.Label(rect, title, titleStyle);
            if (subtitle != null && !subtitle.IsNullOrWhitespace())
            {
                rect = EditorGUI.IndentedRect(GUILayoutUtility.GetRect(GUIHelper.TempContent(subtitle), subtitleStyle));
                GUI.Label(rect, subtitle, subtitleStyle);
            }

            if (horizontalLine)
            {
                SirenixEditorGUI.DrawSolidRect(rect.AlignBottom(1f), SirenixGUIStyles.LightBorderColor);
                GUILayout.Space(3f);
            }
        }

        #endregion

        #region Selector

        public static IEnumerable<T> DrawSelectorDropdown<T>(SelectorDropdownConfig<T> config,
            params GUILayoutOption[] options)
        {
            var btnLabel = config.BtnLabel;
            if (typeof(T) == typeof(Type))
            {
                btnLabel.image = GUIHelper.GetAssetThumbnail(null, typeof(T), false);
            }

            return OdinSelector<T>.DrawSelectorDropdown(config.Label, config.BtnLabel,
                rect => ShowSelectorInPopup(rect, rect.width, config),
                config.ReturnValuesOnSelectionChange, config.Style, options);
        }

        private static OdinSelector<T> GetSelector<T>(PopupSelectorConfig<T> config)
        {
            OdinSelector<T> selector;

            if (config.MenuItemNameGetter != null)
            {
                selector = new GenericSelector<T>(config.Title, config.Collection, config.SupportsMultiSelect,
                    t => config.MenuItemNameGetter(t));
            }
            else
            {
                selector = new GenericSelector<T>(config.Title, config.Collection, config.SupportsMultiSelect);
            }

            selector.SelectionConfirmed += types =>
            {
                var f = types.FirstOrDefault();
                if (f != null)
                {
                    config.OnConfirmed?.Invoke(f);
                }
            };
            selector.SelectionTree.EnumerateTree().AddThumbnailIcons(true);
            selector.SelectionChanged += types => { selector.SelectionTree.Selection.ConfirmSelection(); };
            return selector;
        }


        public static OdinSelector<T> ShowSelectorInPopup<T>(PopupSelectorConfig<T> config)
        {
            var selector = GetSelector(config);
            selector.ShowInPopup();
            return selector;
        }

        public static OdinSelector<T> ShowSelectorInPopup<T>(Rect rect, PopupSelectorConfig<T> config)
        {
            var selector = GetSelector(config);
            selector.ShowInPopup(rect);
            return selector;
        }

        public static OdinSelector<T> ShowSelectorInPopup<T>(Rect btnRect, float windowWidth,
            PopupSelectorConfig<T> config)
        {
            var selector = GetSelector(config);
            selector.ShowInPopup(btnRect, windowWidth);
            return selector;
        }

        public static OdinSelector<T> ShowSelectorInPopup<T>(float windowWidth, PopupSelectorConfig<T> config)
        {
            var selector = GetSelector(config);
            selector.ShowInPopup(windowWidth);
            return selector;
        }

        #endregion

        #region Foldout

        public static bool FoldoutHeader(FoldoutHeaderConfig config)
        {
            var e = BeginFoldoutHeader(config);
            EndFoldoutHeader();
            return e;
        }

        public static bool FoldoutHeader(FoldoutHeaderConfig config, out Rect headerRect)
        {
            var e = BeginFoldoutHeader(config, out headerRect);
            EndFoldoutHeader();
            return e;
        }

        public static bool FoldoutGroup(FoldoutGroupConfig config)
        {
            var color = GUI.color;
            if (config.BoxColor != null)
            {
                GUI.color = (Color)config.BoxColor;
            }

            SirenixEditorGUI.BeginBox();
            GUI.color = color;

            config.Expand = BeginFoldoutHeader(config, out var headerRect);
            config.OnTitleBarGUI?.Invoke(headerRect);
            EndFoldoutHeader();

            if (config.Expandable)
            {
                if (SirenixEditorGUI.BeginFadeGroup(config.Key, config.Expand))
                {
                    config.OnContentGUI?.Invoke(headerRect);
                }

                SirenixEditorGUI.EndFadeGroup();
            }
            else
            {
                config.OnContentGUI?.Invoke(headerRect);
            }

            SirenixEditorGUI.EndBox();

            return config.Expand;
        }

        public static bool BeginFoldoutHeader(FoldoutHeaderConfig config)
        {
            return BeginFoldoutHeader(config, out _);
        }

        public static bool BeginFoldoutHeader(FoldoutHeaderConfig config, out Rect headerRect)
        {
            // var color = GUI.color;
            // if (config.BoxColor != null)
            // {
            //     GUI.color = (Color)config.BoxColor;
            // }
            SirenixEditorGUI.BeginBoxHeader();
            // GUI.color = color;

            headerRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false));

            if (config.RightLabel != null && config.RightLabel != GUIContent.none)
            {
                var s = SirenixGUIStyles.Label.CalcSize(config.RightLabel);
                EditorGUI.PrefixLabel(headerRect.AlignRight(s.x), config.RightLabel);
            }

            config.OnCoveredTitleBarGUI?.Invoke(headerRect);
            if (config.Expandable)
            {
                config.Expand = SirenixEditorGUI.Foldout(headerRect, config.Expand, config.Label);
            }
            else
            {
                var rect = headerRect;
                // rect.x += 13;
                EditorGUI.LabelField(rect, config.Label);
            }

            return config.Expand;
        }

        public static void EndFoldoutHeader()
        {
            SirenixEditorGUI.EndBoxHeader();
        }

        #endregion

        #region Box

        public static void BoxGroup(BoxGroupConfig config)
        {
            BoxGroup(config, out var rect);
        }

        public static void BoxGroup(BoxGroupConfig config, out Rect headerRect)
        {
            var color = GUI.color;
            if (config.BoxColor != null)
            {
                GUI.color = (Color)config.BoxColor;
            }

            SirenixEditorGUI.BeginBox();
            GUI.color = color;

            SirenixEditorGUI.BeginBoxHeader();
            // GUI.color = color;

            headerRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false));

            if (config.RightLabel != null && config.RightLabel != GUIContent.none)
            {
                var s = SirenixGUIStyles.Label.CalcSize(config.RightLabel);
                EditorGUI.PrefixLabel(headerRect.AlignRight(s.x), config.RightLabel);
            }

            config.OnCoveredTitleBarGUI?.Invoke(headerRect);

            var rect = headerRect;
            // rect.x += 13;
            EditorGUI.LabelField(rect, config.Label);

            config.OnTitleBarGUI?.Invoke(rect);

            SirenixEditorGUI.EndBoxHeader();

            config.OnContentGUI?.Invoke(headerRect);

            SirenixEditorGUI.EndBox();
        }

        #endregion

        #region WindowLikeToolBar

        public static bool WindowLikeToolGroup(WindowLikeToolGroupConfig config)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                config.Expand = BeginWindowLikeToolbar(config, out var headerRect);
                config.OnTitleBarGUI?.Invoke(headerRect);
                EndWindowLikeToolbar();

                EditorGUILayout.Space(-2);
                SirenixEditorGUI.BeginBox();
                if (SirenixEditorGUI.BeginFadeGroup(config.Key, config.Expand))
                {
                    config.OnContentGUI?.Invoke();
                }

                SirenixEditorGUI.EndFadeGroup();
                SirenixEditorGUI.EndBox();
            }

            return config.Expand;
        }

        public static bool WindowLikeToolbar(WindowLikeToolbarConfig config)
        {
            var e = BeginWindowLikeToolbar(config);
            EndWindowLikeToolbar();
            return e;
        }

        public static bool BeginWindowLikeToolbar(WindowLikeToolbarConfig config)
        {
            return BeginWindowLikeToolbar(config, out _);
        }

        public static bool BeginWindowLikeToolbar(WindowLikeToolbarConfig config, out Rect headerRect)
        {
            var expand = BeginFoldoutToolbar(config, out headerRect);

            if (ToolbarButton(
                    new GUIContent(EasyEditorIcons.Expand.image, tooltip: config.ExpandButtonTooltip),
                    SirenixEditorGUI.currentDrawingToolbarHeight))
            {
                config.OnMaximize?.Invoke();
            }

            if (ToolbarButton(
                    new GUIContent(EasyEditorIcons.Collapse.image, tooltip: config.CollapseButtonTooltip),
                    SirenixEditorGUI.currentDrawingToolbarHeight))
            {
                config.OnMinimize?.Invoke();
            }

            return expand;
        }

        public static void EndWindowLikeToolbar()
        {
            EndFoldoutToolbar();
        }

        #endregion

        #region FoldoutToolBar

        public static bool FoldoutToolbar(FoldoutToolbarConfig config)
        {
            var e = BeginFoldoutToolbar(config);
            EndFoldoutToolbar();
            return e;
        }

        public static bool BeginFoldoutToolbar(FoldoutToolbarConfig config)
        {
            return BeginFoldoutToolbar(config, out _);
        }

        public static bool BeginFoldoutToolbar(FoldoutToolbarConfig config, out Rect headerRect)
        {
            SirenixEditorGUI.BeginHorizontalToolbar();

            if (!config.ShowFoldout)
            {
                GUILayout.Label(config.Label, GUILayoutOptions.ExpandWidth(expand: false));
                headerRect = GUILayoutUtility.GetLastRect();
            }
            else
            {
                float tmp = EditorGUIUtility.fieldWidth;
                EditorGUIUtility.fieldWidth = 10f;
                headerRect = EditorGUILayout.GetControlRect(false);
                EditorGUIUtility.fieldWidth = tmp;
            }

            GUILayout.FlexibleSpace();

            config.Expand = !config.ShowFoldout || SirenixEditorGUI.Foldout(headerRect, config.Expand, config.Label);

            return config.Expand;
        }

        public static void EndFoldoutToolbar()
        {
            SirenixEditorGUI.EndHorizontalToolbar();
        }

        #endregion

        #region TreeGroup

        private static void DrawTreeNode<TElement>(TElement node, int level, TreeGroupConfig<TElement> config)
        {
            var children = config.NodeChildrenGetter(node);

            var state = config.NodeStateGetter(node);
            var label = config.NodeLabelGetter(node);

            var info = new TreeNodeInfo()
            {
                IsLastNode = children.IsNullOrEmpty(),
                State = state
            };

            var boxColor = state.BoxColor;
            if (boxColor == null)
            {
                if (level % 2 != 0)
                {
                    boxColor = Color.black;
                }
            }

            var expand2 = FoldoutGroup(new FoldoutGroupConfig(node, label, state.Expand)
            {
                BoxColor = boxColor,
                Expandable = state.Expandable ?? children.IsNotNullOrEmpty(),
                OnTitleBarGUI = headerRect => config.OnNodeTitleBarGUI?.Invoke(node, headerRect, info),
                OnCoveredTitleBarGUI = headerRect => config.OnNodeConveredTitleBarGUI?.Invoke(node, headerRect, info),
                OnContentGUI = headerRect =>
                {
                    config.OnBeforeChildrenContentGUI?.Invoke(node, headerRect, info);
                    if (!info.IsLastNode)
                    {
                        EditorGUI.indentLevel++;
                        DrawTreeNodes(children, level + 1, config);
                    }

                    config.OnAfterChildrenContentGUI?.Invoke(node, headerRect, info);
                }
            });
            if (expand2 != state.Expand)
            {
                state.Expand = expand2;
                state.OnExpandChanged?.Invoke(expand2);
            }
        }

        private static void ExpandTreeNode<TElement>(TElement node, bool expand, TreeGroupConfig<TElement> config)
        {
            var state = config.NodeStateGetter(node);
            if (state.Expand != expand)
            {
                state.Expand = expand;
                state.OnExpandChanged?.Invoke(expand);
            }

            ExpandTreeNodes(config.NodeChildrenGetter(node), expand, config);
        }

        private static void ExpandTreeNodes<TElement>(IList<TElement> nodes, bool expand,
            TreeGroupConfig<TElement> config)
        {
            if (nodes == null)
                return;
            foreach (var node in nodes)
            {
                ExpandTreeNode(node, expand, config);
            }
        }

        private static void DrawTreeNodes<TElement>(IList<TElement> nodes, int level, TreeGroupConfig<TElement> config)
        {
            if (nodes == null)
                return;
            foreach (var node in nodes)
            {
                DrawTreeNode(node, level, config);
            }
        }

        public static bool TreeGroup<TElement>(IList<TElement> nodes, TreeGroupConfig<TElement> config)
        {
            return WindowLikeToolGroup(new WindowLikeToolGroupConfig(config.Key, config.Label)
            {
                Expand = config.Expand,
                OnMaximize = () => ExpandTreeNodes(nodes, true, config),
                OnMinimize = () => ExpandTreeNodes(nodes, false, config),
                ShowFoldout = nodes != null,
                OnTitleBarGUI = rect => config.OnTitleBarGUI?.Invoke(rect),
                OnContentGUI = () => DrawTreeNodes(nodes, 0, config)
            });
        }

        #endregion

        #region MessageBox

        /// <summary>Draws a message box.</summary>
        /// <param name="message">The message.</param>
        /// <param name="wide">If set to <c>true</c> the message box will be wide.</param>
        public static void MessageBox(string message, bool wide = true)
        {
            MessageBox(message, MessageType.None, wide);
        }

        /// <summary>Draws a message box.</summary>
        /// <param name="message">The message.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="wide">If set to <c>true</c> the message box will be wide.</param>
        public static void MessageBox(string message, MessageType messageType, bool wide = true)
        {
            SirenixEditorGUI.MessageBox(message, messageType, EasyGUIStyles.MessageBox, wide);
        }

        #endregion
    }
}

#endif
