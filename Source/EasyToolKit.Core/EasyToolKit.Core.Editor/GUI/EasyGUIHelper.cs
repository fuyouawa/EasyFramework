using System;
using System.Reflection;
using EasyToolKit.ThirdParty.OdinSerializer.Utilities;
using UnityEngine;

namespace EasyToolKit.Core.Editor
{
    //TODO 版权问题
    public static class EasyGUIHelper
    {
        private static readonly GUIScopeStack<EventType> EventTypeStack = new GUIScopeStack<EventType>();
        
        private static readonly Func<Rect> TopLevelLayoutRectGetter;
        private static readonly Func<float> TopLevelLayoutMinHeightGetter;
        private static readonly Func<float> TopLevelLayoutMaxHeightGetter;
        private static readonly Func<object> TopLevelLayoutGetter;
        private static readonly MethodInfo TopLevelLayoutCalcHeightMethod;

        static EasyGUIHelper()
        {
            TopLevelLayoutGetter = ReflectionUtility.CreateValueGetter<object>(typeof(GUILayoutUtility), "current.topLevel");
            TopLevelLayoutRectGetter = ReflectionUtility.CreateValueGetter<Rect>(typeof(GUILayoutUtility), "current.topLevel.rect");
            TopLevelLayoutMinHeightGetter = ReflectionUtility.CreateValueGetter<float>(typeof(GUILayoutUtility), "current.topLevel.minHeight");
            TopLevelLayoutMaxHeightGetter = ReflectionUtility.CreateValueGetter<float>(typeof(GUILayoutUtility), "current.topLevel.maxHeight");

            var layoutType = Type.GetType("UnityEngine.GUILayoutGroup, UnityEngine.IMGUIModule");
            TopLevelLayoutCalcHeightMethod = layoutType.GetMethod("CalcHeight");
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

        /// <summary>
        /// Ignores input on following GUI calls. Remember to end with <see cref="EndIgnoreInput"/>.
        /// </summary>
        public static void BeginIgnoreInput()
        {
            var e = Event.current.type;
            PushEventType(e == EventType.Layout || e == EventType.Repaint || e == EventType.Used ? e : EventType.Ignore);
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
    }
}
