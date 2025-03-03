using System;
using System.Collections.Generic;
using System.Text;
using Object = UnityEngine.Object;

namespace EasyFramework.Serialization
{
    public static class EasySerialize
    {
        public static byte[] ToBinary<T>(T value)
        {
            return ToBinary(value, out _);
        }

        public static T FromBinary<T>(byte[] data)
        {
            return FromBinary<T>(data, new List<Object>());
        }

        public static byte[] ToBinary<T>(T value, out List<Object> referencedUnityObjects)
        {
            return To(value, out referencedUnityObjects, Format.Binary);
        }

        public static T FromBinary<T>(byte[] data, List<Object> referencedUnityObjects)
        {
            return From<T>(data, referencedUnityObjects, Format.Binary);
        }
        
        public static string ToJson<T>(T value)
        {
            return ToJson(value, out _);
        }

        public static T FromJson<T>(string json)
        {
            return FromJson<T>(json, new List<Object>());
        }

        public static string ToJson<T>(T value, out List<Object> referencedUnityObjects)
        {
            var data = To(value, out referencedUnityObjects, Format.Json);
            return Encoding.UTF8.GetString(data);
        }

        public static T FromJson<T>(string json, List<Object> referencedUnityObjects)
        {
            var data = Encoding.UTF8.GetBytes(json);
            return From<T>(data, referencedUnityObjects, Format.Json);
        }

        private enum Format
        {
            Binary,
            Json,
            Xml,
            Yaml
        }

        private static IEasySerializer<T> GetSerializerWithThrow<T>()
        {
            var serializer = EasySerializerUtility.GetSerializer<T>();
            if (serializer == null)
            {
                throw new ArgumentException(
                    $"There is no serializer for type '{typeof(T).FullName}'." +
                    "You need to implement a 'IEasySerializer' for type.");
            }

            return serializer;
        }

        private static OutputArchive GetOutputArchive(Format format, EasySerializeNative.IoStream stream)
        {
            return format switch
            {
                Format.Binary => new BinaryOutputArchive(stream),
                Format.Json => new JsonOutputArchive(stream),
                Format.Xml => throw new NotImplementedException(),
                Format.Yaml => throw new NotImplementedException(),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }

        private static InputArchive GetInputArchive(Format format, EasySerializeNative.IoStream stream)
        {
            return format switch
            {
                Format.Binary => new BinaryInputArchive(stream),
                Format.Json => new JsonInputArchive(stream),
                Format.Xml => throw new NotImplementedException(),
                Format.Yaml => throw new NotImplementedException(),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };
        }

        private static byte[] To<T>(T value, out List<Object> referencedUnityObjects, Format format)
        {
            var serializer = GetSerializerWithThrow<T>();

            var ios = EasySerializeNative.AllocStringIoStream();
            using (new EasySerializeNative.IoStreamWrapper(ios))
            {
                using (var arch = GetOutputArchive(format, ios))
                {
                    serializer.Process(ref value, arch);
                    referencedUnityObjects = arch.GetReferencedUnityObjects();
                }

                var cBuf = EasySerializeNative.GetIoStreamBuffer(ios);
                var buf = EasySerializeNative.ConvertBufferToBytesWithFree(cBuf);

                return buf;
            }
        }

        private static T From<T>(byte[] data, List<Object> referencedUnityObjects, Format format)
        {
            var serializer = GetSerializerWithThrow<T>();

            var ios = EasySerializeNative.AllocStringIoStream();
            using (new EasySerializeNative.IoStreamWrapper(ios))
            {
                var cBuf = EasySerializeNative.ConvertBytesToBuffer(data);
                using (new EasySerializeNative.BufferWrapper(cBuf))
                {
                    EasySerializeNative.WriteToIoStreamBuffer(ios, cBuf);
                }

                var ret = default(T);
                using (var arch = GetInputArchive(format, ios))
                {
                    arch.SetupReferencedUnityObjects(referencedUnityObjects);
                    serializer.Process(ref ret, arch);
                }

                return ret;
            }
        }
    }
}
