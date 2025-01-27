using System.Collections.Generic;
using System;
using JetBrains.Annotations;
using UnityEngine;

namespace EasyFramework.Editor
{
    public delegate void OnCoveredTitleBarGUIDelegate(Rect headerRect);

    public delegate void OnTitleBarGUIDelegate(Rect headerRect);

    public delegate void OnContentGUIDelegate(Rect headerRect);

    public delegate void OnConfirmedDelegate(object value);
    public delegate string MenuItemNameGetterDelegate(object value);
    public delegate void OnConfirmedDelegate<in T>(T value);
    public delegate string MenuItemNameGetterDelegate<in T>(T value);

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
        public OnConfirmedDelegate OnConfirmed;
        public MenuItemNameGetterDelegate MenuItemNameGetter = null;
        public string Title = null;
        public bool SupportsMultiSelect = false;
        public bool AddThumbnailIcons = true;

        public PopupSelectorConfig()
        {
        }

        public PopupSelectorConfig(OnConfirmedDelegate onConfirmed, [CanBeNull] MenuItemNameGetterDelegate menuItemNameGetter = null)
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

        public SelectorDropdownConfig(GUIContent label, GUIContent btnLabel, OnConfirmedDelegate onConfirmed, [CanBeNull] MenuItemNameGetterDelegate menuItemNameGetter = null)
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
        public OnCoveredTitleBarGUIDelegate OnCoveredTitleBarGUI;

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
        public OnTitleBarGUIDelegate OnTitleBarGUI;
        public OnContentGUIDelegate OnContentGUI;

        public FoldoutGroupConfig()
        {
        }

        public FoldoutGroupConfig(object key, GUIContent label, bool expand,
            OnContentGUIDelegate onContentGUI)
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
        public OnCoveredTitleBarGUIDelegate OnCoveredTitleBarGUI;
        public OnTitleBarGUIDelegate OnTitleBarGUI;
        public OnContentGUIDelegate OnContentGUI;

        public BoxGroupConfig()
        {
        }

        public BoxGroupConfig(GUIContent label, OnContentGUIDelegate onContentGUI = null)
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
        public delegate GUIContent NodeLabelGetterDelegate(TElement node);

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
