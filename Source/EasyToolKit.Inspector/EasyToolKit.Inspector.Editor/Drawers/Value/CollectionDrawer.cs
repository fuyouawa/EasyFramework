using System;
using System.Collections.Generic;
using System.Reflection;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.ThirdParty.OdinSerializer;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Value + 10)]
    public class CollectionDrawer<T> : EasyValueDrawer<T>
    {
        private static readonly GUIContent TempContent = new GUIContent();
        
        private static GUIStyle s_awesomeHeaderLabelStyle;
        public static GUIStyle AwesomeHeaderLabelStyle
        {
            get
            {
                if (s_awesomeHeaderLabelStyle == null)
                {
                    s_awesomeHeaderLabelStyle = new GUIStyle(GUI.skin.label)
                    {
                        fontSize = EasyGUIStyles.Foldout.fontSize + 1,
                        alignment = TextAnchor.MiddleLeft,
                    };
                }
                return s_awesomeHeaderLabelStyle;
            }
        }

        public static Color AwesomeHeaderBackgroundColor = new Color(0.8f, 0.8f, 0.8f);
        public static Color AwesomeItemBackgroundColor = new Color(0.9f, 0.9f, 0.9f);

        public static GUIStyle ListItemStyle = new GUIStyle(GUIStyle.none)
        {
            padding = new RectOffset(25, 20, 3, 3)
        };

        public static GUIStyle AwesomeListItemStyle = new GUIStyle(GUIStyle.none)
        {
            padding = new RectOffset(30, 37, 3, 3)
        };

        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            return property.ChildrenResolver is ICollectionResolver;
        }

        private ICollectionResolver _collectionResolver;
        private IOrderedCollectionResolver _orderedCollectionResolver;
        private ListDrawerSettingsAttribute _listDrawerSettings;

        [CanBeNull] private ICodeValueResolver<Texture> _iconTextureGetterResolver;

        protected override void Initialize()
        {
            _collectionResolver = (ICollectionResolver)Property.ChildrenResolver;
            _orderedCollectionResolver = Property.ChildrenResolver as IOrderedCollectionResolver;

            _listDrawerSettings = Property.GetAttribute<AwesomeListDrawerSettingsAttribute>();
            if (_listDrawerSettings == null)
            {
                _listDrawerSettings = Property.GetAttribute<ListDrawerSettingsAttribute>();
            }

            var targetType = Property.Parent.ValueEntry.ValueType;

            if (_listDrawerSettings is AwesomeListDrawerSettingsAttribute awesomeListDrawerSettings)
            {
                if (awesomeListDrawerSettings.IconTextureGetter.IsNotNullOrEmpty())
                {
                    _iconTextureGetterResolver = CodeValueResolver.Create<Texture>(awesomeListDrawerSettings.IconTextureGetter, targetType);
                }
            }
        }

        protected override void DrawProperty(GUIContent label)
        {
            if (_iconTextureGetterResolver != null && _iconTextureGetterResolver.HasError(out var error))
            {
                EditorGUILayout.HelpBox(error, MessageType.Error);
                return;
            }


            EasyEditorGUI.BeginIndentedVertical(EasyGUIStyles.PropertyPadding);

            if (_listDrawerSettings is AwesomeListDrawerSettingsAttribute)
            {
                DrawAwesomeHeader(label);
                DrawAwesomeItems();
            }
            else
            {
                DrawHeader(label);
                DrawItems();
            }
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

        private void DrawAwesomeHeader(GUIContent label)
        {
            EasyGUIHelper.PushColor(AwesomeHeaderBackgroundColor);
            EasyEditorGUI.BeginHorizontalToolbar(30);
            EasyGUIHelper.PopColor();

            if (_iconTextureGetterResolver != null)
            {
                var iconTexture = _iconTextureGetterResolver.Resolve(Property.Parent.ValueEntry.WeakSmartValue);
                GUILayout.Label(iconTexture, GUILayout.Width(30), GUILayout.Height(30));
            }

            GUILayout.Label(label, AwesomeHeaderLabelStyle, GUILayout.Height(30));

            GUILayout.FlexibleSpace();

            var btnRect = GUILayoutUtility.GetRect(
                EasyEditorIcons.Plus.HighlightedContent,
                "Button",
                GUILayout.ExpandWidth(false),
                GUILayout.Width(30),
                GUILayout.Height(30));
            
            if (GUI.Button(btnRect, GUIContent.none, "Button"))
            {
                EasyGUIHelper.RemoveFocusControl();
                _collectionResolver.QueueInsertElement(GetValueToAdd());
            }
            
            if (Event.current.type == EventType.Repaint)
            {
                EasyEditorIcons.Plus.Draw(btnRect.AlignCenter(25, 25));
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

        private void DrawAwesomeItems()
        {
            EasyEditorGUI.BeginVerticalList();

            for (int i = 0; i < Property.Children.Count; i++)
            {
                var child = Property.Children[i];
                DrawAwesomeItem(child, i);
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

        private void DrawAwesomeItem(InspectorProperty property, int index)
        {
            EasyGUIHelper.PushColor(AwesomeItemBackgroundColor);
            var rect = EasyEditorGUI.BeginListItem(false, AwesomeListItemStyle, GUILayout.MinHeight(25), GUILayout.ExpandWidth(true));
            EasyGUIHelper.PopColor();

            var dragHandleRect = new Rect(rect.x + 4, rect.y + 2 + ((int)rect.height - 23) / 2, 23, 23);
            var removeBtnRect = new Rect(dragHandleRect.x + rect.width - 37, dragHandleRect.y - 5, 30, 30);
                
            GUI.Label(dragHandleRect, EasyEditorIcons.List.InactiveTexture, GUIStyle.none);

            property.Draw(null);

            if (GUI.Button(removeBtnRect, GUIContent.none, "Button"))
            {
                EasyGUIHelper.RemoveFocusControl();

                if (_orderedCollectionResolver != null)
                {
                    _orderedCollectionResolver.QueueRemoveElementAt(index);
                }
                else
                {
                    //TODO 非顺序容器的元素删除
                }
            }
            
            if (Event.current.type == EventType.Repaint)
            {
                EasyEditorIcons.X.Draw(removeBtnRect.AlignCenter(25, 25));
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
