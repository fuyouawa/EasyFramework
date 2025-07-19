using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Core.Editor
{
    //TODO 版权问题
    public static class EasyGUIHelper
    {
        private static readonly GUIScopeStack<EventType> EventTypeStack = new GUIScopeStack<EventType>();
        private static readonly GUIScopeStack<Color> ColorStack = new GUIScopeStack<Color>();
        private static readonly GUIScopeStack<int> IndentLevelStack = new GUIScopeStack<int>();
        private static readonly GUIScopeStack<bool> HierarchyModeStack = new GUIScopeStack<bool>();

        private static readonly Func<Rect> TopLevelLayoutRectGetter;
        private static readonly Func<float> TopLevelLayoutMinHeightGetter;
        private static readonly Func<float> TopLevelLayoutMaxHeightGetter;
        private static readonly Func<float> ActualLabelWidthGetter;
        private static readonly Func<object> TopLevelLayoutGetter;
        private static readonly MethodInfo TopLevelLayoutCalcHeightMethod;
        private static int numberOfFramesToRepaint;

        static EasyGUIHelper()
        {
            TopLevelLayoutGetter =
                ReflectionUtility.CreateValueGetter<object>(typeof(GUILayoutUtility), "current.topLevel");
            TopLevelLayoutRectGetter =
                ReflectionUtility.CreateValueGetter<Rect>(typeof(GUILayoutUtility), "current.topLevel.rect");
            TopLevelLayoutMinHeightGetter =
                ReflectionUtility.CreateValueGetter<float>(typeof(GUILayoutUtility), "current.topLevel.minHeight");
            TopLevelLayoutMaxHeightGetter =
                ReflectionUtility.CreateValueGetter<float>(typeof(GUILayoutUtility), "current.topLevel.maxHeight");

            ActualLabelWidthGetter =
                ReflectionUtility.CreateValueGetter<float>(typeof(EditorGUIUtility), "s_LabelWidth");

            var layoutType = Type.GetType("UnityEngine.GUILayoutGroup, UnityEngine.IMGUIModule");
            TopLevelLayoutCalcHeightMethod = layoutType.GetMethod("CalcHeight");
        }

        public static void RemoveFocusControl()
        {
            GUIUtility.hotControl = 0;
            DragAndDrop.activeControlID = 0;
            GUIUtility.keyboardControl = 0;
        }
        
        public static void RequestRepaint()
        {
            numberOfFramesToRepaint = Math.Max(numberOfFramesToRepaint, 2);
        }

        public static void PushIndentLevel(int indentLevel)
        {
            IndentLevelStack.Push(EditorGUI.indentLevel);
            EditorGUI.indentLevel = indentLevel;
        }

        public static void PopIndentLevel()
        {
            EditorGUI.indentLevel = IndentLevelStack.Pop();
        }


        public static Rect GetCurrentLayoutRect()
        {
            return TopLevelLayoutRectGetter();
        }

        public static float GetCurrentLayoutMinHeight()
        {
            return TopLevelLayoutMinHeightGetter();
        }

        public static float GetCurrentLayoutMaxHeight()
        {
            return TopLevelLayoutMaxHeightGetter();
        }

        public static void CalculateCurrentLayoutHeight()
        {
            var layout = TopLevelLayoutGetter();
            TopLevelLayoutCalcHeightMethod.Invoke(layout, null);
        }

        public static void PushColor(Color color, bool blendAlpha = false)
        {
            ColorStack.Push(GUI.color);

            if (blendAlpha)
            {
                color.a = color.a * GUI.color.a;
            }

            GUI.color = color;
        }

        public static void PopColor()
        {
            GUI.color = ColorStack.Pop();
        }

        /// <summary>
        /// Ignores input on following GUI calls. Remember to end with <see cref="EndIgnoreInput"/>.
        /// </summary>
        public static void BeginIgnoreInput()
        {
            var e = Event.current.type;
            PushEventType(e == EventType.Layout || e == EventType.Repaint || e == EventType.Used
                ? e
                : EventType.Ignore);
        }

        /// <summary>
        /// Ends the ignore input started by <see cref="BeginIgnoreInput"/>.
        /// </summary>
        public static void EndIgnoreInput()
        {
            PopEventType();
        }

        /// <summary>
        /// Pushes the event type to the stack. Remember to pop with <see cref="PopEventType"/>.
        /// </summary>
        /// <param name="eventType">The type of event to push.</param>
        public static void PushEventType(EventType eventType)
        {
            EventTypeStack.Push(Event.current.type);
            Event.current.type = eventType;
        }

        /// <summary>
        /// Pops the event type pushed by <see cref="PopEventType"/>.
        /// </summary>
        public static void PopEventType()
        {
            Event.current.type = EventTypeStack.Pop();
        }


        private static readonly GUIContent s_tempContent = new GUIContent();

        internal static GUIContent TempContent(string text, string tooltip = null, Texture image = null)
        {
            s_tempContent.text = text;
            s_tempContent.image = image;
            s_tempContent.tooltip = tooltip;
            return s_tempContent;
        }
        
        
        /// <summary>
        /// Pushes the hierarchy mode to the stack. Remember to pop the state with <see cref="PopHierarchyMode"/>.
        /// </summary>
        /// <param name="hierarchyMode">The hierachy mode state to push.</param>
        /// <param name="preserveCurrentLabelWidth">Changing hierachy mode also changes how label-widths are calcualted. By default, we try to keep the current label width.</param>
        public static void PushHierarchyMode(bool hierarchyMode, bool preserveCurrentLabelWidth = true)
        {
            // var actualLabelWidth = ActualLabelWidth;
            // LabelWidthStack.Push(actualLabelWidth);
            // var currentLabelWidth = preserveCurrentLabelWidth ? GUIHelper.BetterLabelWidth : actualLabelWidth;
            HierarchyModeStack.Push(EditorGUIUtility.hierarchyMode);
            EditorGUIUtility.hierarchyMode = hierarchyMode;
            // GUIHelper.BetterLabelWidth = currentLabelWidth;
        }

        /// <summary>
        /// Pops the hierarchy mode pushed by <see cref="PushHierarchyMode"/>.
        /// </summary>
        public static void PopHierarchyMode()
        {
            EditorGUIUtility.hierarchyMode = HierarchyModeStack.Pop();
            // ActualLabelWidth = LabelWidthStack.Pop();
        }
    }
}
