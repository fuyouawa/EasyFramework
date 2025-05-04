using System.Collections.Generic;
using System;
using JetBrains.Annotations;
using UnityEngine;

namespace EasyFramework.Editor
{
    public delegate void CoveredTitleBarGUIHandler(Rect headerRect);

    public delegate void TitleBarGUIHandler(Rect headerRect);

    public delegate void ContentGUIHandler(Rect headerRect);

    public delegate void ConfirmedHandler(object value);
    public delegate string MenuItemNameGetter(object value);
    public delegate void ConfirmationHandler<in T>(T value);
    public delegate string MenuItemNameGetter<in T>(T value);

    public delegate void TitleStyleProcessor(GUIStyle style);
    public delegate void SubTitleStyleProcessor(GUIStyle style);


    public class TitleConfig
    {
        public string Title;
        public string Subtitle;
        public TextAlignment TitleAlignment;
        public bool HorizontalLine;
        public bool BoldTitle;

        public Color? TitleColor;
        public Color? SubtitleColor;
        public float? TitleFontSize;
        public float? SubtitleFontSize;

        public TitleStyleProcessor TitleStyleProcessor;
        public SubTitleStyleProcessor SubtitleStyleProcessor;

        public TitleConfig()
            : this(null)
        {
        }

        public TitleConfig(string title)
        {
            Title = title;
            Subtitle = string.Empty;
            TitleAlignment = TextAlignment.Left;
            HorizontalLine = true;
            BoldTitle = true;
        }
    }

    public class LabelConfig
    {
        public GUIContent Content;
        public Color? Color;
        public GUIStyle Style;

        public LabelConfig()
        {
            Content = GUIContent.none;
        }

        public LabelConfig(GUIContent content, GUIStyle style = null)
        {
            Content = content;
            Style = style;
        }

        public LabelConfig(GUIContent content, Color color, GUIStyle style = null)
        {
            Content = content;
            Style = style;
            Color = color;
        }
    }

    public class PopupSelectorConfig
    {
        public ConfirmedHandler OnConfirmed;
        public MenuItemNameGetter MenuItemNameGetter = null;
        public string Title = null;
        public bool SupportsMultiSelect = false;
        public bool AddThumbnailIcons = true;

        public PopupSelectorConfig()
        {
        }

        public PopupSelectorConfig(ConfirmedHandler onConfirmed, [CanBeNull] MenuItemNameGetter menuItemNameGetter = null)
        {
            OnConfirmed = onConfirmed;
            MenuItemNameGetter = menuItemNameGetter;
        }
    }

    public class SelectorDropdownConfig : PopupSelectorConfig
    {
        public GUIContent Label;
        public GUIContent BtnLabel;
        public bool ReturnValuesOnSelectionChange = true;
        public GUIStyle Style;

        public SelectorDropdownConfig()
        {
        }

        public SelectorDropdownConfig(GUIContent label, GUIContent btnLabel, ConfirmedHandler onConfirmed, [CanBeNull] MenuItemNameGetter menuItemNameGetter = null)
            : base(onConfirmed, menuItemNameGetter)
        {
            Label = label;
            BtnLabel = btnLabel;
        }
    }

    public class FoldoutHeaderConfig
    {
        public GUIContent Label;
        public LabelConfig RightLabelConfig;
        public bool Expand;
        public bool Expandable = true;
        public Color? BoxColor;
        public CoveredTitleBarGUIHandler OnCoveredTitleBarGUI;

        public FoldoutHeaderConfig()
        {
        }

        public FoldoutHeaderConfig(GUIContent label, bool expand)
        {
            Label = label;
            Expand = expand;
        }
    }

    public class FoldoutGroupConfig : FoldoutHeaderConfig
    {
        public object Key;
        public TitleBarGUIHandler OnTitleBarGUI;
        public ContentGUIHandler OnContentGUI;

        public FoldoutGroupConfig()
        {
        }

