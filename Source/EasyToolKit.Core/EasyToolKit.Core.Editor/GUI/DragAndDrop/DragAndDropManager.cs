// using System;
// using System.Linq;
// using UnityEditor;
// using UnityEngine;
//
// namespace EasyToolKit.Core.Editor
// {
//     public class DragAndDropManager
//     {        private const int SPECIAL_CONTROL_ID_START = 961999999;
//
//         private static bool currentDragIsMove;
//         private static int draggingId;
//         private static bool isAccepted;
//         private static object dropZoneObject;
//         private static object[] dragginObjects = new object[] { };
//         private static bool isDragging = false;
//         private static int hoveringAcceptedDropZone;
//         private static int specialDragAndDropControlId = SPECIAL_CONTROL_ID_START;
//         private static EventType prevEventType = EventType.Repaint;
//         private static Rect preventDropAreaRect;
//         private static Rect nextPreventDropAreaRect;
//
//         /// <summary>
//         /// Gets the position from where the last drag started from in screen space.
//         /// </summary>
//         public static Vector2 OnDragStartMouseScreenPos { get; private set; }
//
//         /// <summary>
//         /// Gets the delta position between the currrent mouse position and where the last drag originated from.
//         /// </summary>
//         public static Vector2 MouseDragOffset
//         {
//             get
//             {
//                 if (Event.current == null)
//                 {
//                     return Vector2.zero;
//                 }
//
//                 return GUIUtility.GUIToScreenPoint(Event.current.mousePosition) - OnDragStartMouseScreenPos;
//             }
//         }
//
//         /// <summary>
//         /// Gets the hovering accepted drop zone ID.
//         /// </summary>
//         public static int HoveringAcceptedDropZone { get { return hoveringAcceptedDropZone; } }
//
//         /// <summary>
//         /// Gets a value indicating whether an instance is currently being dragged.
//         /// </summary>
//         public static bool IsDragging
//         {
//             get
//             {
//                 switch (Event.current.rawType)
//                 {
//                     case EventType.MouseDown:
//                     case EventType.MouseUp:
//                     case EventType.MouseMove:
//                         isDragging = false;
//                         break;
//                     case EventType.MouseDrag:
//                     case EventType.DragUpdated:
//                     case EventType.DragPerform:
//                     case EventType.DragExited:
//                         isDragging = true;
//                         break;
//                     default:
//                         break;
//                 }
//
//                 return isDragging;
//             }
//         }
//
//         /// <summary>
//         /// Gets the currently dragging identifier.
//         /// </summary>
//         public static int CurrentDragId
//         {
//             get
//             {
//                 if (!IsDragging)
//                 {
//                     return 0;
//                 }
//
//                 return draggingId;
//             }
//         }
//
//         /// <summary>
//         /// Gets the current hovering drop zone identifier.
//         /// </summary>
//         public static int CurrentDropId
//         {
//             get
//             {
//                 if (!IsDragging)
//                 {
//                     return 0;
//                 }
//
//                 return hoveringAcceptedDropZone != 0 ? hoveringAcceptedDropZone : DragAndDrop.activeControlID;
//             }
//         }
//
//         /// <summary>
//         /// Gets a more percistent id for drag and drop.
//         /// </summary>
//         public static int GetDragAndDropId(Rect rect)
//         {
//             // Drag and Drop can sometimes cause the GUIUtility.ControlIds to mes up. 
//             // That is just the nature of drag and drop, and nothing we can really do about it.
//             GUIUtility.GetControlID(FocusType.Passive, rect);
//             if (Event.current.rawType != prevEventType && Event.current.rawType != EventType.Used)
//             {
//                 prevEventType = Event.current.rawType;
//                 specialDragAndDropControlId = SPECIAL_CONTROL_ID_START;
//             }
//
//             return specialDragAndDropControlId++;
//         }
//
//         /// <summary>
//         /// Draws a objectpicker button in the given rect. This one is designed to look good on top of DrawDropZone().
//         /// </summary>
//         public static object ObjectPickerZone(Rect rect, object value, Type type, bool allowSceneObjects, int id)
//         {
//             var btnId = GUIUtility.GetControlID(FocusType.Passive);
//             var objectPicker = ObjectPicker.GetObjectPicker(type.FullName + "+" + EasyGUIHelper.CurrentWindowInstanceID.ToString() +"+" + id, type);
//             var selectRect = rect.AlignBottom(15).AlignCenter(45);
//             var uObj = value as UnityEngine.Object;
//             selectRect.xMin = Mathf.Max(selectRect.xMin, rect.xMin);
//
//             var hide = IsDragging || Event.current.type == EventType.Repaint && !rect.Contains(Event.current.mousePosition);
//
//             if (hide)
//             {
//                 EasyGUIHelper.PushColor(new Color(0, 0, 0, 0));
//                 EasyGUIHelper.PushGUIEnabled(false);
//             }
//
//             bool hideInspectorBtn = !hide && !(uObj);
//
//             if (hideInspectorBtn)
//             {
//                 EasyGUIHelper.PushGUIEnabled(false);
//                 EasyGUIHelper.PushColor(new Color(0, 0, 0, 0));
//             }
//
//             var inspectBtn = rect.AlignRight(14);
//             inspectBtn.height = 14;
//             EasyEditorGUI.BeginDrawOpenInspector(inspectBtn, uObj, rect);
//             EasyEditorGUI.EndDrawOpenInspector(inspectBtn, uObj);
//
//             if (hideInspectorBtn)
//             {
//                 EasyGUIHelper.PopColor();
//                 EasyGUIHelper.PopGUIEnabled();
//             }
//
//             if (GUI.Button(selectRect, "select", SirenixGUIStyles.TagButton))
//             {
//                 EasyGUIHelper.RemoveFocusControl();
//                 objectPicker.ShowObjectPicker(value, allowSceneObjects, rect, false);
//                 Event.current.Use();
//             }
//
//             if (Event.current.keyCode == KeyCode.Return && Event.current.type == EventType.KeyDown && EditorGUIUtility.keyboardControl == id)
//             {
//                 objectPicker.ShowObjectPicker(value, allowSceneObjects, rect, false);
//                 Event.current.Use();
//             }
//
//             if (hide)
//             {
//                 EasyGUIHelper.PopColor();
//                 EasyGUIHelper.PopGUIEnabled();
//             }
//
//             if (objectPicker.IsReadyToClaim)
//             {
//                 EasyGUIHelper.RequestRepaint();
//                 GUI.changed = true;
//                 var newValue = objectPicker.ClaimObject();
//                 Event.current.Use();
//                 return newValue;
//             }
//
//             if (objectPicker.IsPickerOpen && typeof(UnityEngine.Object).IsAssignableFrom(type))
//             {
//                 return objectPicker.CurrentSelectedObject;
//             }
//
//             if (Event.current.keyCode == KeyCode.Delete && Event.current.type == EventType.KeyDown && EditorGUIUtility.keyboardControl == id)
//             {
//                 Event.current.Use();
//                 GUI.changed = true;
//                 return null;
//             }
//
//             if (uObj && Event.current.rawType == EventType.MouseUp && rect.Contains(Event.current.mousePosition) && Event.current.button == 0)
//             {
//                 // For components ping the attached game object instead, because then Unity can figure out to ping prefabs in the project window too.
//                 UnityEngine.Object pingObj = uObj;
//                 if (pingObj is Component)
//                 {
//                     pingObj = (pingObj as Component).gameObject;
//                 }
//
//                 EditorGUIUtility.PingObject(pingObj);
//             }
//
//             return value;
//         }
//
//         /// <summary>
//         /// Draws a objectpicker butter, in the given rect. This one is designed to look good on top of DrawDropZone().
//         /// </summary>
//         public static T ObjectPickerZone<T>(Rect rect, T value, bool allowSceneObjects, int id)
//         {
//             return (T)ObjectPickerZone(rect, value, typeof(T), allowSceneObjects, id);
//         }
//
//         /// <summary>
//         /// Draws the graphics for a DropZone.
//         /// </summary>
//         public static void DrawDropZone(Rect rect, object value, GUIContent label, int id)
//         {
//             bool isDragging = IsDragging;
//
//             if (Event.current.type == EventType.Repaint)
//             {
//                 var objectToPaint = value as UnityEngine.Object;
//                 var objectFieldThumb = EditorStyles.objectFieldThumb;
//                 var on = GUI.enabled && hoveringAcceptedDropZone == id && rect.Contains(Event.current.mousePosition) && isDragging;
//
//                 objectFieldThumb.Draw(rect, GUIContent.none, id, on);
//
//                 if (EditorGUI.showMixedValue)
//                 {
//                     GUI.Label(rect, EasyEditorGUI.MixedValueDashChar, EasyGUIStyles.LabelCentered);
//                 }
//                 else if (objectToPaint)
//                 {
//                     Texture image = null;
//
//                     var rt =  objectToPaint as RenderTexture;
//                     if (rt)
//                     {
//                         image = rt;
//                     }
//
//                     var img = objectToPaint as UnityEngine.UI.Image;
//                     if (img)
//                     {
//                         objectToPaint = img.sprite;
//                     }
//
//                     var rawImg = objectToPaint as UnityEngine.UI.RawImage;
//                     if (rawImg)
//                     {
//                         image = rawImg.texture;
//                     }
//
//                     if (image == null)
//                     {
//                         image = EasyGUIHelper.GetAssetThumbnail(objectToPaint, objectToPaint.GetType(), true);   
//                     }
//
//                     rect = rect.Padding(2);
//                     float size = Mathf.Min(rect.width, rect.height);
//
//                     EditorGUI.DrawTextureTransparent(rect.AlignCenter(size, size), image, ScaleMode.ScaleToFit);
//
//                     if (label != null)
//                     {
//                         rect = rect.AlignBottom(16);
//                         GUI.Label(rect, label, EditorStyles.label);
//                     }
//                 }
//             }
//         }
//
//         /// <summary>
//         /// A draggable zone for both Unity and non-unity objects.
//         /// </summary>
//         public static object DragAndDropZone(Rect rect, object value, Type type, bool allowMove, bool allowSwap)
//         {
//             var id = GetDragAndDropId(rect);
//             value = DropZone(rect, value, type, id);
//             value = DragZone(rect, value, type, allowMove, allowSwap, id);
//             return value;
//         }
//
//         /// <summary>
//         /// A drop zone area for bot Unity and non-unity objects.
//         /// </summary>
//         public static object DropZone(Rect rect, object value, Type type, bool allowSceneObjects, int id)
//         {
//             if (rect.Contains(Event.current.mousePosition))
//             {
//                 var t = Event.current.type;
//
//                 if (t == EventType.DragUpdated || t == EventType.DragPerform)
//                 {
//                     // This bit disables all dropzones inside the provided preventDropAreaRect.
//                     // 
//                     // RootNode1
//                     //    ChileNode1
//                     //    ChileNode2
//                     //       ChileNode2.1
//                     //       ChileNode2.2
//                     //    ChileNode3
//                     // RootNode2
//                     // 
//                     // If the RootNode has provided a preventDropAreaRect, then that means that the RootNode won't be able to be dragged into any of its child nodes.
//
//                     if (preventDropAreaRect.Contains(new Vector2(rect.x, rect.y)) && preventDropAreaRect.Contains(new Vector2(rect.xMax, rect.yMax)))
//                     {
//                         return value;
//                     }
//
//                     object obj = null;
//
//                     if (obj == null) obj = dragginObjects.Where(x => x != null && x.GetType().IsInheritsFrom(type)).FirstOrDefault();
//                     if (obj == null) obj = DragAndDrop.objectReferences.Where(x => x != null && x.GetType().IsInheritsFrom(type)).FirstOrDefault();
//
//                     if (type.IsInheritsFrom<Component>() || type.IsInterface)
//                     {
//                         if (obj == null) obj = dragginObjects.OfType<GameObject>().Where(x => x != null).Select(x => x.GetComponent(type)).Where(x => x != null).FirstOrDefault();
//                         if (obj == null) obj = DragAndDrop.objectReferences.OfType<GameObject>().Where(x => x != null).Select(x => x.GetComponent(type)).Where(x => x != null).FirstOrDefault();
//                     }
//
//                     bool acceptsDrag = obj != null;
//
//                     if (acceptsDrag && allowSceneObjects == false)
//                     {
//                         var uObj = obj as UnityEngine.Object;
//                         if (uObj != null)
//                         {
//                             if (typeof(Component).IsAssignableFrom(uObj.GetType()))
//                             {
//                                 uObj = ((Component)uObj).gameObject;
//                             }
//
//                             acceptsDrag = EditorUtility.IsPersistent(uObj);
//                         }
//                     }
//
//                     if (acceptsDrag)
//                     {
//                         hoveringAcceptedDropZone = id;
//                         bool move = Event.current.modifiers != EventModifiers.Control && draggingId != 0 && currentDragIsMove;
//                         if (move)
//                         {
//                             DragAndDrop.visualMode = DragAndDropVisualMode.Move;
//                         }
//                         else
//                         {
//                             DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
//                         }
//
//                         Event.current.Use();
//                         if (t == EventType.DragPerform)
//                         {
//                             if (!move)
//                             {
//                                 draggingId = 0;
//                                 //preventDropAreaRect = new Rect();
//                             }
//
//                             // Calling this here makes Unity crash on MacOS
//                             // DragAndDrop.objectReferences = new UnityEngine.Object[] { };
//                             DragAndDrop.AcceptDrag();
//                             GUI.changed = true;
//                             EasyGUIHelper.RemoveFocusControl();
//                             dragginObjects = new object[] { };
//                             currentDragIsMove = false;
//                             isAccepted = true;
//                             dropZoneObject = value;
//                             preventDropAreaRect = new Rect();
//                             DragAndDrop.activeControlID = 0;
//                             EasyGUIHelper.RequestRepaint();
//                             return obj;
//                         }
//                         else
//                         {
//                             DragAndDrop.activeControlID = id;
//                         }
//                     }
//                     else
//                     {
//                         hoveringAcceptedDropZone = 0;
//                         DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
//                     }
//                 }
//             }
//             else
//             {
//                 if (hoveringAcceptedDropZone == id)
//                 {
//                     hoveringAcceptedDropZone = 0;
//                 }
//             }
//
//             return value;
//         }
//
//         /// <summary>
//         /// A drop zone area for bot Unity and non-unity objects.
//         /// </summary>
//         public static object DropZone(Rect rect, object value, Type type, int id)
//         {
//             return DropZone(rect, value, type, true, id);
//         }
//
//         /// <summary>
//         /// A drop zone area for bot Unity and non-unity objects.
//         /// </summary>
//         public static object DropZone(Rect rect, object value, Type type)
//         {
//             var id = GetDragAndDropId(rect);
//             return DropZone(rect, value, type, id);
//         }
//
//         /// <summary>
//         /// A drop zone area for bot Unity and non-unity objects.
//         /// </summary>
//         public static object DropZone(Rect rect, object value, Type type, bool allowSceneObjects)
//         {
//             var id = GetDragAndDropId(rect);
//             return DropZone(rect, value, type, allowSceneObjects, id);
//         }
//
//         /// <summary>
//         /// A drop zone area for bot Unity and non-unity objects.
//         /// </summary>
//         public static T DropZone<T>(Rect rect, T value, bool allowSceneObjects, int id)
//         {
//             return (T)DropZone(rect, value, typeof(T), allowSceneObjects, id);
//         }
//
//         /// <summary>
//         /// A drop zone area for bot Unity and non-unity objects.
//         /// </summary>
//         public static T DropZone<T>(Rect rect, T value, int id)
//         {
//             return (T)DropZone(rect, value, typeof(T), id);
//         }
//
//         /// <summary>
//         /// A drop zone area for bot Unity and non-unity objects.
//         /// </summary>
//         public static T DropZone<T>(Rect rect, T value, bool allowSceneObjects)
//         {
//             var id = GetDragAndDropId(rect);
//             return (T)DropZone(rect, value, typeof(T), allowSceneObjects, id);
//         }
//
//         /// <summary>
//         /// A drop zone area for bot Unity and non-unity objects.
//         /// </summary>
//         public static T DropZone<T>(Rect rect, T value)
//         {
//             var id = GetDragAndDropId(rect);
//             return (T)DropZone(rect, value, typeof(T), id);
//         }
//
//         /// <summary>
//         /// Disalloweds the drop area for next drag zone. Follow this function call by a DragZone.
//         /// </summary>
//         public static void DisallowedDropAreaForNextDragZone(Rect rect)
//         {
//             nextPreventDropAreaRect = rect;
//         }
//
//         /// <summary>
//         /// A draggable zone for both Unity and non-unity objects.
//         /// </summary>
//         public static object DragZone(Rect rect, object value, Type type, bool allowMove, bool allowSwap, int id)
//         {
//             var tmpNextPreventedDropArea = nextPreventDropAreaRect;
//             nextPreventDropAreaRect = new Rect();
//
//             if (value == null) return null;
//
//             // Unity null
//             if (!(value as UnityEngine.Object) && typeof(UnityEngine.Object).IsAssignableFrom(value.GetType()))
//             {
//                 return value;
//             }
//
//             var t = Event.current.type;
//             var isMouseOver = rect.Contains(Event.current.mousePosition);
//             var unityObject = value as UnityEngine.Object;
//
//             if (isMouseOver && t == EventType.MouseDown)
//             {
//                 EasyGUIHelper.RemoveFocusControl();
//                 GUIUtility.hotControl = id;
//                 GUIUtility.keyboardControl = id;
//                 dragginObjects = new object[] { };
//                 DragAndDrop.PrepareStartDrag();
//                 EasyGUIHelper.RequestRepaint();
//                 isAccepted = false;
//                 dropZoneObject = null;
//                 draggingId = 0;
//                 preventDropAreaRect = new Rect();
//                 currentDragIsMove = false;
//                 Event.current.Use();
//             }
//
//             if (isAccepted && draggingId == id)
//             {
//                 EasyGUIHelper.RequestRepaint();
//                 GUI.changed = true;
//                 draggingId = 0;
//                 preventDropAreaRect = new Rect();
//
//                 // TODO: Validate drop zone object, and only return that if it's assignable from type.
//
//                 return allowMove ? (allowSwap ? dropZoneObject : null) : value;
//             }
//
//             if (GUIUtility.hotControl != id)
//             {
//                 return value;
//             }
//             else if (t == EventType.MouseMove)
//             {
//                 EasyGUIHelper.RequestRepaint();
//                 draggingId = 0;
//                 preventDropAreaRect = new Rect();
//                 DragAndDrop.PrepareStartDrag();
//                 DragAndDrop.objectReferences = new UnityEngine.Object[] { };
//                 EasyGUIHelper.RemoveFocusControl();
//                 dragginObjects = new object[] { };
//                 currentDragIsMove = false;
//             }
//
//             if (Event.current.type == EventType.MouseDrag && isMouseOver && (DragAndDrop.objectReferences == null || DragAndDrop.objectReferences.Length == 0))
//             {
//                 isAccepted = false;
//                 dropZoneObject = null;
//                 draggingId = id;
//                 OnDragStartMouseScreenPos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
//                 DragAndDrop.StartDrag("Movable drag");
//                 preventDropAreaRect = tmpNextPreventedDropArea.Expand(1);
//                 if (unityObject)
//                 {
//                     DragAndDrop.objectReferences = new UnityEngine.Object[] { unityObject };
//                     dragginObjects = new object[] { };
//                 }
//                 else
//                 {
//                     DragAndDrop.objectReferences = new UnityEngine.Object[] { };
//                     dragginObjects = new object[] { value };
//                 }
//
//                 DragAndDrop.activeControlID = 0;
//                 currentDragIsMove = allowMove;
//                 Event.current.Use();
//                 EasyGUIHelper.RequestRepaint();
//             }
//
//             return value;
//         }
//
//         /// <summary>
//         /// A draggable zone for both Unity and non-unity objects.
//         /// </summary>
//         public static object DragZone(Rect rect, object value, Type type, bool allowMove, bool allowSwap)
//         {
//             var id = GetDragAndDropId(rect);
//             return DragZone(rect, value, type, allowMove, allowSwap, id);
//         }
//
//         /// <summary>
//         /// A draggable zone for both Unity and non-unity objects.
//         /// </summary>
//         public static T DragZone<T>(Rect rect, T value, bool allowMove, bool allowSwap, int id)
//         {
//             return (T)DragZone(rect, value, typeof(T), allowMove, allowSwap, id);
//         }
//
//         /// <summary>
//         /// A draggable zone for both Unity and non-unity objects.
//         /// </summary>
//         public static T DragZone<T>(Rect rect, T value, bool allowMove, bool allowSwap)
//         {
//             var id = GetDragAndDropId(rect);
//             return (T)DragZone(rect, value, typeof(T), allowMove, allowSwap, id);
//         }
//     }
// }
