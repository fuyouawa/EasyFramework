using System;
using System.Collections.Generic;
using System.Reflection;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.ThirdParty.OdinSerializer;
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
            return property.ChildrenResolver is ICollectionResolver;
        }

        private ICollectionResolver _collectionResolver;
        private IOrderedCollectionResolver _orderedCollectionResolver;

        protected override void Initialize()
        {
            _collectionResolver = (ICollectionResolver)Property.ChildrenResolver;
            _orderedCollectionResolver = Property.ChildrenResolver as IOrderedCollectionResolver;
        }

        protected override void DrawProperty(GUIContent label)
        {
            EasyEditorGUI.BeginIndentedVertical(EasyGUIStyles.PropertyPadding);
            DrawHeader(label);
            DrawItems();
            EasyEditorGUI.EndIndentedVertical();
        }

        private void DrawHeader(GUIContent label)
        {
            EasyEditorGUI.BeginHorizontalToolbar();

            GUILayout.Label(label);

            GUILayout.FlexibleSpace();

            if (EasyEditorGUI.ToolbarButton(EasyEditorIcons.Plus))
            {
                _collectionResolver.QueueInsertElement(GetValueToAdd());
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

            var dragHandleRect = new Rect(rect.x + 4, rect.y + 2 + ((int)rect.height - 23) / 2, 20, 20);
            var removeBtnRect = new Rect(dragHandleRect.x + rect.width - 22, dragHandleRect.y + 1, 14, 14);
                
            GUI.Label(dragHandleRect, EasyEditorIcons.List.InactiveTexture, GUIStyle.none);

            property.Draw(null);

            if (EasyEditorGUI.IconButton(removeBtnRect, EasyEditorIcons.X))
            {
                if (_orderedCollectionResolver != null)
                {
                    _orderedCollectionResolver.QueueRemoveElementAt(index);
                }
                else
                {
                    //TODO 非顺序容器的元素删除
                }
            }

            EasyEditorGUI.EndListItem();
        }

        private object GetValueToAdd()
        {
            if (_collectionResolver.ElementType.IsInheritsFrom<UnityEngine.Object>())
            {
                return null;
            }
        
            return UnitySerializationUtility.CreateDefaultUnityInitializedObject(_collectionResolver.ElementType);
        }
    }
}
