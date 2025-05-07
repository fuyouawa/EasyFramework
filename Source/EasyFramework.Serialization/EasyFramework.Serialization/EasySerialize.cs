using System;
using System.Collections.Generic;
using EasyFramework.Core;
using EasyFramework.Core.Internal;
using UnityEngine;

namespace EasyFramework.Serialization
{
    public static class EasySerialize
    {
        public static EasySerializeSettings DefaultSettings { get; set; } = new EasySerializeSettings();

        internal static EasySerializeSettings CurrentSettings { get; private set; }
        
        public static void To<T>(T value, ref EasySerializationData serializationData,
            EasySerializeSettings settings = null)
        {
            To(value, typeof(T), ref serializationData, settings);
        }

        public static T From<T>(ref EasySerializationData serializationData, EasySerializeSettings settings = null)
        {
            return (T)From(typeof(T), ref serializationData, settings);
        }

        public static void To(object value, Type valueType, ref EasySerializationData serializationData,
            EasySerializeSettings settings = null)
        {
            if (value == null)
            {
                // Debug.LogWarning("Serialize null value!");
                serializationData.SetData(new byte[] { });
                return;
            }

            ChangeSettings(settings);

            var ios = NativeGeneric.AllocStringIoStreamSafety();
            using (ios.GetWrapper())
            {
                using (var arch = GetOutputArchive(serializationData.Format, ios))
                {
                    var serializer = GetSerializerWithThrow(valueType);
                    ((IEasySerializer)serializer).IsRoot = true;

                    serializer.Process(ref value, valueType, arch);

                    serializationData.ReferencedUnityObjects = arch.GetReferencedUnityObjects();
                }

                var cBuf = NativeGeneric.GetIoStreamBufferSafety(ios);
                serializationData.SetData(cBuf.ToBytesWithFree());
            }
        }

        public static object From(Type type, ref EasySerializationData serializationData,
            EasySerializeSettings settings = null)
        {
            ChangeSettings(settings);

            object res = null;
            var buf = serializationData.GetData();
            if (buf.Length == 0)
                return null;

            var ios = NativeGeneric.AllocStringIoStreamSafety();
            using (ios.GetWrapper())
            {
                var cBuf = buf.ToNativeBuffer();
                using (cBuf.GetWrapper())
                {
                    NativeGeneric.WriteToIoStreamBufferSafety(ios, cBuf);
                }

                using (var arch = GetInputArchive(serializationData.Format, ios))
                {
                    arch.SetupReferencedUnityObjects(serializationData.ReferencedUnityObjects);

                    var serializer = GetSerializerWithThrow(type);
                    ((IEasySerializer)serializer).IsRoot = true;

                    serializer.Process(ref res, type, arch);
                }
            }
            
            return res;
        }

        private static EasySerializer GetSerializerWithThrow(Type type)
        {
            var serializer = EasySerializersManager.Instance.GetSerializer(type);
            if (serializer == null)
            {
                throw new ArgumentException(
                    $"There is no serializer for type '{type.FullName}'." +
                    "You need to implement a 'IEasySerializer' for type.");
            }

            return serializer;
        }

        private static void ChangeSettings(EasySerializeSettings settings)
        {
            settings ??= DefaultSettings;
            if (settings != CurrentSettings)
            {
                CurrentSettings = settings;
                EasySerializersManager.Instance.ClearCache();
            }
        }

        private static OutputArchive GetOutputArchive(EasyDataFormat format, NativeIoStream stream)
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

        private static InputArchive GetInputArchive(EasyDataFormat format, NativeIoStream stream)
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
