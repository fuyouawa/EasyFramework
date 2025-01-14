// using System;
// using System.Collections;
// using System.Globalization;
// using System.Linq;
// using Sirenix.OdinInspector;
// using Sirenix.OdinInspector.Editor;
// using Sirenix.Utilities;
// using Sirenix.Utilities.Editor;
// using UnityEditor;
// using UnityEngine;
//
// namespace EasyGameFramework
// {
//     internal class EditorListDrawHelper<T>
//         where T : IList
//     {
//         public static class CollectionDrawerStaticInfo
//         {
//             public static InspectorProperty CurrentDraggingPropertyInfo;
//             public static InspectorProperty CurrentDroppingPropertyInfo;
//             public static DelayedGUIDrawer DelayedGUIDrawer = new DelayedGUIDrawer();
//             internal static Action NextCustomAddFunction;
//         }
//
//         internal class CollectionSizeDialogue
//         {
//             public int Size;
//             private Action<int> confirm;
//             private Action cancel;
//
//             public CollectionSizeDialogue(Action<int> confirm, Action cancel, int size)
//             {
//                 this.confirm = confirm;
//                 this.cancel = cancel;
//                 this.Size = size;
//             }
//
//             [Button(ButtonSizes.Medium), HorizontalGroup(0.5f)]
//             public void Confirm()
//             {
//                 this.confirm(this.Size);
//             }
//
//             [Button(ButtonSizes.Medium), HorizontalGroup]
//             public void Cancel()
//             {
//                 this.cancel();
//             }
//         }
//
//         private static GUILayoutOption[] listItemOptions = GUILayoutOptions.MinHeight(25).ExpandWidth(true);
//         private ListDrawerConfigInfo info;
//         private string errorMessage;
//
//
//         public void Initialize(
//             T collection,
//             bool isReadOnly, bool toggled, bool cantDraggable,
//             Action<object, int> onBeginListElementGUI,
//             Action<object, int> onEndListElementGUI,
//             Func<object, object> getListElementLabelText,
//             Func<object, object> getCustomAddFunction,
//             Action<object> getCustomAddFunctionVoid,
//             Action<object> onTitleBarGUI,
//             Action<object, int> customRemoveIndexFunction,
//             Action<object, object> customRemoveElementFunction,
//             ListDrawerSettingsAttribute customListDrawerOptions)
//         {
//             this.info = new ListDrawerConfigInfo()
//             {
//                 Collection = collection,
//                 StartIndex = 0,
//                 Toggled = toggled,
//                 RemoveAt = -1,
//                 ShowAllWhilePaging = false,
//                 EndIndex = 0,
//                 CustomListDrawerOptions = customListDrawerOptions,
//                 IsReadOnly = isReadOnly,
//                 Draggable = !isReadOnly && (!customListDrawerOptions.IsReadOnlyHasValue),
//                 HideAddButton = isReadOnly || customListDrawerOptions.HideAddButton,
//                 HideRemoveButton = isReadOnly || customListDrawerOptions.HideRemoveButton,
//             };
//
//             this.info.ListConfig = GeneralDrawerConfig.Instance;
//
//             if (customListDrawerOptions.DraggableHasValue && !customListDrawerOptions.DraggableItems)
//             {
//                 this.info.Draggable = false;
//             }
//
//             if (cantDraggable)
//             {
//                 this.info.Draggable = false;
//             }
//
//             this.info.OnBeginListElementGUI = onBeginListElementGUI;
//             this.info.OnEndListElementGUI = onEndListElementGUI;
//             this.info.OnTitleBarGUI = onTitleBarGUI;
//             this.info.GetListElementLabelText = getListElementLabelText;
//             this.info.GetCustomAddFunction = getCustomAddFunction;
//             this.info.GetCustomAddFunctionVoid = getCustomAddFunctionVoid;
//             this.info.CustomRemoveIndexFunction = customRemoveIndexFunction;
//             this.info.CustomRemoveElementFunction = customRemoveElementFunction;
//         }
//
//
//         /// <summary>
//         /// Draws the property.
//         /// </summary>
//         public void DrawPropertyLayout(object key, GUIContent label)
//         {
//             if (this.errorMessage != null)
//             {
//                 SirenixEditorGUI.ErrorMessageBox(this.errorMessage);
//             }
//
//             if (this.info.Label == null || (label != null && label.text != this.info.Label.text))
//             {
//                 this.info.Label =
//                     new GUIContent(
//                         label == null || string.IsNullOrEmpty(label.text)
//                             ? this.info.Collection.GetType().GetNiceName()
//                             : label.text, label == null ? string.Empty : label.tooltip);
//             }
//
//             this.info.ListItemStyle.padding.left = this.info.Draggable ? 25 : 7;
//             this.info.ListItemStyle.padding.right = this.info.IsReadOnly || this.info.HideRemoveButton ? 4 : 20;
//
//             if (Event.current.type == EventType.Repaint)
//             {
//                 this.info.DropZoneTopLeft = GUIUtility.GUIToScreenPoint(new Vector2(0, 0));
//             }
//
//             SirenixEditorGUI.BeginIndentedVertical(SirenixGUIStyles.PropertyPadding);
//             this.BeginDropZone();
//             {
//                 this.DrawToolbar();
//                 if (SirenixEditorGUI.BeginFadeGroup(key, this.info.Toggled))
//                 {
//                     GUIHelper.PushLabelWidth(GUIHelper.BetterLabelWidth - this.info.ListItemStyle.padding.left);
//                     this.DrawItems();
//                     GUIHelper.PopLabelWidth();
//                 }
//
//                 SirenixEditorGUI.EndFadeGroup();
//             }
//             this.EndDropZone();
//             SirenixEditorGUI.EndIndentedVertical();
//             
//             if (this.info.RemoveAt >= 0 && Event.current.type == EventType.Repaint)
//             {
//                 try
//                 {
//                     if (this.info.CustomRemoveIndexFunction != null)
//                     {
//                         info.CustomRemoveIndexFunction(info, info.RemoveAt);
//                     }
//                     else if (this.info.CustomRemoveElementFunction != null)
//                     {
//                         info.CustomRemoveElementFunction(info, info.Collection[info.RemoveAt]);
//                     }
//                     else
//                     {
//                         info.Collection.RemoveAt(info.RemoveAt);
//                     }
//                 }
//                 finally
//                 {
//                     this.info.RemoveAt = -1;
//                 }
//
//                 GUIHelper.RequestRepaint();
//             }
//         }
//
//         private DropZoneHandle BeginDropZone()
//         {
//             var dropZone = DragAndDropManager.BeginDropZone(
//                 this.info.GetHashCode(),
//                 this.info.Collection.GetType(), true);
//
//             if (Event.current.type == EventType.Repaint && DragAndDropManager.IsDragInProgress)
//             {
//                 var rect = dropZone.Rect;
//                 dropZone.Rect = rect;
//             }
//
//             dropZone.Enabled = this.info.IsReadOnly == false;
//             this.info.DropZone = dropZone;
//             return dropZone;
//         }
//
//         private static UnityEngine.Object[] HandleUnityObjectsDrop(ListDrawerConfigInfo info)
//         {
//             if (info.IsReadOnly) return null;
//
//             var eventType = Event.current.type;
//             if (eventType == EventType.Layout)
//             {
//                 info.IsAboutToDroppingUnityObjects = false;
//             }
//
//             if ((eventType == EventType.DragUpdated || eventType == EventType.DragPerform) &&
//                 info.DropZone.Rect.Contains(Event.current.mousePosition))
//             {
//                 UnityEngine.Object[] objReferences = null;
//
//                 if (DragAndDrop.objectReferences.Any(n =>
//                         n != null && info.CollectionResolver.ElementType.IsAssignableFrom(n.GetType())))
//                 {
//                     objReferences = DragAndDrop.objectReferences
//                         .Where(x => x != null && info.CollectionResolver.ElementType.IsAssignableFrom(x.GetType()))
//                         .Reverse().ToArray();
//                 }
//                 else if (info.CollectionResolver.ElementType.InheritsFrom(typeof(Component)))
//                 {
//                     objReferences = DragAndDrop.objectReferences.OfType<GameObject>()
//                         .Select(x => x.GetComponent(info.CollectionResolver.ElementType)).Where(x => x != null)
//                         .Reverse().ToArray();
//                 }
//                 else if (info.CollectionResolver.ElementType.InheritsFrom(typeof(Sprite)) &&
//                          DragAndDrop.objectReferences.Any(n => n is Texture2D && AssetDatabase.Contains(n)))
//                 {
//                     objReferences = DragAndDrop.objectReferences.OfType<Texture2D>().Select(x =>
//                     {
//                         var path = AssetDatabase.GetAssetPath(x);
//                         return AssetDatabase.LoadAssetAtPath<Sprite>(path);
//                     }).Where(x => x != null).Reverse().ToArray();
//                 }
//
//                 bool acceptsDrag = objReferences != null && objReferences.Length > 0;
//
//                 if (acceptsDrag)
//                 {
//                     DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
//                     Event.current.Use();
//                     info.IsAboutToDroppingUnityObjects = true;
//                     info.IsDroppingUnityObjects = info.IsAboutToDroppingUnityObjects;
//                     if (eventType == EventType.DragPerform)
//                     {
//                         DragAndDrop.AcceptDrag();
//                         return objReferences;
//                     }
//                 }
//             }
//
//             if (eventType == EventType.Repaint)
//             {
//                 info.IsDroppingUnityObjects = info.IsAboutToDroppingUnityObjects;
//             }
//
//             return null;
//         }
//
//         private void EndDropZone()
//         {
//             if (this.info.OrderedCollectionResolver == null) return;
//
//             if (this.info.DropZone.IsReadyToClaim)
//             {
//                 CollectionDrawerStaticInfo.CurrentDraggingPropertyInfo = null;
//                 CollectionDrawerStaticInfo.CurrentDroppingPropertyInfo = this.info.Property;
//                 object droppedObject = this.info.DropZone.ClaimObject();
//
//                 object[] values = new object[this.info.Property.Tree.WeakTargets.Count];
//
//                 for (int i = 0; i < values.Length; i++)
//                 {
//                     values[i] = droppedObject;
//                 }
//
//                 if (this.info.DropZone.IsCrossWindowDrag)
//                 {
//                     // If it's a cross-window drag, the changes will for some reason be lost if we don't do this.
//                     GUIHelper.RequestRepaint();
//                     EditorApplication.delayCall += () =>
//                     {
//                         this.info.OrderedCollectionResolver.QueueInsertAt(
//                             Mathf.Clamp(this.info.InsertAt, 0, this.info.Property.Children.Count), values);
//                     };
//                 }
//                 else
//                 {
//                     this.info.OrderedCollectionResolver.QueueInsertAt(
//                         Mathf.Clamp(this.info.InsertAt, 0, this.info.Property.Children.Count), values);
//                 }
//             }
//             else if (this.info.IsReadOnly == false)
//             {
//                 UnityEngine.Object[] droppedObjects = HandleUnityObjectsDrop(this.info);
//                 if (droppedObjects != null)
//                 {
//                     foreach (var obj in droppedObjects)
//                     {
//                         object[] values = new object[this.info.Property.Tree.WeakTargets.Count];
//
//                         for (int i = 0; i < values.Length; i++)
//                         {
//                             values[i] = obj;
//                         }
//
//                         this.info.OrderedCollectionResolver.QueueInsertAt(
//                             Mathf.Clamp(this.info.InsertAt, 0, this.info.Property.Children.Count), values);
//                     }
//                 }
//             }
//
//             DragAndDropManager.EndDropZone();
//         }
//
//         private void DrawToolbar()
//         {
//             SirenixEditorGUI.BeginHorizontalToolbar();
//             {
//                 // Label
//                 if (this.info.DropZone != null && DragAndDropManager.IsDragInProgress &&
//                     this.info.DropZone.IsAccepted == false)
//                 {
//                     GUIHelper.PushGUIEnabled(false);
//                 }
//
//                 if (this.info.Property.ValueEntry.ListLengthChangedFromPrefab)
//                 {
//                     GUIHelper.PushIsBoldLabel(true);
//                 }
//
//                 if (this.info.ListConfig.HideFoldoutWhileEmpty && this.info.IsEmpty ||
//                     this.info.CustomListDrawerOptions.Expanded)
//                 {
//                     GUILayout.Label(this.info.Label, GUILayoutOptions.ExpandWidth(false));
//                 }
//                 else
//                 {
//                     this.info.Toggled.Value =
//                         SirenixEditorGUI.Foldout(this.info.Toggled.Value, this.info.Label ?? GUIContent.none);
//                 }
//
//                 if (this.info.Property.ValueEntry.ListLengthChangedFromPrefab)
//                 {
//                     GUIHelper.PopIsBoldLabel();
//                 }
//
//                 if (this.info.CustomListDrawerOptions.Expanded)
//                 {
//                     this.info.Toggled.Value = true;
//                 }
//
//                 if (this.info.DropZone != null && DragAndDropManager.IsDragInProgress &&
//                     this.info.DropZone.IsAccepted == false)
//                 {
//                     GUIHelper.PopGUIEnabled();
//                 }
//
//                 GUILayout.FlexibleSpace();
//
//                 // Item Count
//                 if (this.info.CustomListDrawerOptions.ShowItemCountHasValue
//                         ? this.info.CustomListDrawerOptions.ShowItemCount
//                         : this.info.ListConfig.ShowItemCount)
//                 {
//                     if (this.info.Property.ValueEntry.ValueState == PropertyValueState.CollectionLengthConflict)
//                     {
//                         GUILayout.Label(
//                             this.info.Count + " / " + this.info.CollectionResolver.MaxCollectionLength + " items",
//                             EditorStyles.centeredGreyMiniLabel);
//                     }
//                     else
//                     {
//                         GUILayout.Label(this.info.IsEmpty ? "Empty" : this.info.Count + " items",
//                             EditorStyles.centeredGreyMiniLabel);
//                     }
//                 }
//
//                 bool paging = this.info.CustomListDrawerOptions.PagingHasValue
//                     ? this.info.CustomListDrawerOptions.ShowPaging
//                     : true;
//                 bool hidePaging =
//                     this.info.ListConfig.HidePagingWhileCollapsed && this.info.Toggled.Value == false ||
//                     this.info.ListConfig.HidePagingWhileOnlyOnePage &&
//                     this.info.Count <= this.info.NumberOfItemsPerPage;
//
//                 int numberOfItemsPrPage = Math.Max(1, this.info.NumberOfItemsPerPage);
//                 int numberOfPages = Mathf.CeilToInt(this.info.Count / (float)numberOfItemsPrPage);
//                 int pageIndex = this.info.Count == 0
//                     ? 0
//                     : (this.info.StartIndex / numberOfItemsPrPage) % this.info.Count;
//
//                 // Paging
//                 if (paging)
//                 {
//                     bool disablePaging = paging && !hidePaging && (DragAndDropManager.IsDragInProgress ||
//                                                                    this.info.ShowAllWhilePaging ||
//                                                                    this.info.Toggled.Value == false);
//                     if (disablePaging)
//                     {
//                         GUIHelper.PushGUIEnabled(false);
//                     }
//
//                     if (!hidePaging)
//                     {
//                         if (pageIndex == 0)
//                         {
//                             GUIHelper.PushGUIEnabled(false);
//                         }
//
//                         if (SirenixEditorGUI.ToolbarButton(EditorIcons.TriangleLeft, true))
//                         {
//                             if (Event.current.button == 0)
//                             {
//                                 this.info.StartIndex -= numberOfItemsPrPage;
//                             }
//                             else
//                             {
//                                 this.info.StartIndex = 0;
//                             }
//                         }
//
//                         if (pageIndex == 0)
//                         {
//                             GUIHelper.PopGUIEnabled();
//                         }
//
//                         var userPageIndex = EditorGUILayout.IntField((numberOfPages == 0 ? 0 : (pageIndex + 1)),
//                             GUILayoutOptions.Width(
//                                 10 + numberOfPages.ToString(CultureInfo.InvariantCulture).Length * 10)) - 1;
//                         if (pageIndex != userPageIndex)
//                         {
//                             this.info.StartIndex = userPageIndex * numberOfItemsPrPage;
//                         }
//
//                         GUILayout.Label("/ " + numberOfPages);
//
//                         if (pageIndex == numberOfPages - 1)
//                         {
//                             GUIHelper.PushGUIEnabled(false);
//                         }
//
//                         if (SirenixEditorGUI.ToolbarButton(EditorIcons.TriangleRight, true))
//                         {
//                             if (Event.current.button == 0)
//                             {
//                                 this.info.StartIndex += numberOfItemsPrPage;
//                             }
//                             else
//                             {
//                                 this.info.StartIndex = numberOfItemsPrPage * numberOfPages;
//                             }
//                         }
//
//                         if (pageIndex == numberOfPages - 1)
//                         {
//                             GUIHelper.PopGUIEnabled();
//                         }
//                     }
//
//                     pageIndex = this.info.Count == 0
//                         ? 0
//                         : (this.info.StartIndex / numberOfItemsPrPage) % this.info.Count;
//
//                     var newStartIndex = Mathf.Clamp(pageIndex * numberOfItemsPrPage, 0,
//                         Mathf.Max(0, this.info.Count - 1));
//                     if (newStartIndex != this.info.StartIndex)
//                     {
//                         this.info.StartIndex = newStartIndex;
//                         var newPageIndex = this.info.Count == 0
//                             ? 0
//                             : (this.info.StartIndex / numberOfItemsPrPage) % this.info.Count;
//                         if (pageIndex != newPageIndex)
//                         {
//                             pageIndex = newPageIndex;
//                             this.info.StartIndex = Mathf.Clamp(pageIndex * numberOfItemsPrPage, 0,
//                                 Mathf.Max(0, this.info.Count - 1));
//                         }
//                     }
//
//                     this.info.EndIndex = Mathf.Min(this.info.StartIndex + numberOfItemsPrPage, this.info.Count);
//
//                     if (disablePaging)
//                     {
//                         GUIHelper.PopGUIEnabled();
//                     }
//                 }
//                 else
//                 {
//                     this.info.StartIndex = 0;
//                     this.info.EndIndex = this.info.Count;
//                 }
//
//                 if (paging && hidePaging == false && this.info.ListConfig.ShowExpandButton)
//                 {
//                     if (this.info.Count < 300)
//                     {
//                         if (SirenixEditorGUI.ToolbarButton(
//                                 this.info.ShowAllWhilePaging ? EditorIcons.TriangleUp : EditorIcons.TriangleDown, true))
//                         {
//                             this.info.ShowAllWhilePaging = !this.info.ShowAllWhilePaging;
//                         }
//                     }
//                     else
//                     {
//                         this.info.ShowAllWhilePaging = false;
//                     }
//                 }
//
//                 // Add Button
//                 if (this.info.IsReadOnly == false && !this.info.HideAddButton)
//                 {
//                     this.info.ObjectPicker =
//                         ObjectPicker.GetObjectPicker(this.info, this.info.CollectionResolver.ElementType);
//                     var superHackyAddFunctionWeSeriouslyNeedANewListDrawer =
//                         CollectionDrawerStaticInfo.NextCustomAddFunction;
//                     CollectionDrawerStaticInfo.NextCustomAddFunction = null;
//
//                     if (SirenixEditorGUI.ToolbarButton(EditorIcons.Plus))
//                     {
//                         if (superHackyAddFunctionWeSeriouslyNeedANewListDrawer != null)
//                         {
//                             superHackyAddFunctionWeSeriouslyNeedANewListDrawer();
//                         }
//                         else if (this.info.GetCustomAddFunctionVoid != null)
//                         {
//                             this.info.GetCustomAddFunctionVoid(this.info.Property.ParentValues[0]);
//
//                             this.Property.ValueEntry.WeakValues.ForceMarkDirty();
//                         }
//                         else
//                         {
//                             object[] objs = new object[this.info.Property.ValueEntry.ValueCount];
//
//                             bool wasFallback;
//
//                             objs[0] = this.GetValueToAdd(0, out wasFallback);
//
//                             if (wasFallback)
//                             {
//                                 this.info.ObjectPicker.ShowObjectPicker(
//                                     null,
//                                     this.info.Property.GetAttribute<AssetsOnlyAttribute>() == null,
//                                     GUIHelper.GetCurrentLayoutRect(),
//                                     this.info.Property.ValueEntry.SerializationBackend == SerializationBackend.Unity);
//                             }
//                             else
//                             {
//                                 for (int i = 1; i < objs.Length; i++)
//                                 {
//                                     objs[i] = this.GetValueToAdd(i);
//                                 }
//
//                                 this.info.CollectionResolver.QueueAdd(objs);
//                             }
//                         }
//                     }
//
//                     this.info.JumpToNextPageOnAdd = paging && (this.info.Count % numberOfItemsPrPage == 0) &&
//                                                     (pageIndex + 1 == numberOfPages);
//                 }
//
//                 if (this.info.OnTitleBarGUI != null)
//                 {
//                     this.info.OnTitleBarGUI(this.info.Property.ParentValues[0]);
//                 }
//             }
//             SirenixEditorGUI.EndHorizontalToolbar();
//         }
//
//         private void DrawItems()
//         {
//             int from = 0;
//             int to = this.info.Count;
//             bool paging = this.info.CustomListDrawerOptions.PagingHasValue
//                 ? this.info.CustomListDrawerOptions.ShowPaging
//                 : true;
//             if (paging && this.info.ShowAllWhilePaging == false)
//             {
//                 from = Mathf.Clamp(this.info.StartIndex, 0, this.info.Count);
//                 to = Mathf.Clamp(this.info.EndIndex, 0, this.info.Count);
//             }
//
//             var drawEmptySpace = this.info.DropZone != null && this.info.DropZone.IsBeingHovered ||
//                                  this.info.IsDroppingUnityObjects;
//             float height = drawEmptySpace
//                 ? this.info.IsDroppingUnityObjects ? 16 : (DragAndDropManager.CurrentDraggingHandle.Rect.height - 3)
//                 : 0;
//             var rect = SirenixEditorGUI.BeginVerticalList();
//             {
//                 for (int i = 0, j = from, k = from; j < to; i++, j++)
//                 {
//                     var dragHandle = this.BeginDragHandle(j, i);
//                     {
//                         if (drawEmptySpace)
//                         {
//                             var topHalf = dragHandle.Rect;
//                             topHalf.height /= 2;
//                             if (topHalf.Contains(this.info.LayoutMousePosition) ||
//                                 topHalf.y > this.info.LayoutMousePosition.y && i == 0)
//                             {
//                                 GUILayout.Space(height);
//                                 drawEmptySpace = false;
//                                 this.info.InsertAt = k;
//                             }
//                         }
//
//                         if (dragHandle.IsDragging == false)
//                         {
//                             k++;
//                             this.DrawItem(this.info.Property.Children[j], dragHandle, j);
//                         }
//                         else
//                         {
//                             GUILayout.Space(3);
//                             CollectionDrawerStaticInfo.DelayedGUIDrawer.Begin(dragHandle.Rect.width,
//                                 dragHandle.Rect.height, dragHandle.CurrentMethod != DragAndDropMethods.Move);
//                             DragAndDropManager.AllowDrop = false;
//                             this.DrawItem(this.info.Property.Children[j], dragHandle, j);
//                             DragAndDropManager.AllowDrop = true;
//                             CollectionDrawerStaticInfo.DelayedGUIDrawer.End();
//                             if (dragHandle.CurrentMethod != DragAndDropMethods.Move)
//                             {
//                                 GUILayout.Space(3);
//                             }
//                         }
//
//                         if (drawEmptySpace)
//                         {
//                             var bottomHalf = dragHandle.Rect;
//                             bottomHalf.height /= 2;
//                             bottomHalf.y += bottomHalf.height;
//
//                             if (bottomHalf.Contains(this.info.LayoutMousePosition) ||
//                                 bottomHalf.yMax < this.info.LayoutMousePosition.y && j + 1 == to)
//                             {
//                                 GUILayout.Space(height);
//                                 drawEmptySpace = false;
//                                 this.info.InsertAt = Mathf.Min(k, to);
//                             }
//                         }
//                     }
//                     this.EndDragHandle(i);
//                 }
//
//                 if (drawEmptySpace)
//                 {
//                     GUILayout.Space(height);
//                     this.info.InsertAt = Event.current.mousePosition.y > rect.center.y ? to : from;
//                 }
//
//                 if (to == this.info.Property.Children.Count && this.info.Property.ValueEntry.ValueState ==
//                     PropertyValueState.CollectionLengthConflict)
//                 {
//                     SirenixEditorGUI.BeginListItem(false);
//                     GUILayout.Label(GUIHelper.TempContent("------"), EditorStyles.centeredGreyMiniLabel);
//                     SirenixEditorGUI.EndListItem();
//                 }
//             }
//             SirenixEditorGUI.EndVerticalList();
//
//             if (Event.current.type == EventType.Repaint)
//             {
//                 this.info.LayoutMousePosition = Event.current.mousePosition;
//             }
//         }
//
//         private void EndDragHandle(int i)
//         {
//             var handle = DragAndDropManager.EndDragHandle();
//
//             if (handle.IsDragging)
//             {
//                 this.info.Property.Tree.DelayAction(() =>
//                 {
//                     if (DragAndDropManager.CurrentDraggingHandle != null)
//                     {
//                         CollectionDrawerStaticInfo.DelayedGUIDrawer.Draw(Event.current.mousePosition -
//                                                                          DragAndDropManager.CurrentDraggingHandle
//                                                                              .MouseDownPostionOffset);
//                     }
//                 });
//             }
//         }
//
//         private DragHandle BeginDragHandle(int j, int i)
//         {
//             var child = this.info.Property.Children[j];
//             var dragHandle = DragAndDropManager.BeginDragHandle(child, child.ValueEntry.WeakSmartValue,
//                 this.info.IsReadOnly ? DragAndDropMethods.Reference : DragAndDropMethods.Move);
//             dragHandle.Enabled = this.info.Draggable;
//
//             if (dragHandle.OnDragStarted)
//             {
//                 CollectionDrawerStaticInfo.CurrentDroppingPropertyInfo = null;
//                 CollectionDrawerStaticInfo.CurrentDraggingPropertyInfo = this.info.Property.Children[j];
//                 dragHandle.OnDragFinnished = dropEvent =>
//                 {
//                     if (dropEvent == DropEvents.Moved)
//                     {
//                         if (dragHandle.IsCrossWindowDrag ||
//                             (CollectionDrawerStaticInfo.CurrentDroppingPropertyInfo != null &&
//                              CollectionDrawerStaticInfo.CurrentDroppingPropertyInfo.Tree != this.info.Property.Tree))
//                         {
//                             // Make sure drop happens a bit later, as deserialization and other things sometimes
//                             // can override the change.
//                             GUIHelper.RequestRepaint();
//                             EditorApplication.delayCall += () =>
//                             {
//                                 this.info.OrderedCollectionResolver.QueueRemoveAt(j);
//                             };
//                         }
//                         else
//                         {
//                             this.info.OrderedCollectionResolver.QueueRemoveAt(j);
//                         }
//                     }
//
//                     CollectionDrawerStaticInfo.CurrentDraggingPropertyInfo = null;
//                 };
//             }
//
//             return dragHandle;
//         }
//
//         private Rect DrawItem(InspectorProperty itemProperty, DragHandle dragHandle, int index = -1)
//         {
//             var listItemInfo = itemProperty.Context.GetGlobal<ListItemInfo>("listItemInfo");
//
//             Rect rect;
//             rect = SirenixEditorGUI.BeginListItem(false, this.info.ListItemStyle, listItemOptions);
//             {
//                 if (Event.current.type == EventType.Repaint && !this.info.IsReadOnly)
//                 {
//                     listItemInfo.Value.Width = rect.width;
//                     dragHandle.DragHandleRect = new Rect(rect.x + 4, rect.y, 20, rect.height);
//                     listItemInfo.Value.DragHandleRect =
//                         new Rect(rect.x + 4, rect.y + 2 + ((int)rect.height - 23) / 2, 20, 20);
//                     listItemInfo.Value.RemoveBtnRect = new Rect(listItemInfo.Value.DragHandleRect.x + rect.width - 22,
//                         listItemInfo.Value.DragHandleRect.y + 1, 14, 14);
//
//                     if (this.info.HideRemoveButton == false)
//                     {
//                     }
//
//                     if (this.info.Draggable)
//                     {
//                         GUI.Label(listItemInfo.Value.DragHandleRect, EditorIcons.List.Inactive, GUIStyle.none);
//                     }
//                 }
//
//                 GUIHelper.PushHierarchyMode(false);
//                 GUIContent label = null;
//
//                 if (this.info.CustomListDrawerOptions.ShowIndexLabelsHasValue)
//                 {
//                     if (this.info.CustomListDrawerOptions.ShowIndexLabels)
//                     {
//                         label = new GUIContent(index.ToString());
//                     }
//                 }
//                 else if (this.info.ListConfig.ShowIndexLabels)
//                 {
//                     label = new GUIContent(index.ToString());
//                 }
//
//                 if (this.info.GetListElementLabelText != null)
//                 {
//                     var value = itemProperty.ValueEntry.WeakSmartValue;
//
//                     if (object.ReferenceEquals(value, null))
//                     {
//                         if (label == null)
//                         {
//                             label = new GUIContent("Null");
//                         }
//                         else
//                         {
//                             label.text += " : Null";
//                         }
//                     }
//                     else
//                     {
//                         label = label ?? new GUIContent("");
//                         if (label.text != "") label.text += " : ";
//
//                         object text = this.info.GetListElementLabelText(value);
//                         label.text += (text == null ? "" : text.ToString());
//                     }
//                 }
//
//                 if (this.info.OnBeginListElementGUI != null)
//                 {
//                     this.info.OnBeginListElementGUI(this.info.Property.ParentValues[0], index);
//                 }
//
//                 itemProperty.Draw(label);
//
//                 if (this.info.OnEndListElementGUI != null)
//                 {
//                     this.info.OnEndListElementGUI(this.info.Property.ParentValues[0], index);
//                 }
//
//                 GUIHelper.PopHierarchyMode();
//
//                 if (this.info.IsReadOnly == false && this.info.HideRemoveButton == false)
//                 {
//                     if (SirenixEditorGUI.IconButton(listItemInfo.Value.RemoveBtnRect, EditorIcons.X))
//                     {
//                         if (this.info.OrderedCollectionResolver != null)
//                         {
//                             if (index >= 0)
//                             {
//                                 this.info.RemoveAt = index;
//                             }
//                         }
//                         else
//                         {
//                             var values = new object[itemProperty.ValueEntry.ValueCount];
//
//                             for (int i = 0; i < values.Length; i++)
//                             {
//                                 values[i] = itemProperty.ValueEntry.WeakValues[i];
//                             }
//
//                             this.info.RemoveValues = values;
//                         }
//                     }
//                 }
//             }
//             SirenixEditorGUI.EndListItem();
//
//             return rect;
//         }
//
//
//         private class ListDrawerConfigInfo
//         {
//             public ListDrawerSettingsAttribute CustomListDrawerOptions;
//
//             public T Collection;
//             // public LocalPersistentContext<bool> Toggled;
//             public bool Toggled;
//             public int StartIndex;
//             public int EndIndex;
//             public DropZoneHandle DropZone;
//             public Vector2 LayoutMousePosition;
//             public Vector2 DropZoneTopLeft;
//             public int InsertAt;
//             public int RemoveAt;
//             public object[] RemoveValues;
//             public bool IsReadOnly;
//             public bool Draggable;
//             public bool ShowAllWhilePaging;
//             public ObjectPicker ObjectPicker;
//             public bool JumpToNextPageOnAdd;
//             public Action<object> OnTitleBarGUI;
//             public GeneralDrawerConfig ListConfig;
//             public GUIContent Label;
//             public bool IsAboutToDroppingUnityObjects;
//             public bool IsDroppingUnityObjects;
//             public bool HideAddButton;
//             public bool HideRemoveButton;
//
//             public Action<object> GetCustomAddFunctionVoid;
//             public Func<object, object> GetCustomAddFunction;
//
//             public Action<ListDrawerConfigInfo, int> CustomRemoveIndexFunction;
//             public Action<ListDrawerConfigInfo, object> CustomRemoveElementFunction;
//
//             public Func<object, object> GetListElementLabelText;
//             public Action<object, int> OnBeginListElementGUI;
//             public Action<object, int> OnEndListElementGUI;
//
//             public int NumberOfItemsPerPage
//             {
//                 get
//                 {
//                     return this.CustomListDrawerOptions.NumberOfItemsPerPageHasValue
//                         ? this.CustomListDrawerOptions.NumberOfItemsPerPage
//                         : this.ListConfig.NumberOfItemsPrPage;
//                 }
//             }
//
//             public GUIStyle ListItemStyle = new GUIStyle(GUIStyle.none)
//             {
//                 padding = new RectOffset(25, 20, 3, 3)
//             };
//         }
//
//         private struct ListItemInfo
//         {
//             public float Width;
//             public Rect RemoveBtnRect;
//             public Rect DragHandleRect;
//         }
//     }
// }
