using System;
using EasyFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace EasyGameFramework
{
    public static class JsonExtension
    {
        private static JTokenType GetJTokenType(Type type)
        {
            if (type.IsStringType())
                return JTokenType.String;
            if (type.IsIntegerType())
                return JTokenType.Integer;
            if (type.IsFloatType())
                return JTokenType.Float;
            if (type.IsBoolType())
                return JTokenType.Boolean;
            if (type == typeof(DateTime))
                return JTokenType.Date;
            if (type == typeof(Guid))
                return JTokenType.Guid;
            if (type == typeof(Uri))
                return JTokenType.Uri;
            if (type == typeof(TimeSpan))
                return JTokenType.TimeSpan;
            if (type.IsArray || typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
                return JTokenType.Array;
            if (type.IsClass || type.IsValueType)
                return JTokenType.Object;

            return JTokenType.None;
        }

        public static void WriteProperty(this JsonWriter writer, string propertyName, object val)
        {
            writer.WritePropertyName(propertyName);
            writer.WriteValue(val);
        }
        

        public static void WriteCustomProperty(this JsonWriter writer, JsonSerializer serializer, string propertyName, object val)
        {
            writer.WritePropertyName(propertyName);
            serializer.Serialize(writer, val);
        }

        public static bool TryGetProperty<T>(this JObject j, string propertyName, out T val)
        {
            if (j.TryGetValue(propertyName, out var t))
            {
                if (t.Type == GetJTokenType(typeof(T)))
                {
                    val = t.ToObject<T>();
                    return true;
                }
            }

            val = default;
            return false;
        }

        public static bool TryGetCustomProperty<T>(this JObject j, JsonSerializer serializer, string propertyName, out T val)
        {
            if (j.TryGetValue(propertyName, out var t))
            {
                val = t.ToObject<T>(serializer);
                return true;
            }

            val = default;
            return false;
        }

        public static bool TrySetCustomPropertyByRef<T>(this JObject j, JsonSerializer serializer, string propertyName, ref T val)
        {
            if (j.TryGetValue(propertyName, out var t))
            {
                val = t.ToObject<T>(serializer);
                return true;
            }

            return false;
        }


        public static T GetProperty<T>(this JObject j, string propertyName)
        {
            if (j.TryGetProperty(propertyName, out T val))
            {
                return val;
            }
            throw new ArgumentException($"No property:{propertyName}");
        }


        public static T GetPropertyOrDefault<T>(this JObject j, string propertyName, T defaultValue = default)
        {
            if (j.TryGetProperty(propertyName, out T val))
            {
                return val;
            }

            return defaultValue;
        }

        public static T GetCustomProperty<T>(this JObject j, JsonSerializer serializer, string propertyName)
        {
            if (j.TryGetCustomProperty(serializer, propertyName, out T val))
            {
                return val;
            }
            throw new ArgumentException($"No property:{propertyName}");
        }

        public static JsonWriteObjectScope WriteObjectScope(this JsonWriter writer)
        {
            return new JsonWriteObjectScope(writer);
        }

        public static JsonWriteArrayScope WriteArrayScope(this JsonWriter writer)
        {
            return new JsonWriteArrayScope(writer);
        }
    }
}
