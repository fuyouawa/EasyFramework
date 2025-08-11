using System;
using System.Collections.Generic;
using System.Reflection;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.ThirdParty.OdinSerializer;
using EasyToolKit.ThirdParty.OdinSerializer.Utilities;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Inspector.Editor
{
    [DrawerPriority(DrawerPriorityLevel.Value + 9)]
    public class CollectionDrawer<T> : EasyValueDrawer<T>
    {
        private static readonly GUIContent TempContent = new GUIContent();

        private static GUIStyle s_metroHeaderLabelStyle;
        public static GUIStyle MetroHeaderLabelStyle
        {
            get
            {
                if (s_metroHeaderLabelStyle == null)
                {
                    s_metroHeaderLabelStyle = new GUIStyle(GUI.skin.label)
                    {
                        fontSize = EasyGUIStyles.Foldout.fontSize + 1,
                        alignment = TextAnchor.MiddleLeft,
                    };
                }
                return s_metroHeaderLabelStyle;
            }
        }

        public static Color MetroHeaderBackgroundColor = new Color(0.8f, 0.8f, 0.8f);
        public static Color MetroItemBackgroundColor = new Color(0.9f, 0.9f, 0.9f);

        public static GUIStyle ListItemStyle = new GUIStyle(GUIStyle.none)
        {
            padding = new RectOffset(25, 20, 3, 3)
        };

        public static GUIStyle MetroListItemStyle = new GUIStyle(GUIStyle.none)
        {
            padding = new RectOffset(30, 37, 3, 3)
        };

        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            return property.ChildrenResolver is ICollectionResolver;
        }

        private ICollectionResolver _collectionResolver;
        [CanBeNull] private IOrderedCollectionResolver _orderedCollectionResolver;
        [CanBeNull] private ListDrawerSettingsAttribute _listDrawerSettings;

        [CanBeNull] private ICodeValueResolver<Texture> _iconTextureGetterResolver;

        [CanBeNull] private Action<object, object> _onAddedElementCallback;
        [CanBeNull] private Action<object, object> _onRemovedElementCallback;

        [CanBeNull] private Func<object, object> _customCreateElementFunction;
        [CanBeNull] private Action<object, object> _customRemoveElementFunction;
        [CanBeNull] private Action<object, int> _customRemoveIndexFunction;

        private string _error;

        protected override void Initialize()
        {
            _collectionResolver = (ICollectionResolver)Property.ChildrenResolver;
            _orderedCollectionResolver = Property.ChildrenResolver as IOrderedCollectionResolver;

            _listDrawerSettings = Property.GetAttribute<MetroListDrawerSettingsAttribute>();
            if (_listDrawerSettings == null)
            {
                _listDrawerSettings = Property.GetAttribute<ListDrawerSettingsAttribute>();
            }

            var targetType = Property.Parent.ValueEntry.ValueType;

            if (_listDrawerSettings != null)
            {
                if (_listDrawerSettings is MetroListDrawerSettingsAttribute metroListDrawerSettings)
                {
                    if (metroListDrawerSettings.IconTextureGetter.IsNotNullOrEmpty())
                    {
                        _iconTextureGetterResolver = CodeValueResolver.Create<Texture>(metroListDrawerSettings.IconTextureGetter, targetType);
                    }
                }

                try
                {
                    if (_listDrawerSettings.OnAddedElementCallback.IsNotNullOrEmpty())
                    {
                        var onAddedElementMethod = targetType.GetMethodEx(_listDrawerSettings.OnAddedElementCallback, BindingFlagsHelper.All, typeof(object))
                            ?? throw new Exception($"Cannot find method '{_listDrawerSettings.OnAddedElementCallback}' in '{targetType}'");

                        _onAddedElementCallback = (instance, value) =>
                        {
                            onAddedElementMethod.Invoke(instance, new object[] { value });
                        };
                    }

                    if (_listDrawerSettings.OnRemovedElementCallback.IsNotNullOrEmpty())
                    {
                        var onRemovedElementMethod = targetType.GetMethodEx(_listDrawerSettings.OnRemovedElementCallback, BindingFlagsHelper.All, typeof(object))
                            ?? throw new Exception($"Cannot find method '{_listDrawerSettings.OnRemovedElementCallback}' in '{targetType}'");

                        _onRemovedElementCallback = (instance, value) =>
                        {
                            onRemovedElementMethod.Invoke(instance, new object[] { value });
                        };
                    }

                    if (_listDrawerSettings.CustomCreateElementFunction.IsNotNullOrEmpty())
                    {
                        var customCreateElementFunction = targetType.GetMethodEx(_listDrawerSettings.CustomCreateElementFunction, BindingFlagsHelper.All)
                            ?? throw new Exception($"Cannot find method '{_listDrawerSettings.CustomCreateElementFunction}' in '{targetType}'");

                        _customCreateElementFunction = instance =>
                        {
                            return customCreateElementFunction.Invoke(instance, null);
                        };
                    }

                    if (_listDrawerSettings.CustomRemoveElementFunction.IsNotNullOrEmpty())
                    {
                        var customRemoveElementFunction = targetType.GetMethodEx(_listDrawerSettings.CustomRemoveElementFunction, BindingFlagsHelper.All)
                            ?? throw new Exception($"Cannot find method '{_listDrawerSettings.CustomRemoveElementFunction}' in '{targetType}'");

                        _customRemoveElementFunction = (instance, value) =>
                        {
                            customRemoveElementFunction.Invoke(instance, new object[] { value });
                        };
                    }

                    if (_listDrawerSettings.CustomRemoveIndexFunction.IsNotNullOrEmpty())
                    {
                        var customRemoveIndexFunction = targetType.GetMethodEx(_listDrawerSettings.CustomRemoveIndexFunction, BindingFlagsHelper.All, typeof(int))
                            ?? throw new Exception($"Cannot find method '{_listDrawerSettings.CustomRemoveIndexFunction}' in '{targetType}'");
                        _customRemoveIndexFunction = (instance, index) =>
                        {
                            customRemoveIndexFunction.Invoke(instance, new object[] { index });
                        };
                    }
                }
                catch (Exception e)
                {
                    _error = e.Message;
                }
            }
        }

        protected override void DrawProperty(GUIContent label)
        {
            if (_error.IsNotNullOrEmpty())
            {
                EasyEditorGUI.MessageBox(_error, MessageType.Error);
                return;
            }

            if (_iconTextureGetterResolver != null && _iconTextureGetterResolver.HasError(out var error))
            {
                EasyEditorGUI.MessageBox(error, MessageType.Error);
                return;
            }

            EasyEditorGUI.BeginIndentedVertical(EasyGUIStyles.PropertyPadding);

            if (_listDrawerSettings is MetroListDrawerSettingsAttribute)
            {
                DrawMetroHeader(label);
                DrawMetroItems();
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

            if (label != null)
            {
                GUILayout.Label(label);
            }

            GUILayout.FlexibleSpace();

            if (!_listDrawerSettings.HideAddButton)
            {
                if (EasyEditorGUI.ToolbarButton(EasyEditorIcons.Plus))
                {
                    DoAddElement();
                }
            }


            EasyEditorGUI.EndHorizontalToolbar();
        }

        private void DrawMetroHeader(GUIContent label)
        {
            EasyGUIHelper.PushColor(MetroHeaderBackgroundColor);
            EasyEditorGUI.BeginHorizontalToolbar(30);
            EasyGUIHelper.PopColor();

            if (_iconTextureGetterResolver != null)
            {
                var iconTexture = _iconTextureGetterResolver.Resolve(Property.Parent.ValueEntry.WeakSmartValue);
                GUILayout.Label(iconTexture, GUILayout.Width(30), GUILayout.Height(30));
            }

            if (label != null)
            {
                GUILayout.Label(label, MetroHeaderLabelStyle, GUILayout.Height(30));
            }

            GUILayout.FlexibleSpace();

            if (!_listDrawerSettings.HideAddButton)
            {
                var btnRect = GUILayoutUtility.GetRect(
                EasyEditorIcons.Plus.HighlightedContent,
                "Button",
                GUILayout.ExpandWidth(false),
                GUILayout.Width(30),
                GUILayout.Height(30));

                if (GUI.Button(btnRect, GUIContent.none, "Button"))
                {
                    EasyGUIHelper.RemoveFocusControl();
                    DoAddElement();
                }

                if (Event.current.type == EventType.Repaint)
                {
                    EasyEditorIcons.Plus.Draw(btnRect.AlignCenter(25, 25));
                }
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

        private void DrawMetroItems()
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

            GUI.Label(dragHandleRect, EasyEditorIcons.List.InactiveTexture, GUIStyle.none);

            property.Draw(null);

            if (!_listDrawerSettings.HideRemoveButton)
            {
                var removeBtnRect = new Rect(dragHandleRect.x + rect.width - 22, dragHandleRect.y + 1, 14, 14);
                if (EasyEditorGUI.IconButton(removeBtnRect, EasyEditorIcons.X))
                {
                    if (_orderedCollectionResolver != null)
                    {
                        DoRemoveElementAt(index, property);
                    }
                    else
                    {
                        DoRemoveElement(property);
                    }
                }
            }

            EasyEditorGUI.EndListItem();
        }

        private void DrawAwesomeItem(InspectorProperty property, int index)
        {
            EasyGUIHelper.PushColor(MetroItemBackgroundColor);
            var rect = EasyEditorGUI.BeginListItem(false, MetroListItemStyle, GUILayout.MinHeight(25), GUILayout.ExpandWidth(true));
            EasyGUIHelper.PopColor();

            var dragHandleRect = new Rect(rect.x + 4, rect.y + 2 + ((int)rect.height - 23) / 2, 23, 23);

            GUI.Label(dragHandleRect, EasyEditorIcons.List.InactiveTexture, GUIStyle.none);

            property.Draw(null);

            if (!_listDrawerSettings.HideRemoveButton)
            {
                var removeBtnRect = new Rect(dragHandleRect.x + rect.width - 37, dragHandleRect.y - 5, 30, 30);
                if (GUI.Button(removeBtnRect, GUIContent.none, "Button"))
                {
                    EasyGUIHelper.RemoveFocusControl();

                    if (_orderedCollectionResolver != null)
                    {
                        DoRemoveElementAt(index, property);
                    }
                    else
                    {
                        DoRemoveElement(property);
                    }
                }

                if (Event.current.type == EventType.Repaint)
                {
                    EasyEditorIcons.X.Draw(removeBtnRect.AlignCenter(25, 25));
                }
            }


            EasyEditorGUI.EndListItem();
        }

        private void DoAddElement()
        {
            for (int i = 0; i < Property.Tree.Targets.Length; i++)
            {
                DoAddElement(i, GetValueToAdd(i));
            }
        }

        private void DoAddElement(int targetIndex, object valueToAdd)
        {
            _collectionResolver.QueueInsertElement(targetIndex, valueToAdd);
            _onAddedElementCallback?.Invoke(Property.Parent.ValueEntry.WeakValues[targetIndex], valueToAdd);
        }

        private void DoRemoveElementAt(int index, InspectorProperty propertyToRemove)
        {
            for (int i = 0; i < Property.Tree.Targets.Length; i++)
            {
                DoRemoveElementAt(i, index, propertyToRemove);
            }
        }

        private void DoRemoveElementAt(int targetIndex, int index, InspectorProperty propertyToRemove)
        {
            var parent = Property.Parent.ValueEntry.WeakValues[targetIndex];
            if (_customRemoveIndexFunction != null)
            {
                _customRemoveIndexFunction.Invoke(parent, index);
            }
            else
            {
                Assert.IsNotNull(_orderedCollectionResolver);
                _orderedCollectionResolver.QueueRemoveElementAt(targetIndex, index);
            }

            var valueToRemove = propertyToRemove.ValueEntry.WeakValues[targetIndex];
            _onRemovedElementCallback?.Invoke(parent, valueToRemove);
        }

        private void DoRemoveElement(InspectorProperty propertyToRemove)
        {
            for (int i = 0; i < Property.Tree.Targets.Length; i++)
            {
                DoRemoveElement(i, propertyToRemove);
            }
        }

        private void DoRemoveElement(int targetIndex, InspectorProperty propertyToRemove)
        {
            var parent = Property.Parent.ValueEntry.WeakValues[targetIndex];
            var valueToRemove = propertyToRemove.ValueEntry.WeakValues[targetIndex];
            if (_customRemoveElementFunction != null)
            {
                _customRemoveElementFunction.Invoke(parent, valueToRemove);
            }
            else
            {
                _collectionResolver.QueueRemoveElement(targetIndex, valueToRemove);
            }

            _onRemovedElementCallback?.Invoke(parent, valueToRemove);
        }

        private object GetValueToAdd(int targetIndex)
        {
            var parent = Property.Parent.ValueEntry.WeakValues[targetIndex];
            if (_customCreateElementFunction != null)
            {
                return _customCreateElementFunction.Invoke(parent);
            }

            if (_collectionResolver.ElementType.IsInheritsFrom<UnityEngine.Object>())
            {
                return null;
            }

            return UnitySerializationUtility.CreateDefaultUnityInitializedObject(_collectionResolver.ElementType);
        }
    }
}
