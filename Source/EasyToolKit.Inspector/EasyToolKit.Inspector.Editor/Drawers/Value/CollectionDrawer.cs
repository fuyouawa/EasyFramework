using System;
using System.Collections.Generic;
using System.Reflection;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Value + 10)]
    public class CollectionDrawer<T> : EasyValueDrawer<T>
    {
        private static readonly GUIContent TempContent = new GUIContent();
        
        public GUIStyle ListItemStyle = new GUIStyle(GUIStyle.none)
        {
            padding = new RectOffset(25, 20, 3, 3)
        };

        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            return property.ChildrenResolver is InspectorCollectionResolver<T>;
        }

        private InspectorCollectionResolver<T> _collectionResolver;

        protected override void Initialize()
        {
            _collectionResolver = (InspectorCollectionResolver<T>)Property.ChildrenResolver;
        }

        protected override void OnDrawProperty(GUIContent label)
        {
            DrawHeader(label);
            DrawItems();
        }

        private void DrawHeader(GUIContent label)
        {
            EasyEditorGUI.BeginHorizontalToolbar();

            GUILayout.Label(label);

            GUILayout.FlexibleSpace();

            if (EasyEditorGUI.ToolbarButton(TempContent.SetImage(EasyEditorIcons.Plus)))
            {
                
            }
            
            EasyEditorGUI.EndHorizontalToolbar();
        }

        private void DrawItems()
        {
            EasyEditorGUI.BeginVerticalList();

            for (int i = 0; i < Property.Children.Count; i++)
            {
                var child = Property.Children[i];
                DrawItem(child, i);
            }

            EasyEditorGUI.EndVerticalList();
        }

        private void DrawItem(InspectorProperty property, int index)
        {
            var rect = EasyEditorGUI.BeginListItem(false, ListItemStyle, GUILayout.MinHeight(25), GUILayout.ExpandWidth(true));

            Rect dragHandleRect;
            Rect removeBtnRect;
            if (Event.current.type == EventType.Repaint)
            {
                dragHandleRect = new Rect(rect.x + 4, rect.y + 2 + ((int)rect.height - 23) / 2, 20, 20);
                removeBtnRect = new Rect(dragHandleRect.x + rect.width - 22, dragHandleRect.y + 1, 14, 14);
                
                GUI.Label(dragHandleRect, EasyEditorIcons.List, GUIStyle.none);
            }

            property.Draw(null);

            EasyEditorGUI.EndListItem();
        }
    }
}
