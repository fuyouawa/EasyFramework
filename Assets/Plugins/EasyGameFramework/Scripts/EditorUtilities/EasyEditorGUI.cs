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
using static Sirenix.OdinInspector.SelfValidationResult;

namespace EasyGameFramework
{
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

        public static IEnumerable<T> DrawSelectorDropdown<T>(SelectorDropdownConfig<T> config,
            params GUILayoutOption[] options)
        {
            return OdinSelector<T>.DrawSelectorDropdown(config.Label, config.BtnLabel,
                rect => ShowSelectorInPopup(rect, rect.width, config),
                config.ReturnValuesOnSelectionChange, config.Style, options);
        }

        private static OdinSelector<T> GetSelector<T>(PopupSelectorConfig<T> config)
        {
            GenericSelector<T> selector;
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

        public class FoldoutHeaderConfig
        {
            public GUIContent Label;
            public bool Expand;
            public Func<Rect> HeaderRectGetter = null;
            public GUIContent RightLabel = GUIContent.none;
            public bool HasBox = true;

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
            public delegate void OnTitleBarGUIDelegate(Rect headerRect);

            public object Key;
            public OnTitleBarGUIDelegate OnTitleBarGUI;
            public Action OnContentGUI;

            public FoldoutGroupConfig(object key, string label, bool expand = true) : base(label, expand)
            {
                Key = key;
            }

            public FoldoutGroupConfig(object key, GUIContent label, bool expand = true) : base(label, expand)
            {
                Key = key;
            }
        }

        public static bool FoldoutHeader(FoldoutHeaderConfig config)
        {
            var e = BeginFoldoutHeader(config);
            EndFoldoutHeader();
            return e;
        }

        struct FoldoutHeaderContext
        {
            public bool HasBox;
        }

        public static bool FoldoutGroup(FoldoutGroupConfig config)
        {
            if (config.HasBox)
            {
                SirenixEditorGUI.BeginBox();
            }

            config.Expand = BeginFoldoutHeader(config, out var headerRect);
            config.OnTitleBarGUI?.Invoke(headerRect);
            EndFoldoutHeader();

            if (SirenixEditorGUI.BeginFadeGroup(config.Key, config.Expand))
            {
                config.OnContentGUI?.Invoke();
            }

            SirenixEditorGUI.EndFadeGroup();

            if (config.HasBox)
            {
                SirenixEditorGUI.EndBox();
            }

            return config.Expand;
        }

        public static bool BeginFoldoutHeader(FoldoutHeaderConfig config)
        {
            return BeginFoldoutHeader(config, out _);
        }

        public static bool BeginFoldoutHeader(FoldoutHeaderConfig config, out Rect headerRect)
        {
            PushContext(new FoldoutHeaderContext() { HasBox = config.HasBox });
            if (config.HasBox)
            {
                SirenixEditorGUI.BeginBoxHeader();
            }

            headerRect = config.HeaderRectGetter == null
                ? EditorGUILayout.GetControlRect(false)
                : config.HeaderRectGetter();

            if (config.RightLabel != null && config.RightLabel != GUIContent.none)
            {
                var s = SirenixGUIStyles.Label.CalcSize(config.RightLabel);
                EditorGUI.PrefixLabel(headerRect.AlignRight(s.x), config.RightLabel);
            }

            config.Expand = SirenixEditorGUI.Foldout(headerRect, config.Expand, config.Label);

            return config.Expand;
        }

        public static void EndFoldoutHeader()
        {
            var ctx = PopContext<FoldoutHeaderContext>();
            if (ctx.HasBox)
            {
                SirenixEditorGUI.EndBoxHeader();
            }
        }

        #endregion

        #region WindowLikeToolBar

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

        public class TreeGroupConfig<TElement>
        {
            public delegate void OnTitleBarGUIDelegate(Rect headerRect);

            public delegate string NodeLabelGetterDelegate(TElement node);

            public delegate IList<TElement> NodeChildrenGetterDelegate(TElement node);

            public delegate bool NodeExpandStateGetterDelegate(TElement node);

            public delegate void OnNodeExpandStateChangedDelegate(TElement node, bool expand);

            public delegate void OnChildrenTitleBarGUIDelegate(TElement node, float indent, Rect headerRect);

            public delegate void OnBeforeChildrenDrawDelegate(TElement node, float indent);

            public delegate void OnAfterChildrenDrawDelegate(TElement node, float indent, Rect headerRect);

            public object Key;
            public GUIContent Label;
            public bool Expand;
            public int Indent = 10;

            public NodeLabelGetterDelegate NodeLabelGetter;
            public NodeChildrenGetterDelegate NodeChildrenGetter;
            public NodeExpandStateGetterDelegate NodeExpandStateGetter;
            public OnNodeExpandStateChangedDelegate OnNodeExpandStateChanged;

            public OnTitleBarGUIDelegate OnTitleBarGUI;
            public OnChildrenTitleBarGUIDelegate OnChildrenTitleBarGui;
            public OnBeforeChildrenDrawDelegate OnBeforeChildrenDraw;
            public OnAfterChildrenDrawDelegate OnAfterChildrenDraw;

            public TreeGroupConfig(object key, string label,
                NodeLabelGetterDelegate nodeLabelGetter,
                NodeChildrenGetterDelegate nodeChildrenGetter,
                NodeExpandStateGetterDelegate nodeExpandStateGetter,
                OnNodeExpandStateChangedDelegate onNodeExpandStateChanged,
                bool expand = true) : this(key, new GUIContent(label), nodeLabelGetter, nodeChildrenGetter,
                nodeExpandStateGetter, onNodeExpandStateChanged, expand)
            {
            }

            public TreeGroupConfig(object key, GUIContent label,
                NodeLabelGetterDelegate nodeLabelGetter,
                NodeChildrenGetterDelegate nodeChildrenGetter,
                NodeExpandStateGetterDelegate nodeExpandStateGetter,
                OnNodeExpandStateChangedDelegate onNodeExpandStateChanged,
                bool expand = true)
            {
                Key = key;
                NodeLabelGetter = nodeLabelGetter;
                NodeChildrenGetter = nodeChildrenGetter;
                NodeExpandStateGetter = nodeExpandStateGetter;
                OnNodeExpandStateChanged = onNodeExpandStateChanged;
                Label = label;
                Expand = expand;
            }
        }

        private static void DrawTreeNode<TElement>(TElement node, int hierarchy, TreeGroupConfig<TElement> config)
        {
            var off = hierarchy * config.Indent;

            config.OnBeforeChildrenDraw?.Invoke(node, off);

            var children = config.NodeChildrenGetter(node);
            if (!children.IsNullOrEmpty())
            {
                var rect = EditorGUILayout.GetControlRect();
                rect.x += off;
                rect.width -= off;

                var expand = config.NodeExpandStateGetter(node);
                var label = config.NodeLabelGetter(node);
                var expand2 = FoldoutGroup(new FoldoutGroupConfig(node, label, expand)
                {
                    HasBox = false,
                    HeaderRectGetter = () => rect,
                    OnTitleBarGUI = headerRect => config.OnChildrenTitleBarGui?.Invoke(node, off, headerRect),
                    OnContentGUI = () => { DrawTreeNodes(children, hierarchy + 1, config); }
                });
                if (expand2 != expand)
                {
                    config.OnNodeExpandStateChanged(node, expand2);
                }

                config.OnAfterChildrenDraw?.Invoke(node, off, rect);
            }
        }

        private static void ExpandTreeNode<TElement>(TElement node, bool expand, TreeGroupConfig<TElement> config)
        {
            var expand2 = config.NodeExpandStateGetter(node);
            if (expand2 != expand)
            {
                config.OnNodeExpandStateChanged(node, expand);
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

        private static void DrawTreeNodes<TElement>(IList<TElement> nodes, int hierarchy,
            TreeGroupConfig<TElement> config)
        {
            if (nodes == null)
                return;
            foreach (var node in nodes)
            {
                DrawTreeNode(node, hierarchy, config);
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
    }
}

#endif
