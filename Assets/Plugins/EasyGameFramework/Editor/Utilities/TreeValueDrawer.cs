using System;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyGameFramework.Editor
{
    public abstract class TreeValueDrawer<T, TElement> : OdinValueDrawer<T>
        where T : IList<TElement>
    {
        protected virtual GUIContent GetLabel(GUIContent label)
        {
            return label;
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var val = (IList<TElement>)ValueEntry.SmartValue;

            Property.State.Expanded = EasyEditorGUI.TreeGroup(val,
                new EasyEditorGUI.TreeGroupConfig<TElement>(
                    UniqueDrawerKey.Create(Property, this),
                    GetLabel(label),
                    GetNodeLabel,
                    GetNodeChildren,
                    GetNodeState,
                    Property.State.Expanded)
                {
                    OnNodeTitleBarGUI = OnNodeTitleBarGUI,
                    OnNodeConveredTitleBarGUI = OnNodeCoveredTitleBarGUI,
                    OnTitleBarGUI = OnTitleBarGUI,
                    OnBeforeChildrenContentGUI = OnBeforeChildrenContentGUI,
                    OnAfterChildrenContentGUI = OnAfterChildrenContentGUI
                }
            );
        }

        public abstract string GetNodeLabel(TElement node);
        public abstract IList<TElement> GetNodeChildren(TElement node);
        public abstract EasyEditorGUI.TreeNodeState GetNodeState(TElement node);

        protected virtual void OnTitleBarGUI(Rect headerRect)
        {
        }
        
        protected virtual void OnNodeCoveredTitleBarGUI(TElement node, Rect headerRect, EasyEditorGUI.TreeNodeInfo info)
        {
        }

        protected virtual void OnNodeTitleBarGUI(TElement node, Rect headerRect, EasyEditorGUI.TreeNodeInfo info)
        {
        }

        protected virtual void OnBeforeChildrenContentGUI(TElement node, Rect headerRect, EasyEditorGUI.TreeNodeInfo info)
        {
        }

        protected virtual void OnAfterChildrenContentGUI(TElement node, Rect headerRect, EasyEditorGUI.TreeNodeInfo info)
        {
        }
    }
}
