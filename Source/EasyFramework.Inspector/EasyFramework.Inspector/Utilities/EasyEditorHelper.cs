using System;
using System.Linq;
using System.Reflection;
using EasyFramework;
using UnityEditor;
using UnityEngine;

namespace EasyGameFramework
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
    }
}
