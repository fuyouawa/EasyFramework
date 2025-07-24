using System;
using System.Collections.Generic;
using System.Reflection;
using EasyToolKit.ThirdParty.OdinSerializer.Utilities;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Core.Editor
{
    //TODO 版权问题
    public static class EasyGUIHelper
    {
        private static readonly GUIScopeStack<bool> GUIEnabledStack = new GUIScopeStack<bool>();
        private static readonly GUIScopeStack<EventType> EventTypeStack = new GUIScopeStack<EventType>();
        private static readonly GUIScopeStack<Color> ColorStack = new GUIScopeStack<Color>();
        private static readonly GUIScopeStack<int> IndentLevelStack = new GUIScopeStack<int>();
        private static readonly GUIScopeStack<float> LabelWidthStack = new GUIScopeStack<float>();
        private static readonly GUIScopeStack<bool> HierarchyModeStack = new GUIScopeStack<bool>();
        private static readonly GUIScopeStack<float> ContextWidthStackOdinVersion = new GUIScopeStack<float>();
        private static readonly GUIScopeStack<Color> LabelColorStack = new GUIScopeStack<Color>();

        private static readonly Func<Rect> TopLevelLayoutRectGetter;
        private static readonly Func<float> TopLevelLayoutMinHeightGetter;
        private static readonly Func<float> TopLevelLayoutMaxHeightGetter;
        private static readonly Func<float> ActualLabelWidthGetter;
        private static readonly Func<object> TopLevelLayoutGetter;
        private static readonly MethodInfo TopLevelLayoutCalcHeightMethod;
        private static readonly Func<float> ContextWidthGetter;
        private static readonly Action<float> ContextWidthSetter;
        private static readonly Func<Stack<float>> ContextWidthStackGetter;
        private static int numberOfFramesToRepaint;
        private static float betterContextWidth;

        static EasyGUIHelper()
        {
            TopLevelLayoutGetter =
                ReflectionUtility.CreateStaticValueGetter<object>(typeof(GUILayoutUtility), "current.topLevel");
            TopLevelLayoutRectGetter =
                ReflectionUtility.CreateStaticValueGetter<Rect>(typeof(GUILayoutUtility), "current.topLevel.rect");
            TopLevelLayoutMinHeightGetter =
                ReflectionUtility.CreateStaticValueGetter<float>(typeof(GUILayoutUtility), "current.topLevel.minHeight");
            TopLevelLayoutMaxHeightGetter =
                ReflectionUtility.CreateStaticValueGetter<float>(typeof(GUILayoutUtility), "current.topLevel.maxHeight");
            
            ContextWidthGetter = ReflectionUtility.CreateStaticValueGetter<float>(typeof(EditorGUIUtility), "contextWidth");
            FieldInfo field = typeof (EditorGUIUtility).GetField("s_ContextWidth", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            if (field != null)
                ContextWidthSetter = EmitUtilities.CreateStaticFieldSetter<float>(field);
            else
                ContextWidthStackGetter = EmitUtilities.CreateStaticFieldGetter<Stack<float>>(typeof (EditorGUIUtility).GetField("s_ContextWidthStack", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy));

            ActualLabelWidthGetter =
                ReflectionUtility.CreateStaticValueGetter<float>(typeof(EditorGUIUtility), "s_LabelWidth");

            var layoutType = Type.GetType("UnityEngine.GUILayoutGroup, UnityEngine.IMGUIModule");
            TopLevelLayoutCalcHeightMethod = layoutType.GetMethod("CalcHeight");
        }

        /// <summary>
        /// Gets the current indent amount.
        /// </summary>
        /// <value>
        /// The current indent amount.
        /// </value>
        public static float CurrentIndentAmount
        {
            get
            {
                return EditorGUI.indentLevel * 15;
            }
        }

        /// <summary>
        /// Gets the current editor gui context width. Only set these if you know what it does.
        /// </summary>
        public static float ContextWidth => ContextWidthGetter();


        /// <summary>
        /// Unity EditorGUIUtility.labelWidth only works reliablly in Repaint events.
        /// BetterLabelWidth does a better job at giving you the correct LabelWidth in non-repaint events.
        /// </summary>
        public static float BetterLabelWidth
        {
            get
            {
                if (BetterContextWidth == 0)
                {
                    return EditorGUIUtility.labelWidth;
                }
                
                // Unity only ever knows the exact labelWidth in repaint events.
                // But you often need it in Layout events as well.
                // See BetterContextWidths to learn more. 

                // Unity uses the ContextWidth to calculate the labelWidth.
                PushContextWidth(BetterContextWidth);
                var val = EditorGUIUtility.labelWidth;
                PopContextWidth();
                return val;
            }
            set
            {
                EditorGUIUtility.labelWidth = value;
            }
        }

        /// <summary>
        /// Odin will set this for you whenever an Odin property tree is drawn.
        /// But if you're using BetterLabelWidth and BetterContextWidth without Odin, then 
        /// you need to set BetterContextWidth in the beginning of each GUIEvent.
        /// </summary>
        public static float BetterContextWidth
        {
            get
            {
                if (betterContextWidth == 0)
                {
                    return ContextWidth;
                }

                return betterContextWidth;
            }
            set
            {
                betterContextWidth = value;
            }
        }
        

        /// <summary>
        /// Gets or sets the actual EditorGUIUtility.LabelWidth, regardless of the current hierarchy mode or context width.
        /// </summary>
        public static float ActualLabelWidth
        {
            get { return ActualLabelWidthGetter(); }
            set { BetterLabelWidth = value; }
        }

        /// <summary>
        /// Pushes a state to the GUI enabled stack. Remember to pop the state with <see cref="PopGUIEnabled"/>.
        /// </summary>
        /// <param name="enabled">If set to <c>true</c> GUI will be enabled. Otherwise GUI will be disabled.</param>
        public static void PushGUIEnabled(bool enabled)
        {
            GUIEnabledStack.Push(GUI.enabled);
            GUI.enabled = enabled;
        }

        /// <summary>
        /// Pops the GUI enabled pushed by <see cref="PushGUIEnabled(bool)"/>
        /// </summary>
        public static void PopGUIEnabled()
        {
            GUI.enabled = GUIEnabledStack.Pop();
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
            var actualLabelWidth = ActualLabelWidth;
            LabelWidthStack.Push(actualLabelWidth);
            var currentLabelWidth = preserveCurrentLabelWidth ? BetterLabelWidth : actualLabelWidth;
            HierarchyModeStack.Push(EditorGUIUtility.hierarchyMode);
            EditorGUIUtility.hierarchyMode = hierarchyMode;
            BetterLabelWidth = currentLabelWidth;
        }

        /// <summary>
        /// Pops the hierarchy mode pushed by <see cref="PushHierarchyMode"/>.
        /// </summary>
        public static void PopHierarchyMode()
        {
            EditorGUIUtility.hierarchyMode = HierarchyModeStack.Pop();
            ActualLabelWidth = LabelWidthStack.Pop();
        }
        
        /// <summary>
        /// Pushes the width to the editor GUI label width to the stack. Remmeber to Pop with <see cref="PopLabelWidth"/>.
        /// </summary>
        /// <param name="labelWidth">The editor GUI label width to push.</param>
        public static void PushLabelWidth(float labelWidth)
        {
            LabelWidthStack.Push(ActualLabelWidth);
            BetterLabelWidth = labelWidth;
        }

        /// <summary>
        /// Pops editor gui label widths pushed by <see cref="PushLabelWidth(float)"/>.
        /// </summary>
        public static void PopLabelWidth()
        {
            BetterLabelWidth = LabelWidthStack.Pop();
        }

        /// <summary>
        /// Pushes a context width to the context width stack.
        /// Remember to pop the value again with <see cref="M:Sirenix.Utilities.Editor.GUIHelper.PopContextWidth" />.
        /// </summary>
        /// <param name="width">The width to push.</param>
        public static void PushContextWidth(float width)
        {
            if (ContextWidthSetter != null)
            {
                ContextWidthStackOdinVersion.Push(width);
                ContextWidthSetter(width);
            }
            else
                ContextWidthStackGetter().Push(width);
        }

        /// <summary>
        /// Pops a value pushed by <see cref="M:Sirenix.Utilities.Editor.GUIHelper.PushContextWidth(System.Single)" />.
        /// </summary>
        public static void PopContextWidth()
        {
            if (ContextWidthSetter != null)
            {
                float num = ContextWidthStackOdinVersion.Pop();
                ContextWidthSetter(num);
            }
            else
            {
                Stack<float> floatStack = ContextWidthStackGetter();
                if (floatStack.Count <= 0)
                    return;
                double num = floatStack.Pop();
            }
        }

        /// <summary>
        /// Pushes the label color to the stack. Remember to pop with <see cref="PopLabelColor"/>.
        /// </summary>
        /// <param name="color">The label color to push.</param>
        public static void PushLabelColor(Color color)
        {
            LabelColorStack.Push(EditorStyles.label.normal.textColor);
            EditorStyles.label.normal.textColor = color;
            EasyGUIStyles.Foldout.normal.textColor = color;
            EasyGUIStyles.Foldout.onNormal.textColor = color;
        }

        /// <summary>
        /// Pops the label color pushed by <see cref="PushLabelColor(Color)"/>.
        /// </summary>
        public static void PopLabelColor()
        {
            var color = LabelColorStack.Pop();
            EditorStyles.label.normal.textColor = color;
            EasyGUIStyles.Foldout.normal.textColor = color;
            EasyGUIStyles.Foldout.onNormal.textColor = color;
        }
    }
}
