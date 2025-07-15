using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Core.Editor
{
    public static class EasyEditorHelper
    {
        private static MethodInfo s_methodOfForceRebuildInspectors;

        public static void ForceRebuildInspectors()
        {
            if (s_methodOfForceRebuildInspectors == null)
            {
                s_methodOfForceRebuildInspectors = typeof(EditorUtility).GetMethod("ForceRebuildInspectors",
                    BindingFlagsHelper.NonPublicStatic());
            }

            s_methodOfForceRebuildInspectors!.Invoke(null, null);
        }

        private static MethodInfo s_methodOfFindTexture;

        public static Texture2D FindTexture(Type type)
        {
            if (s_methodOfFindTexture == null)
            {
                s_methodOfFindTexture = typeof(EditorGUIUtility).GetMethodEx("FindTexture",
                    BindingFlagsHelper.NonPublicStatic(), new[] { typeof(Type) });
            }

            return (Texture2D)s_methodOfFindTexture.Invoke(null, null);
        }

        private static Type s_typeOfAddComponentWindow;

        public static Type TypeOfAddComponentWindow
        {
            get
            {
                if (s_typeOfAddComponentWindow == null)
                {
                    s_typeOfAddComponentWindow =
                        Type.GetType("UnityEditor.AddComponent.AddComponentWindow, UnityEditor");
                }

                return s_typeOfAddComponentWindow;
            }
        }

        private static MethodInfo s_methodOfShowAddComponentWindow;

        public static void ShowAddComponentWindow(Rect rect, GameObject[] gos)
        {
            if (s_methodOfShowAddComponentWindow == null)
            {
                s_methodOfShowAddComponentWindow = typeof(EditorGUI).GetMethod("Show",
                    BindingFlagsHelper.NonPublicStatic())!;
            }

            s_methodOfShowAddComponentWindow.Invoke(null, new object[] { rect, gos });
        }

        private static MethodInfo s_methodOfHasKeyboardFocus;

        public static bool HasKeyboardFocus(int controlId)
        {
            if (s_methodOfHasKeyboardFocus == null)
            {
                s_methodOfHasKeyboardFocus = typeof(EditorGUI).GetMethod("HasKeyboardFocus",
                    BindingFlagsHelper.NonPublicStatic())!;
            }

            return (bool)s_methodOfHasKeyboardFocus.Invoke(null, new object[] { controlId });
        }

        private static MethodInfo s_methodOfEndEditingActiveTextField;

        public static void EndEditingActiveTextField()
        {
            if (s_methodOfEndEditingActiveTextField == null)
            {
                s_methodOfEndEditingActiveTextField = typeof(EditorGUI).GetMethod("EndEditingActiveTextField",
                    BindingFlagsHelper.NonPublicStatic())!;
            }

            s_methodOfEndEditingActiveTextField.Invoke(null, null);
        }


        private static readonly GUIContent s_tempContent = new GUIContent();

        internal static GUIContent TempContent(string text, string tooltip = null, Texture image = null)
        {
            s_tempContent.text = text;
            s_tempContent.image = image;
            s_tempContent.tooltip = tooltip;
            return s_tempContent;
        }
    }
}
