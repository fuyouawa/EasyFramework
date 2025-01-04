using System;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
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
                    GetNodeExpandState,
                    OnNodeExpandStateChanged,
                    Property.State.Expanded)
                {
                    OnTitleBarGUI = OnTitleBarGUI,
                    OnChildrenTitleBarGui = OnChildrenTitleBarGUI,
                    OnBeforeChildrenDraw = OnBeforeChildrenDraw,
                    OnAfterChildrenDraw = OnAfterChildrenDraw
                }
            );
        }

        public abstract string GetNodeLabel(TElement node);
        public abstract IList<TElement> GetNodeChildren(TElement node);
        public abstract bool GetNodeExpandState(TElement node);
        public abstract void OnNodeExpandStateChanged(TElement node, bool expand);

        protected virtual void OnTitleBarGUI(Rect headerRect)
        {
        }

        protected virtual void OnChildrenTitleBarGUI(TElement node, float indent, Rect headerRect)
        {
        }

        protected virtual void OnBeforeChildrenDraw(TElement node, float indent)
        {
        }

        protected virtual void OnAfterChildrenDraw(TElement node, float indent, Rect headerRect)
        {
        }
    }
}
