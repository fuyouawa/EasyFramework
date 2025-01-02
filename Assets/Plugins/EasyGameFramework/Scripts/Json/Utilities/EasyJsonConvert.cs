#nullable enable
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics;
using EasyFramework;
using UnityEngine;

namespace EasyGameFramework
{
    public class EasyJsonConvert
    {
        public static JsonSerializerSettings SerializerSettings { get; private set; }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitializeStatics()
        {
            Initialize();
        }

        static EasyJsonConvert()
        {
            Initialize();
        }

        private static bool _initialized = false;
        static void Initialize()
        {
            if (_initialized)
                return;
            _initialized = true;

            SerializerSettings = new JsonSerializerSettings();
            SerializerSettings.Converters.AddRange(new JsonConverter[]
            {
                new ColorConverter(),
                new RectConverter(),
                new Vector2Converter(),
                new Vector2IntConverter(),
                new KeyframeConverter(),
                new AnimationCurveConverter()
            });

            JsonConvert.DefaultSettings += () => SerializerSettings;
        }

        private static JsonConverter[] CombineConverters(JsonConverter[] converts)
        {
            var ret = new List<JsonConverter>(SerializerSettings.Converters);
            ret.AddRange(converts);
            return ret.ToArray();
        }

        private static JsonSerializerSettings? CombineSettings(JsonSerializerSettings? settings)
        {
            if (settings == null)
                return null;
            var ret = new JsonSerializerSettings(settings);
            ret.Converters.AddRange(SerializerSettings.Converters);
            return ret;
        }

