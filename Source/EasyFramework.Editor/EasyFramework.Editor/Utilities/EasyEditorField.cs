using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EasyFramework.Editor
{
    public class EasyEditorField
    {
        public static void Enum<T>(GUIContent label, ref T value, GUIStyle style = null)
            where T : Enum
        {
            value = Enum(label, value, style);
        }

        public static T Enum<T>(GUIContent label, T value, GUIStyle style = null)
            where T : Enum
        {
            return EnumSelector<T>.DrawEnumField(label, value, style);
        }

        public static void UnityObject<T>(GUIContent label, ref T value, bool allocSceneObjects = true,
            params GUILayoutOption[] options)
            where T : Object
        {
            value = UnityObject(label, value, allocSceneObjects, options);
        }

        public static T UnityObject<T>(GUIContent label, T value, bool allocSceneObjects = true,
            params GUILayoutOption[] options)
            where T : Object
        {
            return (T)SirenixEditorFields.UnityObjectField(label, value, typeof(T), allocSceneObjects, options);
        }

        public static int Value(GUIContent label, int value)
        {
            return EditorGUILayout.IntField(label, value);
        }

        public static float Value(GUIContent label, float value)
        {
            return EditorGUILayout.FloatField(label, value);
        }

        public static bool Value(GUIContent label, bool value)
        {
            return EditorGUILayout.Toggle(label, value);
        }

        public static string Value(GUIContent label, string value)
        {
            return EditorGUILayout.TextField(label, value);
        }

        public static Vector3 Value(GUIContent label, Vector3 value)
        {
            return EditorGUILayout.Vector3Field(label, value);
        }

        public static Vector2 Value(GUIContent label, Vector2 value)
        {
            return EditorGUILayout.Vector2Field(label, value);
        }

        public static Color Value(GUIContent label, Color value)
        {
            return EditorGUILayout.ColorField(label, value);
        }

        public static Gradient Value(GUIContent label, Gradient value)
        {
            return EditorGUILayout.GradientField(label, value);
        }


        public static void Value(GUIContent label, ref int value)
        {
            value = Value(label, value);
        }

        public static void Value(GUIContent label, ref float value)
        {
            value = Value(label, value);
        }

        public static void Value(GUIContent label, ref bool value)
        {
            value = Value(label, value);
        }

        public static void Value(GUIContent label, ref string value)
        {
            value = Value(label, value);
        }

        public static void Value(GUIContent label, ref Vector3 value)
        {
            value = Value(label, value);
        }

        public static void Value(GUIContent label, ref Vector2 value)
        {
            value = Value(label, value);
        }

        public static void Value(GUIContent label, ref Color value)
        {
            value = Value(label, value);
        }

        public static void Value(GUIContent label, ref Gradient value)
        {
            value = Value(label, value);
        }
    }
}
