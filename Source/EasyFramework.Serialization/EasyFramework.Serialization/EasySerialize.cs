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
            return To(value, EasyDataFormat.Binary).BinaryData;
        }

        public static T FromBinary<T>(byte[] data)
        {
            return From<T>(new EasySerializationData(data, EasyDataFormat.Binary));
        }

        public static string ToJson<T>(T value)
        {
            return To(value, EasyDataFormat.Json).StringData;
        }

        public static T FromJson<T>(string json)
        {
            return From<T>(new EasySerializationData(json, EasyDataFormat.Json));
        }

        public static EasySerializationData To<T>(T value, EasyDataFormat format)
        {
            var data = new EasySerializationData(format);

            var ios = EasySerializeNative.AllocStringIoStream();
            using (new EasySerializeNative.IoStreamWrapper(ios))
            {
                using (var arch = GetOutputArchive(format, ios))
                {
                    var serializer = GetSerializerWithThrow<T>();
                    ((IEasySerializer<T>)serializer).IsRoot = true;
                    serializer.Process(ref value, arch);
                    data.ReferencedUnityObjects = arch.GetReferencedUnityObjects();
                }

                var cBuf = EasySerializeNative.GetIoStreamBuffer(ios);
                data.SetData(EasySerializeNative.ConvertBufferToBytesWithFree(cBuf));
            }

            return data;
        }

        public static T From<T>(EasySerializationData data)
        {
            T ret = default;
            From(ref ret, data);
            return ret;
        }

        public static void From<T>(ref T referencedValue, EasySerializationData data)
        {
            if (data == null)
            {
                return;
            }
            var buf = data.GetData();
            if (buf.Length == 0)
            {
                return;
            }

            var ios = EasySerializeNative.AllocStringIoStream();
            using (new EasySerializeNative.IoStreamWrapper(ios))
            {
                var cBuf = EasySerializeNative.ConvertBytesToBuffer(buf);
                using (new EasySerializeNative.BufferWrapper(cBuf))
                {
                    EasySerializeNative.WriteToIoStreamBuffer(ios, cBuf);
                }

                using (var arch = GetInputArchive(data.Format, ios))
                {
                    arch.SetupReferencedUnityObjects(data.ReferencedUnityObjects);

                    var serializer = GetSerializerWithThrow<T>();
                    ((IEasySerializer<T>)serializer).IsRoot = true;
                    serializer.Process(ref referencedValue, arch);
                }
            }
        }

        private static EasySerializer<T> GetSerializerWithThrow<T>()
        {
            var serializer = EasySerializationUtility.GetSerializer<T>();
            if (serializer == null)
            {
                throw new ArgumentException(
                    $"There is no serializer for type '{typeof(T).FullName}'." +
                    "You need to implement a 'IEasySerializer' for type.");
            }

            return serializer;
        }

        private static OutputArchive GetOutputArchive(EasyDataFormat format, EasySerializeNative.IoStream stream)
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

        private static InputArchive GetInputArchive(EasyDataFormat format, EasySerializeNative.IoStream stream)
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
