using System.Collections.Generic;
using System;
using UnityEngine;

namespace EasyFramework.Inspector
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
        public bool AddThumbnailIcons = true;

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

        public LabelConfig(string content, GUIStyle style = null)
        {
            Content = new GUIContent(content);
            Style = style;
        }

        public LabelConfig(string content, Color color, GUIStyle style = null)
        {
            Content = new GUIContent(content);
            Style = style;
            Color = color;
        }
    }

    public class FoldoutHeaderConfig
    {
        public GUIContent Label;
        public bool Expand;
        public bool Expandable = true;
        public Color? BoxColor;
        public OnCoveredTitleBarGUIDelegate OnCoveredTitleBarGUI;
        public LabelConfig RightLabelConfig = new LabelConfig();

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
}
