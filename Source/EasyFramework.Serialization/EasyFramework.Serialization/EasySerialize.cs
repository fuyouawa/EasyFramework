using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace EasyFramework.Serialization
{
    public static class EasySerialize
    {
        public static byte[] ToBinary<T>(T value)
        {
            return To(value, out _, Format.Binary);
        }

        public static T FromBinary<T>(byte[] data)
        {
            return From<T>(data, new List<Object>(), Format.Binary);
        }

        public static byte[] ToBinary<T>(T value, out List<Object> referencedUnityObjects)
        {
            return To(value, out referencedUnityObjects, Format.Binary);
        }

        public static T FromBinary<T>(byte[] data, List<Object> referencedUnityObjects)
        {
            return From<T>(data, referencedUnityObjects, Format.Binary);
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

        private static byte[] To<T>(T value, out List<Object> referencedUnityObjects, Format format)
        {
            var serializer = GetSerializerWithThrow<T>();

            var ios = EasySerializeNative.AllocStringIoStream();
            using (new EasySerializeNative.IoStreamWrapper(ios))
            {
                using (var arch = new BinaryOutputArchive(ios))
                {
                    serializer.Process(arch, ref value);
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
                using (var arch = new BinaryInputArchive(ios))
                {
                    arch.SetupReferencedUnityObjects(referencedUnityObjects);
                    serializer.Process(arch, ref ret);
                }

                return ret;
            }
        }
    }
}
