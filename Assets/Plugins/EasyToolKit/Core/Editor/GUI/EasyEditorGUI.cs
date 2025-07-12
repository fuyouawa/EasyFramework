// using Sirenix.OdinInspector.Editor;
// using Sirenix.Utilities.Editor;
// using System;
// using System.Collections.Generic;
// using UnityEditor;
// using UnityEngine;
// using System.Linq;
// using JetBrains.Annotations;
// using Object = UnityEngine.Object;
//
// namespace EasyToolKit.Core.Editor
// {
//     public static class EasyEditorGUI
//     {
//         #region Define
//
//         public static readonly GUIContent NoneSelectorBtnLabel = new GUIContent("");
//
//         #endregion
//
//         #region Internal
//
//         private struct ContextInfo
//         {
//             public string Key;
//             public object Context;
//             public Type ContextType;
//         }
//
//         private static readonly Stack<ContextInfo> ContextStack = new Stack<ContextInfo>();
//
//         private static void PushContext(string key, object context, Type contextType)
//         {
//             if (ContextStack.Count > 1024)
//             {
//                 throw new Exception("Stack leak of EasyEditorGUI.Begin - End api!");
//             }
//
//             ContextStack.Push(new ContextInfo()
//             {
//                 Context = context,
//                 Key = key,
//                 ContextType = contextType
//             });
//         }
//
//         private static void PushContext<T>(string key, T context)
//         {
//             PushContext(key, context, typeof(T));
//         }
//
//         private static T PopContext<T>(string key)
//         {
//             return (T)PopContext(key, typeof(T));
//         }
//
//         private static object PopContext(string key, Type contextType)
//         {
//             var ctx = ContextStack.Pop();
//             if (ctx.Key != key)
//             {
//                 throw new ArgumentException($"Context key mismatch, expected \"{key}\", actual \"{ctx.Key}\"");
//             }
//
//             if (ctx.ContextType != contextType)
//             {
//                 throw new ArgumentException(
//                     $"Context type mismatch, expected \"{contextType}\", actual \"{ctx.ContextType}\"");
//             }
//
//             return ctx.Context;
//         }
//
//         #endregion
//
//         #region Extension
//
//         public static bool ToolbarButton(GUIContent content, float width, bool selected = false)
//         {
//             var w = SirenixEditorGUI.currentDrawingToolbarHeight;
//
//             if (GUILayout.Button(content, selected
//                         ? SirenixGUIStyles.ToolbarButtonSelected
//                         : SirenixGUIStyles.ToolbarButton,
//                     GUILayout.Height(w), GUILayout.ExpandWidth(false), GUILayout.Width(width)))
//             {
//                 GUIHelper.RemoveFocusControl();
//                 GUIHelper.RequestRepaint();
//                 return true;
//             }
//
//             return false;
//         }
//
//         public static Rect GetIndentControlRect(params GUILayoutOption[] options)
//         {
//             return GetIndentControlRect(true, 18, options);
//         }
//
//         public static Rect GetIndentControlRect(
//             bool hasLabel,
//             params GUILayoutOption[] options)
//         {
//             return GetIndentControlRect(hasLabel, 18, options);
//         }
//
//         public static Rect GetIndentControlRect(
//             bool hasLabel,
//             float height,
//             params GUILayoutOption[] options)
//         {
//             return GetIndentControlRect(hasLabel, height, EditorStyles.layerMaskField, options);
//         }
//
//         public static Rect GetIndentControlRect(
//             bool hasLabel,
//             float height,
//             GUIStyle style,
//             params GUILayoutOption[] options)
//         {
//             return EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(hasLabel, height, style, options));
//         }
//
//         public static void BeginGUIColor(Color? color)
//         {
//             PushContext("GUIColor", GUI.color);
//             if (color != null)
//             {
//                 GUI.color = (Color)color;
//             }
//         }
//
//         public static void EndGUIColor()
//         {
//             var color = PopContext<Color>("GUIColor");
//             GUI.color = color;
//         }
//
//         public static void BeginGUIContentColor(Color? color)
//         {
//             PushContext("GUIContentColor", GUI.color);
//             if (color != null)
//             {
//                 GUI.contentColor = (Color)color;
//             }
//         }
//
//         public static void EndGUIContentColor()
//         {
//             var color = PopContext<Color>("GUIContentColor");
//             GUI.contentColor = color;
//         }
//
//         #endregion
//
//         #region Title
//
//         private static readonly TitleConfig TempTitleConfig = new TitleConfig();
//
//         public static void Title(
//             string title, string subtitle = null,
//             TextAlignment titleAlignment = TextAlignment.Left,
//             float titleFontSize = 13f,
//             Color? titleColor = null)
//         {
//             TempTitleConfig.Title = title;
//             TempTitleConfig.Subtitle = subtitle;
//             TempTitleConfig.TitleFontSize = titleFontSize;
//             TempTitleConfig.TitleColor = titleColor;
//             TempTitleConfig.TitleAlignment = titleAlignment;
//
//             Title(TempTitleConfig);
//         }
//
//         public static void Title(TitleConfig config)
//         {
//             GUIStyle style1;
//             GUIStyle style2;
//             switch (config.TitleAlignment)
//             {
//                 case TextAlignment.Left:
//                     style1 = config.BoldTitle ? SirenixGUIStyles.BoldTitle : SirenixGUIStyles.Title;
//                     style2 = SirenixGUIStyles.Subtitle;
//                     break;
//                 case TextAlignment.Center:
//                     style1 = config.BoldTitle ? SirenixGUIStyles.BoldTitleCentered : SirenixGUIStyles.TitleCentered;
//                     style2 = SirenixGUIStyles.SubtitleCentered;
//                     break;
//                 case TextAlignment.Right:
//                     style1 = config.BoldTitle ? SirenixGUIStyles.BoldTitleRight : SirenixGUIStyles.TitleRight;
//                     style2 = SirenixGUIStyles.SubtitleRight;
//                     break;
//                 default:
//                     style1 = config.BoldTitle ? SirenixGUIStyles.BoldTitle : SirenixGUIStyles.Title;
//                     style2 = SirenixGUIStyles.SubtitleRight;
//                     break;
//             }
//
//             if (config.TitleFontSize != null)
//                 style1.fontSize = (int)config.TitleFontSize;
//
//             if (config.SubtitleFontSize != null)
//                 style2.fontSize = (int)config.SubtitleFontSize;
//
//             config.TitleStyleProcessor?.Invoke(style1);
//             config.SubtitleStyleProcessor?.Invoke(style2);
//
//             if (config.TitleAlignment > TextAlignment.Right)
//             {
//                 Rect rect = GUILayoutUtility.GetRect(0.0f, 18f, style1, GUILayout.ExpandWidth(true));
//
//                 BeginGUIContentColor(config.TitleColor);
//                 GUI.Label(rect, config.Title, style1);
//                 EndGUIContentColor();
//
//                 rect.y += 3f;
//
//                 BeginGUIContentColor(config.SubtitleColor);
//                 GUI.Label(rect, config.Subtitle, style2);
//                 EndGUIContentColor();
//
//                 if (!config.HorizontalLine)
//                     return;
//                 SirenixEditorGUI.HorizontalLineSeparator(SirenixGUIStyles.LightBorderColor);
//                 GUILayout.Space(3f);
//             }
//             else
//             {
//                 Rect rect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false));
//
//                 BeginGUIContentColor(config.TitleColor);
//                 GUI.Label(rect, config.Title, style1);
//                 EndGUIContentColor();
//
//                 if (config.Subtitle.IsNotNullOrWhiteSpace())
//                 {
//                     rect = EditorGUI.IndentedRect(GUILayoutUtility.GetRect(GUIHelper.TempContent(config.Subtitle),
//                         style2));
//
//                     BeginGUIContentColor(config.SubtitleColor);
//                     GUI.Label(rect, config.Subtitle, style2);
//                     EndGUIContentColor();
//                 }
//
//                 if (!config.HorizontalLine)
//                     return;
//                 SirenixEditorGUI.DrawSolidRect(rect.AlignBottom(1f), SirenixGUIStyles.LightBorderColor);
//                 GUILayout.Space(3f);
//             }
//         }
//
//         #endregion
//
//         #region Selector
//
//         private static readonly SelectorDropdownConfig TempSelectorDropdownConfig = new SelectorDropdownConfig();
//
//         public static IEnumerable<T> DrawSelectorDropdown<T>(
//             Func<IEnumerable<T>> collectionGetter,
//             GUIContent label,
//             GUIContent btnLabel,
//             ConfirmationHandler<T> onConfirmed,
//             [CanBeNull] MenuItemNameGetter<T> menuItemNameGetter = null,
//             params GUILayoutOption[] options)
//         {
//             TempSelectorDropdownConfig.Label = label;
//             TempSelectorDropdownConfig.BtnLabel = btnLabel;
//             TempSelectorDropdownConfig.OnConfirmed = value => onConfirmed((T)value);
//             if (menuItemNameGetter != null)
//             {
//                 TempSelectorDropdownConfig.MenuItemNameGetter = value => menuItemNameGetter((T)value);
//             }
//             else
//             {
//                 TempSelectorDropdownConfig.MenuItemNameGetter = null;
//             }
//
//             return DrawSelectorDropdown(collectionGetter, TempSelectorDropdownConfig, options);
//         }
//
//         public static IEnumerable<T> DrawSelectorDropdown<T>(
//             Func<IEnumerable<T>> collectionGetter,
//             SelectorDropdownConfig config,
//             params GUILayoutOption[] options)
//         {
//             var btnLabel = config.BtnLabel;
//             if (config.AddThumbnailIcons && btnLabel.image == null)
//             {
//                 if (typeof(T) == typeof(Type) || typeof(T).IsSubclassOf(typeof(Object)))
//                 {
//                     btnLabel.image = GUIHelper.GetAssetThumbnail(null, typeof(T), false);
//                 }
//             }
//
//             // 这里必须得单独实例一份PopupSelectorConfig，否则有可能会冲突
//             var copyConfig = new PopupSelectorConfig(config.OnConfirmed, config.MenuItemNameGetter)
//             {
//                 AddThumbnailIcons = config.AddThumbnailIcons,
//                 SupportsMultiSelect = config.SupportsMultiSelect,
//                 Title = config.Title
//             };
//             return OdinSelector<T>.DrawSelectorDropdown(config.Label, config.BtnLabel,
//                 rect => ShowSelectorInPopup(rect, rect.width, collectionGetter(), copyConfig),
//                 config.ReturnValuesOnSelectionChange, config.Style, options);
//         }
//
//         private static OdinSelector<T> GetSelector<T>(
//             IEnumerable<T> collection,
//             PopupSelectorConfig config)
//         {
//             OdinSelector<T> selector;
//
//             if (config.MenuItemNameGetter != null)
//             {
//                 selector = new GenericSelector<T>(config.Title, collection, config.SupportsMultiSelect,
//                     t => config.MenuItemNameGetter(t));
//             }
//             else
//             {
//                 selector = new GenericSelector<T>(config.Title, collection, config.SupportsMultiSelect);
//             }
//
//             selector.SelectionConfirmed += types =>
//             {
//                 var f = types.FirstOrDefault();
//                 if (f != null)
//                 {
//                     config.OnConfirmed?.Invoke(f);
//                 }
//             };
//             if (config.AddThumbnailIcons)
//             {
//                 selector.SelectionTree.EnumerateTree().AddThumbnailIcons();
//             }
//
//             selector.SelectionChanged += types => { selector.SelectionTree.Selection.ConfirmSelection(); };
//             return selector;
//         }
//
//
//         public static OdinSelector<T> ShowSelectorInPopup<T>(
//             IEnumerable<T> collection,
//             PopupSelectorConfig config)
//         {
//             var selector = GetSelector(collection, config);
//             selector.ShowInPopup();
//             return selector;
//         }
//
//         public static OdinSelector<T> ShowSelectorInPopup<T>(Rect rect,
//             IEnumerable<T> collection,
//             PopupSelectorConfig config)
//         {
//             var selector = GetSelector(collection, config);
//             selector.ShowInPopup(rect);
//             return selector;
//         }
//
//         public static OdinSelector<T> ShowSelectorInPopup<T>(Rect btnRect, float windowWidth,
//             IEnumerable<T> collection,
//             PopupSelectorConfig config)
//         {
//             var selector = GetSelector(collection, config);
//             selector.ShowInPopup(btnRect, windowWidth);
//             return selector;
//         }
//
//         public static OdinSelector<T> ShowSelectorInPopup<T>(float windowWidth,
//             IEnumerable<T> collection,
//             PopupSelectorConfig config)
//         {
//             var selector = GetSelector(collection, config);
//             selector.ShowInPopup(windowWidth);
//             return selector;
//         }
//
//         #endregion
//
//         #region Label
//
//         private static readonly LabelConfig s_emptyLabelConfig = new LabelConfig();
//
//         public static Rect PrefixLabel(Rect totalRect, LabelConfig config)
//         {
//             config ??= s_emptyLabelConfig;
//
//             BeginGUIColor(config.Color);
//             var rect = EditorGUI.PrefixLabel(totalRect, config.Content, config.Style);
//             EndGUIColor();
//             return rect;
//         }
//
//         #endregion
//
//         #region Foldout
//
//         public static bool FoldoutHeader(FoldoutHeaderConfig config)
//         {
//             var e = BeginFoldoutHeader(config);
//             EndFoldoutHeader();
//             return e;
//         }
//
//         public static bool FoldoutHeader(FoldoutHeaderConfig config, out Rect headerRect)
//         {
//             var e = BeginFoldoutHeader(config, out headerRect);
//             EndFoldoutHeader();
//             return e;
//         }
//
//         private static readonly FoldoutGroupConfig s_tempFoldoutGroupConfig = new FoldoutGroupConfig();
//
//         public static bool FoldoutGroup(object key, string label, bool expand, ContentGUIHandler onContentGUI)
//         {
//             s_tempFoldoutGroupConfig.Label ??= new GUIContent();
//             s_tempFoldoutGroupConfig.Key = key;
//             s_tempFoldoutGroupConfig.Label.text = label;
//             s_tempFoldoutGroupConfig.Expand = expand;
//             s_tempFoldoutGroupConfig.OnContentGUI = onContentGUI;
//             return FoldoutGroup(s_tempFoldoutGroupConfig);
//         }
//
//         public static bool FoldoutGroup(FoldoutGroupConfig config)
//         {
//             BeginGUIColor(config.BoxColor);
//             SirenixEditorGUI.BeginBox();
//             EndGUIColor();
//
//             config.Expand = BeginFoldoutHeader(config, out var headerRect);
//             config.OnTitleBarGUI?.Invoke(headerRect);
//             EndFoldoutHeader();
//
//             if (config.Expandable)
//             {
//                 if (SirenixEditorGUI.BeginFadeGroup(config.Key, config.Expand))
//                 {
//                     config.OnContentGUI?.Invoke(headerRect);
//                 }
//
//                 SirenixEditorGUI.EndFadeGroup();
//             }
//             else
//             {
//                 config.OnContentGUI?.Invoke(headerRect);
//             }
//
//             SirenixEditorGUI.EndBox();
//
//             return config.Expand;
//         }
//
//         public static bool BeginFoldoutHeader(FoldoutHeaderConfig config)
//         {
//             return BeginFoldoutHeader(config, out _);
//         }
//
//         public static bool BeginFoldoutHeader(FoldoutHeaderConfig config, out Rect headerRect)
//         {
//             // var color = GUI.color;
//             // if (config.BoxColor != null)
//             // {
//             //     GUI.color = (Color)config.BoxColor;
//             // }
//             SirenixEditorGUI.BeginBoxHeader();
//             // GUI.color = color;
//
//             headerRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false));
//
//             if (config.RightLabelConfig != null && config.RightLabelConfig.Content != GUIContent.none)
//             {
//                 var s = SirenixGUIStyles.Label.CalcSize(config.RightLabelConfig.Content);
//                 PrefixLabel(headerRect.AlignRight(s.x), config.RightLabelConfig);
//             }
//
//             config.OnCoveredTitleBarGUI?.Invoke(headerRect);
//             if (config.Expandable)
//             {
//                 config.Expand = SirenixEditorGUI.Foldout(headerRect, config.Expand, config.Label);
//             }
//             else
//             {
//                 var rect = headerRect;
//                 // rect.x += 13;
//                 EditorGUI.LabelField(rect, config.Label, config.Label);
//             }
//
//             return config.Expand;
//         }
//
//         public static void EndFoldoutHeader()
//         {
//             SirenixEditorGUI.EndBoxHeader();
//         }
//
//         #endregion
//
//         #region Box
//
//         public static void BoxGroup(BoxGroupConfig config)
//         {
//             BoxGroup(config, out var rect);
//         }
//
//         public static void BoxGroup(GUIContent label, ContentGUIHandler onContentGUI)
//         {
//             BoxGroup(label, onContentGUI, out _);
//         }
//
//         private static readonly BoxGroupConfig s_tempBoxGroupConfig = new BoxGroupConfig();
//
//         public static void BoxGroup(GUIContent label, ContentGUIHandler onContentGUI, out Rect headerRect)
//         {
//             s_tempBoxGroupConfig.Label = label;
//             s_tempBoxGroupConfig.OnContentGUI = onContentGUI;
//             BoxGroup(s_tempBoxGroupConfig, out headerRect);
//         }
//
//         public static void BoxGroup(BoxGroupConfig config, out Rect headerRect)
//         {
//             var color = GUI.color;
//             if (config.BoxColor != null)
//             {
//                 GUI.color = (Color)config.BoxColor;
//             }
//
//             SirenixEditorGUI.BeginBox();
//             GUI.color = color;
//
//             SirenixEditorGUI.BeginBoxHeader();
//             // GUI.color = color;
//
//             headerRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false));
//
//             if (config.RightLabel != null)
//             {
//                 var s = SirenixGUIStyles.Label.CalcSize(config.RightLabel.Content);
//                 PrefixLabel(headerRect.AlignRight(s.x), config.RightLabel);
//             }
//
//             config.OnCoveredTitleBarGUI?.Invoke(headerRect);
//
//             var rect = headerRect;
//             // rect.x += 13;
//             EditorGUI.LabelField(rect, config.Label);
//
//             config.OnTitleBarGUI?.Invoke(rect);
//
//             SirenixEditorGUI.EndBoxHeader();
//
//             config.OnContentGUI?.Invoke(headerRect);
//
//             SirenixEditorGUI.EndBox();
//         }
//
//         #endregion
//
//         #region WindowLikeToolBar
//
//         private static readonly WindowLikeToolGroupConfig s_tempWindowLikeToolGroupConfig =
//             new WindowLikeToolGroupConfig();
//
//         public static bool WindowLikeToolGroup(object key, GUIContent label, bool expand, Action onContentGUI)
//         {
//             s_tempWindowLikeToolGroupConfig.Key = key;
//             s_tempWindowLikeToolGroupConfig.Label = label;
//             s_tempWindowLikeToolGroupConfig.Expand = expand;
//             s_tempWindowLikeToolGroupConfig.OnContentGUI = onContentGUI;
//             return WindowLikeToolGroup(s_tempWindowLikeToolGroupConfig);
//         }
//
//         public static bool WindowLikeToolGroup(WindowLikeToolGroupConfig config)
//         {
//             using (new EditorGUILayout.VerticalScope())
//             {
//                 config.Expand = BeginWindowLikeToolbar(config, out var headerRect);
//                 config.OnTitleBarGUI?.Invoke(headerRect);
//                 EndWindowLikeToolbar();
//
//                 EditorGUILayout.Space(-2);
//                 SirenixEditorGUI.BeginBox();
//                 if (SirenixEditorGUI.BeginFadeGroup(config.Key, config.Expand))
//                 {
//                     config.OnContentGUI?.Invoke();
//                 }
//
//                 SirenixEditorGUI.EndFadeGroup();
//                 SirenixEditorGUI.EndBox();
//             }
//
//             return config.Expand;
//         }
//
//         public static bool WindowLikeToolbar(WindowLikeToolbarConfig config)
//         {
//             var e = BeginWindowLikeToolbar(config);
//             EndWindowLikeToolbar();
//             return e;
//         }
//
//         public static bool BeginWindowLikeToolbar(WindowLikeToolbarConfig config)
//         {
//             return BeginWindowLikeToolbar(config, out _);
//         }
//
//         public static bool BeginWindowLikeToolbar(WindowLikeToolbarConfig config, out Rect headerRect)
//         {
//             var expand = BeginFoldoutToolbar(config, out headerRect);
//
//             if (ToolbarButton(
//                     new GUIContent(EasyEditorIcons.Expand, tooltip: config.ExpandButtonTooltip),
//                     SirenixEditorGUI.currentDrawingToolbarHeight))
//             {
//                 config.OnMaximize?.Invoke();
//             }
//
//             if (ToolbarButton(
//                     new GUIContent(EasyEditorIcons.Collapse, tooltip: config.CollapseButtonTooltip),
//                     SirenixEditorGUI.currentDrawingToolbarHeight))
//             {
//                 config.OnMinimize?.Invoke();
//             }
//
//             return expand;
//         }
//
//         public static void EndWindowLikeToolbar()
//         {
//             EndFoldoutToolbar();
//         }
//
//         #endregion
//
//         #region FoldoutToolBar
//
//         public static bool FoldoutToolbar(FoldoutToolbarConfig config)
//         {
//             var e = BeginFoldoutToolbar(config);
//             EndFoldoutToolbar();
//             return e;
//         }
//
//         public static bool BeginFoldoutToolbar(FoldoutToolbarConfig config)
//         {
//             return BeginFoldoutToolbar(config, out _);
//         }
//
//         public static bool BeginFoldoutToolbar(FoldoutToolbarConfig config, out Rect headerRect)
//         {
//             SirenixEditorGUI.BeginHorizontalToolbar();
//
//             if (!config.ShowFoldout)
//             {
//                 GUILayout.Label(config.Label, GUILayout.ExpandWidth(false));
//                 headerRect = GUILayoutUtility.GetLastRect();
//             }
//             else
//             {
//                 float tmp = EditorGUIUtility.fieldWidth;
//                 EditorGUIUtility.fieldWidth = 10f;
//                 headerRect = EditorGUILayout.GetControlRect(false);
//                 EditorGUIUtility.fieldWidth = tmp;
//             }
//
//             GUILayout.FlexibleSpace();
//
//             config.Expand = !config.ShowFoldout || SirenixEditorGUI.Foldout(headerRect, config.Expand, config.Label);
//
//             return config.Expand;
//         }
//
//         public static void EndFoldoutToolbar()
//         {
//             SirenixEditorGUI.EndHorizontalToolbar();
//         }
//
//         #endregion
//
//         #region TreeGroup
//
//         private static void DrawTreeNode<TElement>(TElement node, int level, TreeGroupConfig<TElement> config)
//         {
//             var children = config.NodeChildrenGetter(node);
//
//             var state = config.NodeStateGetter(node);
//             var label = config.NodeLabelGetter(node);
//
//             var info = new TreeNodeInfo()
//             {
//                 IsLastNode = children.IsNullOrEmpty(),
//                 State = state
//             };
//
//             var boxColor = state.BoxColor;
//             if (boxColor == null)
//             {
//                 if (level % 2 != 0)
//                 {
//                     boxColor = Color.black;
//                 }
//             }
//
//             var expand2 = FoldoutGroup(new FoldoutGroupConfig(node, label, state.Expand, OnContentGUI)
//             {
//                 BoxColor = boxColor,
//                 Expandable = state.Expandable ?? children.IsNotNullOrEmpty(),
//                 OnTitleBarGUI = headerRect => config.OnNodeTitleBarGUI?.Invoke(node, headerRect, info),
//                 OnCoveredTitleBarGUI = headerRect => config.OnNodeConveredTitleBarGUI?.Invoke(node, headerRect, info)
//             });
//             if (expand2 != state.Expand)
//             {
//                 state.Expand = expand2;
//                 state.OnExpandChanged?.Invoke(expand2);
//             }
//
//             void OnContentGUI(Rect headerRect)
//             {
//                 config.OnBeforeChildrenContentGUI?.Invoke(node, headerRect, info);
//                 if (!info.IsLastNode)
//                 {
//                     EditorGUI.indentLevel++;
//                     DrawTreeNodes(children, level + 1, config);
//                 }
//
//                 config.OnAfterChildrenContentGUI?.Invoke(node, headerRect, info);
//             }
//         }
//
//         private static void ExpandTreeNode<TElement>(TElement node, bool expand, TreeGroupConfig<TElement> config)
//         {
//             var state = config.NodeStateGetter(node);
//             if (state.Expand != expand)
//             {
//                 state.Expand = expand;
//                 state.OnExpandChanged?.Invoke(expand);
//             }
//
//             ExpandTreeNodes(config.NodeChildrenGetter(node), expand, config);
//         }
//
//         private static void ExpandTreeNodes<TElement>(IList<TElement> nodes, bool expand,
//             TreeGroupConfig<TElement> config)
//         {
//             if (nodes == null)
//                 return;
//             foreach (var node in nodes)
//             {
//                 ExpandTreeNode(node, expand, config);
//             }
//         }
//
//         private static void DrawTreeNodes<TElement>(IList<TElement> nodes, int level, TreeGroupConfig<TElement> config)
//         {
//             if (nodes == null)
//                 return;
//             foreach (var node in nodes)
//             {
//                 DrawTreeNode(node, level, config);
//             }
//         }
//
//         private static readonly WindowLikeToolGroupConfig s_tempWindowLikeToolGroupConfig2 =
//             new WindowLikeToolGroupConfig();
//
//         public static bool TreeGroup<TElement>(IList<TElement> nodes, TreeGroupConfig<TElement> config)
//         {
//             s_tempWindowLikeToolGroupConfig2.Key = config.Key;
//             s_tempWindowLikeToolGroupConfig2.Label = config.Label;
//             s_tempWindowLikeToolGroupConfig2.Expand = config.Expand;
//             s_tempWindowLikeToolGroupConfig2.OnMaximize = () => ExpandTreeNodes(nodes, true, config);
//             s_tempWindowLikeToolGroupConfig2.OnMinimize = () => ExpandTreeNodes(nodes, false, config);
//             s_tempWindowLikeToolGroupConfig2.ShowFoldout = nodes != null;
//             s_tempWindowLikeToolGroupConfig2.OnTitleBarGUI = rect => config.OnTitleBarGUI?.Invoke(rect);
//             s_tempWindowLikeToolGroupConfig2.OnContentGUI = () => DrawTreeNodes(nodes, 0, config);
//
//             return WindowLikeToolGroup(s_tempWindowLikeToolGroupConfig2);
//         }
//
//         #endregion
//
//         #region MessageBox
//
//         /// <summary>Draws a message box.</summary>
//         /// <param name="message">The message.</param>
//         /// <param name="wide">If set to <c>true</c> the message box will be wide.</param>
//         public static void MessageBox(string message, bool wide = true)
//         {
//             MessageBox(message, MessageType.None, wide);
//         }
//
//         /// <summary>Draws a message box.</summary>
//         /// <param name="message">The message.</param>
//         /// <param name="messageType">Type of the message.</param>
//         /// <param name="wide">If set to <c>true</c> the message box will be wide.</param>
//         public static void MessageBox(string message, MessageType messageType, bool wide = true)
//         {
//             SirenixEditorGUI.MessageBox(message, messageType, EasyGUIStyles.MessageBox, wide);
//         }
//
//         #endregion
//     }
// }
