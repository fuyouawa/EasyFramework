using System;
using System.Reflection;
using EasyFramework.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Inspector
{
    public static class EasyEditorHelper
    {
        private static MethodInfo s_forceRebuildInspectors;

        public static void ForceRebuildInspectors()
        {
            if (s_forceRebuildInspectors == null)
            {
                s_forceRebuildInspectors = typeof(EditorUtility).GetMethod("ForceRebuildInspectors",
                    BindingFlags.NonPublic | BindingFlags.Static);
            }

            s_forceRebuildInspectors!.Invoke(null, null);
        }

        private static MethodInfo s_findTexture;

        public static Texture2D FindTexture(Type type)
        {
            if (s_findTexture == null)
            {
                s_findTexture = typeof(EditorGUIUtility).GetMethodEx("FindTexture",
                    BindingFlags.NonPublic | BindingFlags.Static, new[] { typeof(Type) });
            }

            return (Texture2D)s_findTexture.Invoke(null, null);
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

        public static void ShowAddComponentWindow(Rect rect, GameObject[] gos)
        {
            TypeOfAddComponentWindow.InvokeMethod("Show", null, rect, gos);
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