        #region Serialize
        /// <summary>
        /// Serializes the specified object to a JSON string.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <returns>A JSON string representation of the object.</returns>
        [DebuggerStepThrough]
        public static string SerializeObject(object? value)
        {
            return JsonConvert.SerializeObject(value, SerializerSettings);
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using formatting.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <returns>
        /// A JSON string representation of the object.
        /// </returns>
        [DebuggerStepThrough]
        public static string SerializeObject(object? value, Formatting formatting)
        {
            return JsonConvert.SerializeObject(value, formatting, SerializerSettings);
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using a collection of <see cref="JsonConverter"/>.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="converters">A collection of converters used while serializing.</param>
        /// <returns>A JSON string representation of the object.</returns>
        [DebuggerStepThrough]
        public static string SerializeObject(object? value, params JsonConverter[] converters)
        {
            return JsonConvert.SerializeObject(value, CombineConverters(converters));
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using formatting and a collection of <see cref="JsonConverter"/>.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="converters">A collection of converters used while serializing.</param>
        /// <returns>A JSON string representation of the object.</returns>
        [DebuggerStepThrough]
        public static string SerializeObject(object? value, Formatting formatting, params JsonConverter[] converters)
        {
            return JsonConvert.SerializeObject(value, formatting, CombineConverters(converters));
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using <see cref="JsonSerializerSettings"/>.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <returns>
        /// A JSON string representation of the object.
        /// </returns>
        [DebuggerStepThrough]
        public static string SerializeObject(object? value, JsonSerializerSettings? settings)
        {
            return JsonConvert.SerializeObject(value, CombineSettings(settings));
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using a type, formatting and <see cref="JsonSerializerSettings"/>.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <param name="type">
        /// The type of the value being serialized.
        /// This parameter is used when <see cref="JsonSerializer.TypeNameHandling"/> is <see cref="TypeNameHandling.Auto"/> to write out the type name if the type of the value does not match.
        /// Specifying the type is optional.
        /// </param>
        /// <returns>
        /// A JSON string representation of the object.
        /// </returns>
        [DebuggerStepThrough]
        public static string SerializeObject(object? value, Type? type, JsonSerializerSettings? settings)
        {
            return JsonConvert.SerializeObject(value, type, CombineSettings(settings));
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using formatting and <see cref="JsonSerializerSettings"/>.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <returns>
        /// A JSON string representation of the object.
        /// </returns>
        [DebuggerStepThrough]
        public static string SerializeObject(object? value, Formatting formatting, JsonSerializerSettings? settings)
        {
            return JsonConvert.SerializeObject(value, formatting, CombineSettings(settings));
        }

        /// <summary>
        /// Serializes the specified object to a JSON string using a type, formatting and <see cref="JsonSerializerSettings"/>.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="formatting">Indicates how the output should be formatted.</param>
        /// <param name="settings">The <see cref="JsonSerializerSettings"/> used to serialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.</param>
        /// <param name="type">
        /// The type of the value being serialized.
        /// This parameter is used when <see cref="JsonSerializer.TypeNameHandling"/> is <see cref="TypeNameHandling.Auto"/> to write out the type name if the type of the value does not match.
        /// Specifying the type is optional.
        /// </param>
        /// <returns>
        /// A JSON string representation of the object.
        /// </returns>
        [DebuggerStepThrough]
        public static string SerializeObject(object? value, Type? type, Formatting formatting, JsonSerializerSettings? settings)
        {
            return JsonConvert.SerializeObject(value, type, CombineSettings(settings));
        }
        #endregion

        #region Deserialize
        /// <summary>
        /// Deserializes the JSON to a .NET object.
        /// </summary>
        /// <param name="value">The JSON to deserialize.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        [DebuggerStepThrough]
        public static object? DeserializeObject(string value)
        {
            return JsonConvert.DeserializeObject(value, SerializerSettings);
        }

        /// <summary>
        /// Deserializes the JSON to a .NET object using <see cref="JsonSerializerSettings"/>.
        /// </summary>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="settings">
        /// The <see cref="JsonSerializerSettings"/> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.
        /// </param>
        /// <returns>The deserialized object from the JSON string.</returns>
        [DebuggerStepThrough]
        public static object? DeserializeObject(string value, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject(value, CombineSettings(settings)!);
        }

        /// <summary>
        /// Deserializes the JSON to the specified .NET type.
        /// </summary>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="type">The <see cref="Type"/> of object being deserialized.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        [DebuggerStepThrough]
        public static object? DeserializeObject(string value, Type type)
        {
            return JsonConvert.DeserializeObject(value, type, SerializerSettings);
        }

        /// <summary>
        /// Deserializes the JSON to the specified .NET type.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="value">The JSON to deserialize.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        [DebuggerStepThrough]
        public static T? DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, SerializerSettings);
        }

        /// <summary>
        /// Deserializes the JSON to the given anonymous type.
        /// </summary>
        /// <typeparam name="T">
        /// The anonymous type to deserialize to. This can't be specified
        /// traditionally and must be inferred from the anonymous type passed
        /// as a parameter.
        /// </typeparam>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="anonymousTypeObject">The anonymous type object.</param>
        /// <returns>The deserialized anonymous type from the JSON string.</returns>
        [DebuggerStepThrough]
        public static T? DeserializeAnonymousType<T>(string value, T anonymousTypeObject)
        {
            return JsonConvert.DeserializeAnonymousType<T>(value, anonymousTypeObject, SerializerSettings);
        }

        /// <summary>
        /// Deserializes the JSON to the given anonymous type using <see cref="JsonSerializerSettings"/>.
        /// </summary>
        /// <typeparam name="T">
        /// The anonymous type to deserialize to. This can't be specified
        /// traditionally and must be inferred from the anonymous type passed
        /// as a parameter.
        /// </typeparam>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="anonymousTypeObject">The anonymous type object.</param>
        /// <param name="settings">
        /// The <see cref="JsonSerializerSettings"/> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.
        /// </param>
        /// <returns>The deserialized anonymous type from the JSON string.</returns>
        [DebuggerStepThrough]
        public static T? DeserializeAnonymousType<T>(string value, T anonymousTypeObject, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeAnonymousType<T>(value, anonymousTypeObject, CombineSettings(settings));
        }

        /// <summary>
        /// Deserializes the JSON to the specified .NET type using a collection of <see cref="JsonConverter"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="converters">Converters to use while deserializing.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        [DebuggerStepThrough]
        public static T? DeserializeObject<T>(string value, params JsonConverter[] converters)
        {
            return JsonConvert.DeserializeObject<T>(value, CombineConverters(converters));
        }

        /// <summary>
        /// Deserializes the JSON to the specified .NET type using <see cref="JsonSerializerSettings"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
        /// <param name="value">The object to deserialize.</param>
        /// <param name="settings">
        /// The <see cref="JsonSerializerSettings"/> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.
        /// </param>
        /// <returns>The deserialized object from the JSON string.</returns>
        [DebuggerStepThrough]
        public static T? DeserializeObject<T>(string value, JsonSerializerSettings? settings)
        {
            return JsonConvert.DeserializeObject<T>(value, CombineSettings(settings));
        }

        /// <summary>
        /// Deserializes the JSON to the specified .NET type using a collection of <see cref="JsonConverter"/>.
        /// </summary>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="type">The type of the object to deserialize.</param>
        /// <param name="converters">Converters to use while deserializing.</param>
        /// <returns>The deserialized object from the JSON string.</returns>
        [DebuggerStepThrough]
        public static object? DeserializeObject(string value, Type type, params JsonConverter[] converters)
        {
            return JsonConvert.DeserializeObject(value, type, CombineConverters(converters));
        }

        /// <summary>
        /// Deserializes the JSON to the specified .NET type using <see cref="JsonSerializerSettings"/>.
        /// </summary>
        /// <param name="value">The JSON to deserialize.</param>
        /// <param name="type">The type of the object to deserialize to.</param>
        /// <param name="settings">
        /// The <see cref="JsonSerializerSettings"/> used to deserialize the object.
        /// If this is <c>null</c>, default serialization settings will be used.
        /// </param>
        /// <returns>The deserialized object from the JSON string.</returns>
        public static object? DeserializeObject(string value, Type? type, JsonSerializerSettings? settings)
        {
            return JsonConvert.DeserializeObject(value, type, CombineSettings(settings));
        }
        #endregion
    }
}