        public FoldoutGroupConfig(object key, GUIContent label, bool expand,
            ContentGUIHandler onContentGUI)
            : base(label, expand)
        {
            Key = key;
            OnContentGUI = onContentGUI;
        }
    }

    public class BoxGroupConfig
    {
        public GUIContent Label;
        public LabelConfig RightLabel;
        public Color? BoxColor;
        public CoveredTitleBarGUIHandler OnCoveredTitleBarGUI;
        public TitleBarGUIHandler OnTitleBarGUI;
        public ContentGUIHandler OnContentGUI;

        public BoxGroupConfig()
        {
        }

        public BoxGroupConfig(GUIContent label, ContentGUIHandler onContentGUI = null)
        {
            Label = label;
            OnContentGUI = onContentGUI;
        }
    }

    public class FoldoutToolbarConfig
    {
        public GUIContent Label;
        public bool Expand;
        public bool ShowFoldout = true;

        public FoldoutToolbarConfig()
        {
        }

        public FoldoutToolbarConfig(GUIContent label, bool expand)
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

        public WindowLikeToolbarConfig()
        {
        }

        public WindowLikeToolbarConfig(GUIContent label, bool expand) : base(label, expand)
        {
        }
    }

    public class WindowLikeToolGroupConfig : WindowLikeToolbarConfig
    {
        public object Key;
        public Action<Rect> OnTitleBarGUI = null;
        public Action OnContentGUI = null;

        public WindowLikeToolGroupConfig()
        {
        }

        public WindowLikeToolGroupConfig(object key, GUIContent label, bool expand, Action onContentGUI) : base(label,
            expand)
        {
            Key = key;
            OnContentGUI = onContentGUI;
        }
    }

    public class TreeNodeState
    {
        public delegate void ExpandChangedHandler(bool expand);

        public bool Expand = false;
        public bool? Expandable;
        public Color? BoxColor;

        public ExpandChangedHandler OnExpandChanged;
    }

    public struct TreeNodeInfo
    {
        public bool IsLastNode;
        public TreeNodeState State;
    }

    public class TreeGroupConfig<TElement>
    {
        public delegate GUIContent NodeLabelGetterDelegate(TElement node);

        public delegate IList<TElement> NodeChildrenGetterDelegate(TElement node);

        public delegate TreeNodeState NodeStateGetterDelegate(TElement node);

        public delegate void NodeTitleBarGUIHandler(TElement node, Rect headerRect, TreeNodeInfo info);

        public delegate void NodeConveredTitleBarGUIHandler(TElement node, Rect headerRect, TreeNodeInfo info);

        public delegate void BeforeChildrenContentGUIHandler(TElement node, Rect headerRect, TreeNodeInfo info);

        public delegate void AfterChildrenContentGUIHandler(TElement node, Rect headerRect, TreeNodeInfo info);

        public object Key;
        public GUIContent Label;
        public bool Expand;

        public NodeLabelGetterDelegate NodeLabelGetter;
        public NodeChildrenGetterDelegate NodeChildrenGetter;
        public NodeStateGetterDelegate NodeStateGetter;

        public TitleBarGUIHandler OnTitleBarGUI;
        public NodeTitleBarGUIHandler OnNodeTitleBarGUI;
        public NodeConveredTitleBarGUIHandler OnNodeConveredTitleBarGUI;

        public BeforeChildrenContentGUIHandler OnBeforeChildrenContentGUI;
        public AfterChildrenContentGUIHandler OnAfterChildrenContentGUI;

        public TreeGroupConfig()
        {
        }

        public TreeGroupConfig(object key, GUIContent label,
            NodeLabelGetterDelegate nodeLabelGetter,
            NodeChildrenGetterDelegate nodeChildrenGetter,
            NodeStateGetterDelegate nodeStateGetter,
            bool expand)
        {
            Key = key;
            NodeLabelGetter = nodeLabelGetter;
            NodeChildrenGetter = nodeChildrenGetter;
            NodeStateGetter = nodeStateGetter;
            Label = label;
            Expand = expand;
        }
    }
}
