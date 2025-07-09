using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyToolKit.Core
{
    /// <summary>
    /// Specifies a data format to read and write in.
    /// </summary>
    public enum DataFormat
    {
        /// <summary>
        /// A custom packed binary format. This format is most efficient and almost allocation-free,
        /// but its serialized data is not human-readable.
        /// </summary>
        Binary = 0,

        /// <summary>
        /// A JSON format compliant with the json specification found at "http://www.json.org/".
        /// <para />
        /// This format has rather sluggish performance and allocates frightening amounts of string garbage.
        /// </summary>
        Json = 1,

        /// <summary>
        /// A format that does not serialize to a byte stream, but to a list of data nodes in memory
        /// which can then be serialized by Unity.
        /// <para />
        /// This format is highly inefficient, and is primarily used for ensuring that Unity assets
        /// are mergeable by individual values when saved in Unity's text format. This makes
        /// serialized values more robust and data recovery easier in case of issues.
        /// <para />
        /// This format is *not* recommended for use in builds.
        /// </summary>
        Nodes = 2
    }

    [Serializable]
    public struct SerializationData
    {
        [SerializeField] internal Internal.OdinSerializer.SerializationData InternalData;
    }

    public class EasySerializerAttribute : Internal.OdinSerializer.OdinSerializeAttribute {}

    public static class Serializer
    {
        private static Internal.OdinSerializer.DataFormat ConvertToOdin(DataFormat format)
        {
            return format switch
            {
                DataFormat.Binary => Internal.OdinSerializer.DataFormat.Binary,
                DataFormat.Json => Internal.OdinSerializer.DataFormat.JSON,
                DataFormat.Nodes => Internal.OdinSerializer.DataFormat.Nodes,
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }

        /// <summary>
        /// Serializes the given value using the specified format, and returns the result as a byte array.
        /// </summary>
        /// <typeparam name="T">The type of the value to serialize.</typeparam>
        /// <param name="value">The value to serialize.</param>
        /// <param name="format">The format to use.</param>
        /// <returns>A byte array containing the serialized value.</returns>
        public static byte[] Serialize<T>(T value, DataFormat format)
        {
            return Internal.OdinSerializer.SerializationUtility.SerializeValue(value, ConvertToOdin(format));
        }


        /// <summary>
        /// Serializes the given value using the specified format and returns the result as a byte array.
        /// </summary>
        /// <typeparam name="T">The type of the value to serialize.</typeparam>
        /// <param name="value">The value to serialize.</param>
        /// <param name="format">The format to use.</param>
        /// <param name="unityObjects">A list of the Unity objects which were referenced during serialization.</param>
        /// <returns>A byte array containing the serialized value.</returns>
        public static byte[] Serialize<T>(T value, DataFormat format, out List<UnityEngine.Object> unityObjects)
        {
            return Internal.OdinSerializer.SerializationUtility.SerializeValue(value, ConvertToOdin(format),
                out unityObjects);
        }

        /// <summary>
        /// Serializes the given value using the specified format, and returns the result as a byte array.
        /// </summary>
        /// <param name="value">The value to serialize.</param>
        /// <param name="format">The format to use.</param>
        /// <returns>A byte array containing the serialized value.</returns>
        public static byte[] SerializeWeak(object value, DataFormat format)
        {
            return Internal.OdinSerializer.SerializationUtility.SerializeValueWeak(value, ConvertToOdin(format));
        }

        public static byte[] SerializeWeak(object value, DataFormat format, out List<UnityEngine.Object> unityObjects)
        {
            return Internal.OdinSerializer.SerializationUtility.SerializeValueWeak(value, ConvertToOdin(format),
                out unityObjects);
        }


        /// <summary>
        /// Deserializes a value of a given type from the given byte array in the given format.
        /// </summary>
        /// <typeparam name="T">The type to deserialize.</typeparam>
        /// <param name="bytes">The bytes to deserialize from.</param>
        /// <param name="format">The format to read.</param>
        /// <returns>
        /// The deserialized value.
        /// </returns>
        public static T Deserialize<T>(byte[] bytes, DataFormat format)
        {
            return Internal.OdinSerializer.SerializationUtility.DeserializeValue<T>(bytes, ConvertToOdin(format));
        }

        /// <summary>
        /// Deserializes a value from the given byte array in the given format. This might fail with primitive values, as they don't come with type metadata.
        /// </summary>
        /// <param name="bytes">The bytes to deserialize from.</param>
        /// <param name="format">The format to read.</param>
        /// <returns>
        /// The deserialized value.
        /// </returns>
        public static object DeserializeWeak(byte[] bytes, DataFormat format)
        {
            return Internal.OdinSerializer.SerializationUtility.DeserializeValueWeak(bytes, ConvertToOdin(format));
        }


        public static object DeserializeWeak(
            byte[] bytes,
            DataFormat format,
            List<UnityEngine.Object> referencedUnityObjects)
        {
            return Internal.OdinSerializer.SerializationUtility.DeserializeValueWeak(bytes, ConvertToOdin(format),
                referencedUnityObjects);
        }


        /// <summary>
        /// Deserializes a value of a given type from the given byte array in the given format, using the given list of Unity objects for external index reference resolution.
        /// </summary>
        /// <typeparam name="T">The type to deserialize.</typeparam>
        /// <param name="bytes">The bytes to deserialize from.</param>
        /// <param name="format">The format to read.</param>
        /// <param name="referencedUnityObjects">The list of Unity objects to use for external index reference resolution.</param>
        /// <returns>
        /// The deserialized value.
        /// </returns>
        public static T Deserialize<T>(byte[] bytes, DataFormat format, List<UnityEngine.Object> referencedUnityObjects)
        {
            return Internal.OdinSerializer.SerializationUtility.DeserializeValue<T>(bytes, ConvertToOdin(format),
                referencedUnityObjects);
        }


        public static void SerializeUnityObject(UnityEngine.Object unityObject, ref SerializationData data,
            bool serializeUnityFields = false)
        {
            Internal.OdinSerializer.UnitySerializationUtility.SerializeUnityObject(unityObject, ref data.InternalData,
                serializeUnityFields);
        }


        public static void DeserializeUnityObject(
            UnityEngine.Object unityObject,
            ref SerializationData data)
        {
            Internal.OdinSerializer.UnitySerializationUtility.DeserializeUnityObject(unityObject,
                ref data.InternalData);
        }
    }
}