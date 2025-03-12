using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyFramework.Serialization
{
    public static class EasySerialize
    {
        public static EasySerializeSettings DefaultSettings { get; set; } = new EasySerializeSettings();
        internal static EasySerializeSettings CurrentSettings { get; private set; }

        public static byte[] To<T>(T value, EasyDataFormat format, EasySerializeSettings settings = null)
        {
            var referencedUnityObjects = new List<UnityEngine.Object>();
            return To(value, format, ref referencedUnityObjects, settings);
        }

        public static T From<T>(EasyDataFormat format, byte[] data, EasySerializeSettings settings = null)
        {
            return From<T>(format, data, new List<UnityEngine.Object>(), settings);
        }

        public static byte[] To<T>(T value, EasyDataFormat format, ref List<UnityEngine.Object> referencedUnityObjects, EasySerializeSettings settings = null)
        {
            var d = new EasySerializationData(format);
            To(value, ref d, settings);
            referencedUnityObjects = d.ReferencedUnityObjects;
            return d.GetData();
        }

        public static T From<T>(EasyDataFormat format, byte[] data, List<UnityEngine.Object> referencedUnityObjects, EasySerializeSettings settings = null)
        {
            var d = new EasySerializationData(format);
            d.SetData(data);
            d.ReferencedUnityObjects = referencedUnityObjects ?? new List<UnityEngine.Object>();
            return From<T>(ref d, settings);
        }

        public static void To<T>(T value, ref EasySerializationData serializationData, EasySerializeSettings settings = null)
        {
            if (value == null)
            {
                Debug.LogWarning("Serialize null value!");
                serializationData.SetData(Array.Empty<byte>());
                return;
            }
            var valueType = value.GetType();

            CurrentSettings = settings ?? DefaultSettings;

            var ios = GenericNative.AllocStringIoStream();
            using (ios.GetWrapper())
            {
                using (var arch = GetOutputArchive(serializationData.Format, ios))
                {
                    var serializer = GetSerializerWithThrow(valueType);
                    ((IEasySerializer)serializer).IsRoot = true;

                    var obj = (object)value;
                    serializer.Process(ref obj, valueType, arch);

                    serializationData.ReferencedUnityObjects = arch.GetReferencedUnityObjects();
                }

                var cBuf = GenericNative.GetIoStreamBuffer(ios);
                serializationData.SetData(cBuf.ToBytesWithFree());
            }
        }

        public static T From<T>(ref EasySerializationData serializationData, EasySerializeSettings settings = null)
        {
            CurrentSettings = settings ?? DefaultSettings;

            T res = default;
            var buf = serializationData.GetData();
            if (buf.Length == 0)
                return default;

            var ios = GenericNative.AllocStringIoStream();
            using (ios.GetWrapper())
            {
                var cBuf = buf.ToNativeBuffer();
                using (cBuf.GetWrapper())
                {
                    GenericNative.WriteToIoStreamBuffer(ios, cBuf);
                }

                using (var arch = GetInputArchive(serializationData.Format, ios))
                {
                    arch.SetupReferencedUnityObjects(serializationData.ReferencedUnityObjects);

                    var serializer = (EasySerializer<T>)GetSerializerWithThrow(typeof(T));
                    ((IEasySerializer)serializer).IsRoot = true;

                    serializer.Process(ref res, arch);
                }
            }

            return res;
        }

        private static EasySerializer GetSerializerWithThrow(Type type)
        {
            var serializer = EasySerializationUtility.GetSerializer(type);
            if (serializer == null)
            {
                throw new ArgumentException(
                    $"There is no serializer for type '{type.FullName}'." +
                    "You need to implement a 'IEasySerializer' for type.");
            }

            return serializer;
        }

        private static OutputArchive GetOutputArchive(EasyDataFormat format, GenericNative.IoStream stream)
        {
            return format switch
            {
                EasyDataFormat.Binary => new BinaryOutputArchive(stream),
                EasyDataFormat.Json => new JsonOutputArchive(stream),
                EasyDataFormat.Xml => throw new NotImplementedException(),
                EasyDataFormat.Yaml => throw new NotImplementedException(),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }

        private static InputArchive GetInputArchive(EasyDataFormat format, GenericNative.IoStream stream)
        {
            return format switch
            {
                EasyDataFormat.Binary => new BinaryInputArchive(stream),
                EasyDataFormat.Json => new JsonInputArchive(stream),
                EasyDataFormat.Xml => throw new NotImplementedException(),
                EasyDataFormat.Yaml => throw new NotImplementedException(),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }
    }
}
