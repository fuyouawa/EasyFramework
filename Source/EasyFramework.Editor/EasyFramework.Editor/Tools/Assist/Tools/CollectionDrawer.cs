// using Sirenix.OdinInspector;
// using Sirenix.OdinInspector.Editor;
//
// namespace EasyGameFramework.Editor
// {
//
//     using Sirenix.Serialization;
//     using Sirenix.Utilities.Editor;
//     using Sirenix.Utilities;
//     using System;
//     using System.Collections;
//     using System.Globalization;
//     using System.Linq;
//     using System.Reflection;
//     using UnityEditor;
//     using UnityEngine;
//
//     //
//     // TODO: Rewrite ListDrawer completely!
//     // Make a utility for drawing lists that dictioanries, hashsets, etc can utialize.
//     // And use the new DragAndDropUtilities instead of the old and broken DragAndDropmanager.
//     // Handle both drag and drop event, in the same method. Preferably, instead of having order dependency which is impossible
//     // with cross window dragging etc...
//     //
//
//     public static class CollectionDrawerStaticInfo
//     {
//         public static InspectorProperty CurrentDraggingPropertyInfo;
//         public static InspectorProperty CurrentDroppingPropertyInfo;
//         public static DelayedGUIDrawer DelayedGUIDrawer = new DelayedGUIDrawer();
//         internal static Action NextCustomAddFunction;
//     }
//
//     internal class CollectionSizeDialogue
//     {
//         public int Size;
//         private Action<int> confirm;
//         private Action cancel;
//
//         public CollectionSizeDialogue(Action<int> confirm, Action cancel, int size)
//         {
//             this.confirm = confirm;
//             this.cancel = cancel;
//             this.Size = size;
//         }
//
//         [Button(ButtonSizes.Medium), HorizontalGroup(0.5f)]
//         public void Confirm()
//         {
//             this.confirm(this.Size);
//         }
//
//         [Button(ButtonSizes.Medium), HorizontalGroup]
//         public void Cancel()
//         {
//             this.cancel();
//         }
//     }
//
//     /// <summary>
//     /// Property drawer for anything that has a <see cref="ICollectionResolver"/>.
//     /// </summary>
//     [AllowGUIEnabledForReadonly]
//     [DrawerPriority(0, 0, 1)]
//     public class CollectionDrawer<T> : OdinValueDrawer<T>, IDefinesGenericMenuItems
//     {
//         private static GUILayoutOption[] listItemOptions = GUILayoutOptions.MinHeight(25).ExpandWidth(true);
//         private ListDrawerConfigInfo info;
//         private string errorMessage;
//
//         protected override bool CanDrawValueProperty(InspectorProperty property)
//         {
//             return property.ChildResolver is ICollectionResolver;
//         }
//
//         void IDefinesGenericMenuItems.PopulateGenericMenu(InspectorProperty property, GenericMenu genericMenu)
//         {
//             if (property.ValueEntry.WeakSmartValue == null)
//             {
//                 return;
//             }
//
//             var resolver = property.ChildResolver as ICollectionResolver;
//
//             bool isReadOnly = resolver.IsReadOnly;
//
//             var config = property.GetAttribute<ListDrawerSettingsAttribute>();
//             bool isEditable = isReadOnly == false && property.ValueEntry.IsEditable && (config == null || (!config.IsReadOnlyHasValue) || (config.IsReadOnlyHasValue && config.IsReadOnly == false));
//             bool pasteElement = isEditable && Clipboard.CanPaste(resolver.ElementType);
//             bool clearList = isEditable && property.Children.Count > 0;
//             bool setCollectionLength = isEditable && property.ChildResolver is IOrderedCollectionResolver && typeof(IList).IsAssignableFrom(typeof(T));
//
//             //if (genericMenu.GetItemCount() > 0 && (pasteElement || clearList))
//             //{
//             //    genericMenu.AddSeparator(null);
//             //}
//
//             var windowWidth = 300;
//             var rect = property.LastDrawnValueRect.AlignTop(1);
//             rect.y += 14;
//             rect.xMin += rect.width * 0.5f - (windowWidth * 0.5f);
//             rect.position = GUIUtility.GUIToScreenPoint(rect.position);
//             rect.width = 1;
//
//             if (setCollectionLength)
//             {
//                 if (this.info.GetCustomAddFunctionVoid != null)
//                 {
//                     genericMenu.AddDisabledItem(new GUIContent("Set Collection Size - disabled by 'void " + info.CustomListDrawerOptions.CustomAddFunction + "'"));
//                 }
//                 else
//                 {
//                     genericMenu.AddItem(new GUIContent("Set Collection Size"), false, () =>
//                     {
//                         EditorWindow window = null;
//
//                         Action cancel = () =>
//                         {
//                             EditorApplication.delayCall += window.Close;
//                         };
//
//                         Action<int> confirm = (size) =>
//                         {
//                             EditorApplication.delayCall += window.Close;
//                             SetCollectionSize(property, size);
//                         };
//
//                         var sizer = new CollectionSizeDialogue(confirm, cancel, property.ChildResolver.MaxChildCountSeen);
//                         window = OdinEditorWindow.InspectObjectInDropDown(sizer, rect, windowWidth);
//                         GUIHelper.RequestRepaint();
//                     });
//                 }
//             }
//
//             if (pasteElement)
//             {
//                 genericMenu.AddItem(new GUIContent("Paste Element"), false, () =>
//                 {
//                     (property.ChildResolver as ICollectionResolver).QueueAdd(new object[] { Clipboard.Paste() });
//                     GUIHelper.RequestRepaint();
//                 });
//             }
//
//             if (clearList)
//             {
//                 genericMenu.AddSeparator("");
//                 genericMenu.AddItem(new GUIContent("Clear Collection"), false, () =>
//                 {
//                     (property.ChildResolver as ICollectionResolver).QueueClear();
//                     GUIHelper.RequestRepaint();
//                 });
//             }
//             else
//             {
//                 genericMenu.AddSeparator("");
//                 genericMenu.AddDisabledItem(new GUIContent("Clear Collection"));
//             }
//         }
//
//         private void SetCollectionSize(InspectorProperty p, int targetSize)
//         {
//             var resolver = p.ChildResolver as IOrderedCollectionResolver;
//
//             for (int i = 0; i < p.ParentValues.Count; i++)
//             {
//                 var collection = p.ValueEntry.WeakValues[i] as IList;
//                 var size = collection.Count;
//                 var delta = Math.Abs(targetSize - size);
//
//                 if (targetSize > size)
//                 {
//                     for (int j = 0; j < delta; j++)
//                     {
//                         var value = this.GetValueToAdd(i);
//                         resolver.QueueAdd(value, i);
//                     }
//                 }
//                 else
//                 {
//                     for (int j = 0; j < delta; j++)
//                     {
//                         resolver.QueueRemoveAt(size - (1 + j), i);
//                     }
//                 }
//             }
//         }
//
//         private object GetValueToAdd(int selectionIndex)
//         {
//             bool wasFallback;
//             return this.GetValueToAdd(selectionIndex, out wasFallback);
//         }
//
//         private object GetValueToAdd(int selectionIndex, out bool wasFallback)
//         {
//             wasFallback = false;
//
//             if (this.info.GetCustomAddFunction != null)
//             {
//                 return this.info.GetCustomAddFunction(this.info.Property.ParentValues[selectionIndex]);
//             }
//             else if (this.info.CustomListDrawerOptions.AlwaysAddDefaultValue)
//             {
//                 if (this.info.Property.ValueEntry.SerializationBackend == SerializationBackend.Unity)
//                 {
//                     return UnitySerializationUtility.CreateDefaultUnityInitializedObject(this.info.CollectionResolver.ElementType);
//                 }
//                 else if (this.info.CollectionResolver.ElementType.IsValueType)
//                 {
//                     return Activator.CreateInstance(this.info.CollectionResolver.ElementType);
//                 }
//                 else
//                 {
//                     return null;
//                 }
//             }
//             else if (this.info.CustomListDrawerOptions.AddCopiesLastElement && this.info.Count > 0)
//             {
//                 object lastObject = null;
//                 var lastElementProperty = this.info.Property.Children.Last().ValueEntry;
//
//                 var collection = this.info.Property.ValueEntry.WeakValues[selectionIndex] as IEnumerable;
//                 if (collection != null)
//                 {
//                     // Yes, it's intended.
//                     foreach (var item in collection)
//                     {
//                         lastObject = item;
//                     }
//                 }
//                 else
//                 {
//                     lastObject = lastElementProperty.WeakValues[selectionIndex];
//                 }
//
//                 return Sirenix.Serialization.SerializationUtility.CreateCopy(lastObject);
//             }
//             else if (this.info.CollectionResolver.ElementType.InheritsFrom<UnityEngine.Object>() && Event.current.modifiers == EventModifiers.Control)
//             {
//                 return null;
//             }
//
//             wasFallback = true;
//             var elementType = (this.Property.ChildResolver as ICollectionResolver).ElementType;
//             if (this.ValueEntry.SerializationBackend == SerializationBackend.Unity)
//             {
//                 return UnitySerializationUtility.CreateDefaultUnityInitializedObject(elementType);
//             }
//             else
//             {
//                 return elementType.IsValueType ? Activator.CreateInstance(elementType) : null;
//             }
//         }
//
//         /// <summary>
//         /// Initializes the drawer.
//         /// </summary>
//         protected override void Initialize()
//         {
//             var resolver = this.Property.ChildResolver as ICollectionResolver;
//             bool isReadOnly = resolver.IsReadOnly;
//
//             var customListDrawerOptions = this.Property.GetAttribute<ListDrawerSettingsAttribute>() ?? new ListDrawerSettingsAttribute();
//             isReadOnly = this.ValueEntry.IsEditable == false || isReadOnly || customListDrawerOptions.IsReadOnlyHasValue && customListDrawerOptions.IsReadOnly;
//
//             this.info = new ListDrawerConfigInfo()
//             {
//                 StartIndex = 0,
//                 Toggled = this.ValueEntry.Context.GetPersistent<bool>(this, "ListDrawerToggled", customListDrawerOptions.DefaultExpandedStateHasValue ? customListDrawerOptions.DefaultExpandedState : GeneralDrawerConfig.Instance.OpenListsByDefault),
//                 RemoveAt = -1,
//
//                 // Now set further down, so it can be kept updated every frame
//                 //Label = new GUIContent(label == null || string.IsNullOrEmpty(label.text) ? this.Property.ValueEntry.TypeOfValue.GetNiceName() : label.text, label == null ? string.Empty : label.tooltip),
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
//             this.info.Property = this.Property;
//
//             if (customListDrawerOptions.DraggableHasValue && !customListDrawerOptions.DraggableItems)
//             {
//                 this.info.Draggable = false;
//             }
//
//             if (!(this.Property.ChildResolver is IOrderedCollectionResolver))
//             {
//                 this.info.Draggable = false;
//             }
//
//             if (this.info.CustomListDrawerOptions.OnBeginListElementGUI != null)
//             {
//                 string error;
//                 MemberInfo memberInfo = this.Property.ParentType
//                     .FindMember()
//                     .IsMethod()
//                     .IsNamed(this.info.CustomListDrawerOptions.OnBeginListElementGUI)
//                     .HasParameters<int>()
//                     .ReturnsVoid()
//                     .GetMember<MethodInfo>(out error);
//
//                 if (memberInfo == null || error != null)
//                 {
//                     // TOOD: Do something about this "There should really be an error message here." thing.
//                     // For this to trigger both the member and the error message should be null. Can that happen?
//                     this.errorMessage = error ?? "There should really be an error message here.";
//                 }
//                 else
//                 {
//                     this.info.OnBeginListElementGUI = EmitUtilities.CreateWeakInstanceMethodCaller<int>(memberInfo as MethodInfo);
//                 }
//             }
//
//             if (this.info.CustomListDrawerOptions.OnEndListElementGUI != null)
//             {
//                 string error;
//                 MemberInfo memberInfo = this.Property.ParentType
//                     .FindMember()
//                     .IsMethod()
//                     .IsNamed(this.info.CustomListDrawerOptions.OnEndListElementGUI)
//                     .HasParameters<int>()
//                     .ReturnsVoid()
//                     .GetMember<MethodInfo>(out error);
//
//                 if (memberInfo == null || error != null)
//                 {
//                     // TOOD: Do something about this "There should really be an error message here." thing.
//                     // For this to trigger both the member and the error message should be null. Can that happen?
//                     this.errorMessage = error ?? "There should really be an error message here.";
//                 }
//                 else
//                 {
//                     this.info.OnEndListElementGUI = EmitUtilities.CreateWeakInstanceMethodCaller<int>(memberInfo as MethodInfo);
//                 }
//             }
//
//             if (this.info.CustomListDrawerOptions.OnTitleBarGUI != null)
//             {
//                 string error;
//                 MemberInfo memberInfo = this.Property.ParentType
//                     .FindMember()
//                     .IsMethod()
//                     .IsNamed(this.info.CustomListDrawerOptions.OnTitleBarGUI)
//                     .HasNoParameters()
//                     .ReturnsVoid()
//                     .GetMember<MethodInfo>(out error);
//
//                 if (memberInfo == null || error != null)
//                 {
//                     // TOOD: Do something about this "There should really be an error message here." thing.
//                     // For this to trigger both the member and the error message should be null. Can that happen?
//                     this.errorMessage = error ?? "There should really be an error message here.";
//                 }
//                 else
//                 {
//                     this.info.OnTitleBarGUI = EmitUtilities.CreateWeakInstanceMethodCaller(memberInfo as MethodInfo);
//                 }
//             }
//
//             if (this.info.CustomListDrawerOptions.ListElementLabelName != null)
//             {
//                 string error;
//                 MemberInfo memberInfo = resolver.ElementType
//                     .FindMember()
//                     .HasNoParameters()
//                     .IsNamed(this.info.CustomListDrawerOptions.ListElementLabelName)
//                     .HasReturnType<object>(true)
//                     .GetMember(out error);
//
//                 if (memberInfo == null || error != null)
//                 {
//                     // TOOD: Do something about this "There should really be an error message here." thing.
//                     // For this to trigger both the member and the error message should be null. Can that happen?
//                     this.errorMessage = error ?? "There should really be an error message here.";
//                 }
//                 else
//                 {
//                     string methodSuffix = memberInfo as MethodInfo == null ? "" : "()";
//                     this.info.GetListElementLabelText = DeepReflection.CreateWeakInstanceValueGetter(resolver.ElementType, typeof(object), this.info.CustomListDrawerOptions.ListElementLabelName + methodSuffix);
//                 }
//             }
//
//             // Resolve custom add method member reference.
//             if (this.info.CustomListDrawerOptions.CustomAddFunction != null)
//             {
//                 string error;
//                 MemberInfo memberInfo = this.Property.ParentType
//                     .FindMember()
//                     .HasNoParameters()
//                     .IsNamed(this.info.CustomListDrawerOptions.CustomAddFunction)
//                     .IsInstance()
//                     .HasReturnType(resolver.ElementType)
//                     .GetMember(out error);
//
//                 if (memberInfo == null || error != null)
//                 {
//                     string error2;
//
//                     memberInfo = this.Property.ParentType
//                        .FindMember()
//                        .IsMethod()
//                        .HasNoParameters()
//                        .IsNamed(this.info.CustomListDrawerOptions.CustomAddFunction)
//                        .IsInstance()
//                        .ReturnsVoid()
//                        .GetMember(out error2);
//
//                     if (memberInfo == null || error2 != null)
//                     {
//                         this.errorMessage = error + " - or - " + error2;
//                     }
//                     else
//                     {
//                         this.info.GetCustomAddFunctionVoid = EmitUtilities.CreateWeakInstanceMethodCaller(memberInfo as MethodInfo);
//                     }
//                 }
//                 else
//                 {
//                     string methodSuffix = memberInfo as MethodInfo == null ? "" : "()";
//                     this.info.GetCustomAddFunction = DeepReflection.CreateWeakInstanceValueGetter(
//                         this.Property.ParentType,
//                         resolver.ElementType,
//                         this.info.CustomListDrawerOptions.CustomAddFunction + methodSuffix
//                     );
//                 }
//             }
//
//             // Resolve custom remove index method member reference.
//             if (this.info.CustomListDrawerOptions.CustomRemoveIndexFunction != null)
//             {
//                 if (this.Property.ChildResolver is IOrderedCollectionResolver == false)
//                 {
//                     this.errorMessage = "ListDrawerSettings.CustomRemoveIndexFunction is invalid on unordered collections. Use ListDrawerSetings.CustomRemoveElementFunction instead.";
//                 }
//                 else
//                 {
//                     MethodInfo method = this.Property.ParentType
//                         .FindMember()
//                         .IsNamed(this.info.CustomListDrawerOptions.CustomRemoveIndexFunction)
//                         .IsMethod()
//                         .IsInstance()
//                         .HasParameters<int>()
//                         .ReturnsVoid()
//                         .GetMember<MethodInfo>(out this.errorMessage);
//
//                     if (method != null)
//                     {
//                         this.info.CustomRemoveIndexFunction = EmitUtilities.CreateWeakInstanceMethodCaller<int>(method);
//                     }
//                 }
//             }
//             // Resolve custom remove element method member reference.
//             else if (this.info.CustomListDrawerOptions.CustomRemoveElementFunction != null)
//             {
//                 var element = (this.Property.ChildResolver as ICollectionResolver).ElementType;
//
//                 MethodInfo method = this.Property.ParentType
//                     .FindMember()
//                     .IsNamed(this.info.CustomListDrawerOptions.CustomRemoveElementFunction)
//                     .IsMethod()
//                     .IsInstance()
//                     .HasParameters(element)
//                     .ReturnsVoid()
//                     .GetMember<MethodInfo>(out this.errorMessage);
//
//                 if (method != null)
//                 {
//                     // TOOD: Emit dis.
//                     this.info.CustomRemoveElementFunction = (o, e) => method.Invoke(o, new object[] { e });
//                 }
//             }
//         }
//
//         /// <summary>
//         /// Draws the property.
//         /// </summary>
//         protected override void DrawPropertyLayout(GUIContent label)
//         {
//             var resolver = this.Property.ChildResolver as ICollectionResolver;
//             bool isReadOnly = resolver.IsReadOnly;
//
//             if (this.errorMessage != null)
//             {
//                 SirenixEditorGUI.ErrorMessageBox(this.errorMessage);
//             }
//
//             if (this.info.Label == null || (label != null && label.text != this.info.Label.text))
//             {
//                 this.info.Label = new GUIContent(label == null || string.IsNullOrEmpty(label.text) ? this.Property.ValueEntry.TypeOfValue.GetNiceName() : label.text, label == null ? string.Empty : label.tooltip);
//             }
//
//             this.info.IsReadOnly = resolver.IsReadOnly;
//
//             this.info.ListItemStyle.padding.left = this.info.Draggable ? 25 : 7;
//             this.info.ListItemStyle.padding.right = this.info.IsReadOnly || this.info.HideRemoveButton ? 4 : 20;
//
//             if (Event.current.type == EventType.Repaint)
//             {
//                 this.info.DropZoneTopLeft = GUIUtility.GUIToScreenPoint(new Vector2(0, 0));
//             }
//
//             this.info.CollectionResolver = this.Property.ChildResolver as ICollectionResolver;
//             this.info.OrderedCollectionResolver = this.Property.ChildResolver as IOrderedCollectionResolver;
//             this.info.Count = this.Property.Children.Count;
//             this.info.IsEmpty = this.Property.Children.Count == 0;
//
//             SirenixEditorGUI.BeginIndentedVertical(SirenixGUIStyles.PropertyPadding);
//             this.BeginDropZone();
//             {
//                 this.DrawToolbar();
//                 if (SirenixEditorGUI.BeginFadeGroup(UniqueDrawerKey.Create(this.Property, this), this.info.Toggled.Value))
//                 {
//                     GUIHelper.PushLabelWidth(GUIHelper.BetterLabelWidth - this.info.ListItemStyle.padding.left);
//                     this.DrawItems();
//                     GUIHelper.PopLabelWidth();
//                 }
//                 SirenixEditorGUI.EndFadeGroup();
//             }
//             this.EndDropZone();
//             SirenixEditorGUI.EndIndentedVertical();
//
//             if (this.info.OrderedCollectionResolver != null)
//             {
//                 if (this.info.RemoveAt >= 0 && Event.current.type == EventType.Repaint)
//                 {
//                     try
//                     {
//                         if (this.info.CustomRemoveIndexFunction != null)
//                         {
//                             foreach (var parent in this.Property.ParentValues)
//                             {
//                                 this.info.CustomRemoveIndexFunction(
//                                     parent,
//                                     this.info.RemoveAt);
//                             }
//                         }
//                         else if (this.info.CustomRemoveElementFunction != null)
//                         {
//                             for (int i = 0; i < this.Property.ParentValues.Count; i++)
//                             {
//                                 this.info.CustomRemoveElementFunction(
//                                     this.Property.ParentValues[i],
//                                     this.Property.Children[this.info.RemoveAt].ValueEntry.WeakValues[i]);
//                             }
//                         }
//                         else
//                         {
//                             this.info.OrderedCollectionResolver.QueueRemoveAt(this.info.RemoveAt);
//                         }
//                     }
//                     finally
//                     {
//
//                         this.info.RemoveAt = -1;
//                     }
//
//                     GUIHelper.RequestRepaint();
//                 }
//             }
//             else if (this.info.RemoveValues != null && Event.current.type == EventType.Repaint)
//             {
//                 try
//                 {
//                     if (this.info.CustomRemoveElementFunction != null)
//                     {
//                         for (int i = 0; i < this.Property.ParentValues.Count; i++)
//                         {
//                             this.info.CustomRemoveElementFunction(
//                                 this.Property.ParentValues[i],
//                                 this.info.RemoveValues[i]);
//                         }
//                     }
//                     else
//                     {
//                         this.info.CollectionResolver.QueueRemove(this.info.RemoveValues);
//                     }
//                 }
//                 finally
//                 {
//
//                     this.info.RemoveValues = null;
//                 }
//                 GUIHelper.RequestRepaint();
//             }
//
//             if (this.info.ObjectPicker != null && this.info.ObjectPicker.IsReadyToClaim && Event.current.type == EventType.Repaint)
//             {
//                 var value = this.info.ObjectPicker.ClaimObject();
//
//                 if (this.info.JumpToNextPageOnAdd)
//                 {
//                     this.info.StartIndex = int.MaxValue;
//                 }
//
//                 object[] values = new object[this.info.Property.Tree.WeakTargets.Count];
//
//                 values[0] = value;
//                 for (int j = 1; j < values.Length; j++)
//                 {
//                     values[j] = Sirenix.Serialization.SerializationUtility.CreateCopy(value);
//                 }
//
//                 this.info.CollectionResolver.QueueAdd(values);
//             }
//         }
//
//         private DropZoneHandle BeginDropZone()
//         {
//             if (this.info.OrderedCollectionResolver == null) return null;
//
//             var dropZone = DragAndDropManager.BeginDropZone(this.info.Property.Tree.GetHashCode() + "-" + this.info.Property.Path, this.info.CollectionResolver.ElementType, true);
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
//             if ((eventType == EventType.DragUpdated || eventType == EventType.DragPerform) && info.DropZone.Rect.Contains(Event.current.mousePosition))
//             {
//                 UnityEngine.Object[] objReferences = null;
//
//                 if (DragAndDrop.objectReferences.Any(n => n != null && info.CollectionResolver.ElementType.IsAssignableFrom(n.GetType())))
//                 {
//                     objReferences = DragAndDrop.objectReferences.Where(x => x != null && info.CollectionResolver.ElementType.IsAssignableFrom(x.GetType())).Reverse().ToArray();
//                 }
//                 else if (info.CollectionResolver.ElementType.InheritsFrom(typeof(Component)))
//                 {
//                     objReferences = DragAndDrop.objectReferences.OfType<GameObject>().Select(x => x.GetComponent(info.CollectionResolver.ElementType)).Where(x => x != null).Reverse().ToArray();
//                 }
//                 else if (info.CollectionResolver.ElementType.InheritsFrom(typeof(Sprite)) && DragAndDrop.objectReferences.Any(n => n is Texture2D && AssetDatabase.Contains(n)))
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
//             if (eventType == EventType.Repaint)
//             {
//                 info.IsDroppingUnityObjects = info.IsAboutToDroppingUnityObjects;
//             }
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
//                         this.info.OrderedCollectionResolver.QueueInsertAt(Mathf.Clamp(this.info.InsertAt, 0, this.info.Property.Children.Count), values);
//                     };
//                 }
//                 else
//                 {
//                     this.info.OrderedCollectionResolver.QueueInsertAt(Mathf.Clamp(this.info.InsertAt, 0, this.info.Property.Children.Count), values);
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
//                         this.info.OrderedCollectionResolver.QueueInsertAt(Mathf.Clamp(this.info.InsertAt, 0, this.info.Property.Children.Count), values);
//                     }
//                 }
//             }
//             DragAndDropManager.EndDropZone();
//         }
//
//         private void DrawToolbar()
//         {
//             SirenixEditorGUI.BeginHorizontalToolbar();
//             {
//                 // Label
//                 if (this.info.DropZone != null && DragAndDropManager.IsDragInProgress && this.info.DropZone.IsAccepted == false)
//                 {
//                     GUIHelper.PushGUIEnabled(false);
//                 }
//
//                 if (this.info.Property.ValueEntry.ListLengthChangedFromPrefab)
//                 {
//                     GUIHelper.PushIsBoldLabel(true);
//                 }
//
//                 if (this.info.ListConfig.HideFoldoutWhileEmpty && this.info.IsEmpty || this.info.CustomListDrawerOptions.Expanded)
//                 {
//                     GUILayout.Label(this.info.Label, GUILayoutOptions.ExpandWidth(false));
//                 }
//                 else
//                 {
//                     this.info.Toggled.Value = SirenixEditorGUI.Foldout(this.info.Toggled.Value, this.info.Label ?? GUIContent.none);
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
//                 if (this.info.DropZone != null && DragAndDropManager.IsDragInProgress && this.info.DropZone.IsAccepted == false)
//                 {
//                     GUIHelper.PopGUIEnabled();
//                 }
//
//                 GUILayout.FlexibleSpace();
//
//                 // Item Count
//                 if (this.info.CustomListDrawerOptions.ShowItemCountHasValue ? this.info.CustomListDrawerOptions.ShowItemCount : this.info.ListConfig.ShowItemCount)
//                 {
//                     if (this.info.Property.ValueEntry.ValueState == PropertyValueState.CollectionLengthConflict)
//                     {
//                         GUILayout.Label(this.info.Count + " / " + this.info.CollectionResolver.MaxCollectionLength + " items", EditorStyles.centeredGreyMiniLabel);
//                     }
//                     else
//                     {
//                         GUILayout.Label(this.info.IsEmpty ? "Empty" : this.info.Count + " items", EditorStyles.centeredGreyMiniLabel);
//                     }
//                 }
//
//                 bool paging = this.info.CustomListDrawerOptions.PagingHasValue ? this.info.CustomListDrawerOptions.ShowPaging : true;
//                 bool hidePaging =
//                         this.info.ListConfig.HidePagingWhileCollapsed && this.info.Toggled.Value == false ||
//                         this.info.ListConfig.HidePagingWhileOnlyOnePage && this.info.Count <= this.info.NumberOfItemsPerPage;
//
//                 int numberOfItemsPrPage = Math.Max(1, this.info.NumberOfItemsPerPage);
//                 int numberOfPages = Mathf.CeilToInt(this.info.Count / (float)numberOfItemsPrPage);
//                 int pageIndex = this.info.Count == 0 ? 0 : (this.info.StartIndex / numberOfItemsPrPage) % this.info.Count;
//
//                 // Paging
//                 if (paging)
//                 {
//                     bool disablePaging = paging && !hidePaging && (DragAndDropManager.IsDragInProgress || this.info.ShowAllWhilePaging || this.info.Toggled.Value == false);
//                     if (disablePaging)
//                     {
//                         GUIHelper.PushGUIEnabled(false);
//                     }
//
//                     if (!hidePaging)
//                     {
//                         if (pageIndex == 0) { GUIHelper.PushGUIEnabled(false); }
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
//                         if (pageIndex == 0) { GUIHelper.PopGUIEnabled(); }
//
//                         var userPageIndex = EditorGUILayout.IntField((numberOfPages == 0 ? 0 : (pageIndex + 1)), GUILayoutOptions.Width(10 + numberOfPages.ToString(CultureInfo.InvariantCulture).Length * 10)) - 1;
//                         if (pageIndex != userPageIndex)
//                         {
//                             this.info.StartIndex = userPageIndex * numberOfItemsPrPage;
//                         }
//
//                         GUILayout.Label("/ " + numberOfPages);
//
//                         if (pageIndex == numberOfPages - 1) { GUIHelper.PushGUIEnabled(false); }
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
//                         if (pageIndex == numberOfPages - 1) { GUIHelper.PopGUIEnabled(); }
//                     }
//
//                     pageIndex = this.info.Count == 0 ? 0 : (this.info.StartIndex / numberOfItemsPrPage) % this.info.Count;
//
//                     var newStartIndex = Mathf.Clamp(pageIndex * numberOfItemsPrPage, 0, Mathf.Max(0, this.info.Count - 1));
//                     if (newStartIndex != this.info.StartIndex)
//                     {
//                         this.info.StartIndex = newStartIndex;
//                         var newPageIndex = this.info.Count == 0 ? 0 : (this.info.StartIndex / numberOfItemsPrPage) % this.info.Count;
//                         if (pageIndex != newPageIndex)
//                         {
//                             pageIndex = newPageIndex;
//                             this.info.StartIndex = Mathf.Clamp(pageIndex * numberOfItemsPrPage, 0, Mathf.Max(0, this.info.Count - 1));
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
//                         if (SirenixEditorGUI.ToolbarButton(this.info.ShowAllWhilePaging ? EditorIcons.TriangleUp : EditorIcons.TriangleDown, true))
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
//                     this.info.ObjectPicker = ObjectPicker.GetObjectPicker(this.info, this.info.CollectionResolver.ElementType);
//                     var superHackyAddFunctionWeSeriouslyNeedANewListDrawer = CollectionDrawerStaticInfo.NextCustomAddFunction;
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
//                     this.info.JumpToNextPageOnAdd = paging && (this.info.Count % numberOfItemsPrPage == 0) && (pageIndex + 1 == numberOfPages);
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
//             bool paging = this.info.CustomListDrawerOptions.PagingHasValue ? this.info.CustomListDrawerOptions.ShowPaging : true;
//             if (paging && this.info.ShowAllWhilePaging == false)
//             {
//                 from = Mathf.Clamp(this.info.StartIndex, 0, this.info.Count);
//                 to = Mathf.Clamp(this.info.EndIndex, 0, this.info.Count);
//             }
//
//             var drawEmptySpace = this.info.DropZone != null && this.info.DropZone.IsBeingHovered || this.info.IsDroppingUnityObjects;
//             float height = drawEmptySpace ? this.info.IsDroppingUnityObjects ? 16 : (DragAndDropManager.CurrentDraggingHandle.Rect.height - 3) : 0;
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
//                             if (topHalf.Contains(this.info.LayoutMousePosition) || topHalf.y > this.info.LayoutMousePosition.y && i == 0)
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
//                             CollectionDrawerStaticInfo.DelayedGUIDrawer.Begin(dragHandle.Rect.width, dragHandle.Rect.height, dragHandle.CurrentMethod != DragAndDropMethods.Move);
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
//                             if (bottomHalf.Contains(this.info.LayoutMousePosition) || bottomHalf.yMax < this.info.LayoutMousePosition.y && j + 1 == to)
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
//                 if (to == this.info.Property.Children.Count && this.info.Property.ValueEntry.ValueState == PropertyValueState.CollectionLengthConflict)
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
//                         CollectionDrawerStaticInfo.DelayedGUIDrawer.Draw(Event.current.mousePosition - DragAndDropManager.CurrentDraggingHandle.MouseDownPostionOffset);
//                     }
//                 });
//             }
//         }
//
//         private DragHandle BeginDragHandle(int j, int i)
//         {
//             var child = this.info.Property.Children[j];
//             var dragHandle = DragAndDropManager.BeginDragHandle(child, child.ValueEntry.WeakSmartValue, this.info.IsReadOnly ? DragAndDropMethods.Reference : DragAndDropMethods.Move);
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
//                         if (dragHandle.IsCrossWindowDrag || (CollectionDrawerStaticInfo.CurrentDroppingPropertyInfo != null && CollectionDrawerStaticInfo.CurrentDroppingPropertyInfo.Tree != this.info.Property.Tree))
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
//                     listItemInfo.Value.DragHandleRect = new Rect(rect.x + 4, rect.y + 2 + ((int)rect.height - 23) / 2, 20, 20);
//                     listItemInfo.Value.RemoveBtnRect = new Rect(listItemInfo.Value.DragHandleRect.x + rect.width - 22, listItemInfo.Value.DragHandleRect.y + 1, 14, 14);
//
//                     if (this.info.HideRemoveButton == false)
//                     {
//
//                     }
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
//         private struct ListItemInfo
//         {
//             public float Width;
//             public Rect RemoveBtnRect;
//             public Rect DragHandleRect;
//         }
//
//         private class ListDrawerConfigInfo
//         {
//             public ICollectionResolver CollectionResolver;
//             public IOrderedCollectionResolver OrderedCollectionResolver;
//             public bool IsEmpty;
//             public ListDrawerSettingsAttribute CustomListDrawerOptions;
//             public int Count;
//             public LocalPersistentContext<bool> Toggled;
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
//             public InspectorProperty Property;
//             public GUIContent Label;
//             public bool IsAboutToDroppingUnityObjects;
//             public bool IsDroppingUnityObjects;
//             public bool HideAddButton;
//             public bool HideRemoveButton;
//
//             public Action<object> GetCustomAddFunctionVoid;
//             public Func<object, object> GetCustomAddFunction;
//
//             public Action<object, int> CustomRemoveIndexFunction;
//             public Action<object, object> CustomRemoveElementFunction;
//
//             public Func<object, object> GetListElementLabelText;
//             public Action<object, int> OnBeginListElementGUI;
//             public Action<object, int> OnEndListElementGUI;
//
//             public int NumberOfItemsPerPage
//             {
//                 get
//                 {
//                     return this.CustomListDrawerOptions.NumberOfItemsPerPageHasValue ? this.CustomListDrawerOptions.NumberOfItemsPerPage : this.ListConfig.NumberOfItemsPrPage;
//                 }
//             }
//
//             public GUIStyle ListItemStyle = new GUIStyle(GUIStyle.none)
//             {
//                 padding = new RectOffset(25, 20, 3, 3)
//             };
//         }
//     }
// }
