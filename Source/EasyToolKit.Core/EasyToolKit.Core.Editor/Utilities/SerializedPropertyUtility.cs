using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Core.Editor
{
    public static class SerializedPropertyUtility
    {
        private static Dictionary<Type, Delegate> PrimitiveValueGetters = new Dictionary<Type, Delegate>()
        {
            { typeof(int),              (Func<SerializedProperty, int>)             (p => p.intValue) },
            { typeof(bool),             (Func<SerializedProperty, bool>)            (p => p.boolValue) },
            { typeof(float),            (Func<SerializedProperty, float>)           (p => p.floatValue) },
            { typeof(string),           (Func<SerializedProperty, string>)          (p => p.stringValue) },
            { typeof(Color),            (Func<SerializedProperty, Color>)           (p => p.colorValue) },
            { typeof(LayerMask),        (Func<SerializedProperty, LayerMask>)       (p => p.intValue) },
            { typeof(Vector2),          (Func<SerializedProperty, Vector2>)         (p => p.vector2Value) },
            { typeof(Vector3),          (Func<SerializedProperty, Vector3>)         (p => p.vector3Value) },
            { typeof(Vector4),          (Func<SerializedProperty, Vector4>)         (p => p.vector4Value) },
            { typeof(Rect),             (Func<SerializedProperty, Rect>)            (p => p.rectValue) },
            { typeof(char),             (Func<SerializedProperty, char>)            (p => (char)p.intValue) },
            { typeof(AnimationCurve),   (Func<SerializedProperty, AnimationCurve>)  (p => p.animationCurveValue) },
            { typeof(Bounds),           (Func<SerializedProperty, Bounds>)          (p => p.boundsValue) },
            { typeof(Quaternion),       (Func<SerializedProperty, Quaternion>)      (p => p.quaternionValue) },
        };

        private static Dictionary<Type, Delegate> PrimitiveValueSetters = new Dictionary<Type, Delegate>()
        {
            { typeof(int),              (Action<SerializedProperty, int>)             ((p, v) => p.intValue = v) },
            { typeof(bool),             (Action<SerializedProperty, bool>)            ((p, v) => p.boolValue = v) },
            { typeof(float),            (Action<SerializedProperty, float>)           ((p, v) => p.floatValue = v) },
            { typeof(string),           (Action<SerializedProperty, string>)          ((p, v) => p.stringValue = v) },
            { typeof(Color),            (Action<SerializedProperty, Color>)           ((p, v) => p.colorValue = v) },
            { typeof(LayerMask),        (Action<SerializedProperty, LayerMask>)       ((p, v) => p.intValue = v) },
            { typeof(Vector2),          (Action<SerializedProperty, Vector2>)         ((p, v) => p.vector2Value = v) },
            { typeof(Vector3),          (Action<SerializedProperty, Vector3>)         ((p, v) => p.vector3Value = v) },
            { typeof(Vector4),          (Action<SerializedProperty, Vector4>)         ((p, v) => p.vector4Value = v) },
            { typeof(Rect),             (Action<SerializedProperty, Rect>)            ((p, v) => p.rectValue = v) },
            { typeof(char),             (Action<SerializedProperty, char>)            ((p, v) => p.intValue = v) },
            { typeof(AnimationCurve),   (Action<SerializedProperty, AnimationCurve>)  ((p, v) => p.animationCurveValue = v) },
            { typeof(Bounds),           (Action<SerializedProperty, Bounds>)          ((p, v) => p.boundsValue = v) },
            { typeof(Quaternion),       (Action<SerializedProperty, Quaternion>)      ((p, v) => p.quaternionValue = v) },
        };
        
        public static string GetProperTypeName(this SerializedProperty property)
        {
            if (property.type.StartsWith("PPtr<"))
            {
                return property.type.Substring(5).Trim('<', '>', '$');
            }

            return property.type;
        }

        public static Type GetPropertyType(this SerializedProperty property)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Generic:
                    return null;

                case SerializedPropertyType.Integer:
                    return typeof(int);

                case SerializedPropertyType.Boolean:
                    return typeof(bool);

                case SerializedPropertyType.Float:
                    return typeof(float);

                case SerializedPropertyType.String:
                    return typeof(string);

                case SerializedPropertyType.Color:
                    return typeof(Color);

                case SerializedPropertyType.ObjectReference:
                    {
                        if (!object.ReferenceEquals(property.objectReferenceValue, null))
                        {
                            return property.objectReferenceValue.GetType();
                        }

                        string typeName = property.GetProperTypeName();

                        var possibles = AssemblyUtility.GetTypes(AssemblyCategory.UnityEngine)
                                                         .Where(n => n.Name == typeName && typeof(UnityEngine.Object).IsAssignableFrom(n))
                                                         .ToList();

                        if (possibles.Count == 1)
                        {
                            return possibles[0];
                        }

                        return null;
                    }
                case SerializedPropertyType.LayerMask:
                    return typeof(LayerMask);

                case SerializedPropertyType.Enum:
                    return null;

                case SerializedPropertyType.Vector2:
                    return typeof(Vector2);

                case SerializedPropertyType.Vector3:
                    return typeof(Vector3);

                case SerializedPropertyType.Vector4:
                    return typeof(Vector4);

                case SerializedPropertyType.Rect:
                    return typeof(Rect);

                case SerializedPropertyType.ArraySize:
                    return null;

                case SerializedPropertyType.Character:
                    return typeof(char);

                case SerializedPropertyType.AnimationCurve:
                    return typeof(AnimationCurve);

                case SerializedPropertyType.Bounds:
                    return typeof(Bounds);

                case SerializedPropertyType.Gradient:
                    return typeof(Gradient);

                case SerializedPropertyType.Quaternion:
                    return typeof(Quaternion);
                default:
                    return null;
            }
        }
        
        public static Func<SerializedProperty, T> GetValueGetter<T>()
        {
            var getter = GetWeakValueGetter(typeof(T));
            return property => (T)getter(property);
        }

        public static Func<SerializedProperty, object> GetWeakValueGetter(Type valueType)
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(valueType))
            {
                return p => p.objectReferenceValue;
            }

            if (PrimitiveValueGetters.TryGetValue(valueType, out var result))
            {
                return (Func<SerializedProperty, object>)result;
            }

            return property => property.boxedValue;
        }
        
        public static Action<SerializedProperty, T> GetValueSetter<T>()
        {
            var setter = GetWeakValueSetter(typeof(T));
            return (property, value) => setter(property, value);
        }

        public static Action<SerializedProperty, object> GetWeakValueSetter(Type valueType)
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(valueType))
            {
                return (p, v) => p.objectReferenceValue = (UnityEngine.Object)v;
            }

            if (PrimitiveValueSetters.TryGetValue(valueType, out var result))
            {
                return (Action<SerializedProperty, object>)result;
            }

            return (property, value) => property.boxedValue = value;
        }
    }
}
