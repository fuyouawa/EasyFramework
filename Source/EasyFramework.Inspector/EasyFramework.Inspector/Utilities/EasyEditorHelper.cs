using System;
using System.Reflection;
using EasyFramework.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyFramework.Inspector
{
    public static class EasyEditorHelper
    {
        private static MethodInfo _forceRebuildInspectors;
        public static void ForceRebuildInspectors()
        {
            if (_forceRebuildInspectors == null)
            {
                _forceRebuildInspectors = typeof(EditorUtility).GetMethod("ForceRebuildInspectors",
                    BindingFlags.NonPublic | BindingFlags.Static);
            }

            _forceRebuildInspectors!.Invoke(null, null);
        }

        private static MethodInfo _findTexture;
        public static Texture2D FindTexture(Type type)
        {
            if (_findTexture == null)
            {
                _findTexture = typeof(EditorGUIUtility).GetMethodEx("FindTexture",
                    BindingFlags.NonPublic | BindingFlags.Static, new[] { typeof(Type) });
            }

            return (Texture2D)_findTexture.Invoke(null, null);
        }
        
        private static Type _typeOfAddComponentWindow;

        public static Type TypeOfAddComponentWindow
        {
            get
            {
                if (_typeOfAddComponentWindow == null)
                {
                    _typeOfAddComponentWindow = Type.GetType("UnityEditor.AddComponent.AddComponentWindow, UnityEditor");
                }
                return _typeOfAddComponentWindow;
            }
        }
        
        public static void ShowAddComponentWindow(Rect rect, GameObject[] gos)
        {
            TypeOfAddComponentWindow.InvokeMethod("Show", null, rect, gos);
        }
    }
}
