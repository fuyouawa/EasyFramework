using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace EasyFramework.Serialization
{
    public class EasySerialize
    {
        public byte[] ToBinary<T>(T value, out List<Object> referencedUnityObjects)
        {
            return To(value, out referencedUnityObjects, Format.Binary);
        }

        public T FromBinary<T>(byte[] data, List<Object> referencedUnityObjects)
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

        private IEasySerializer<T> GetSerializerWithThrow<T>()
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

        private byte[] To<T>(T value, out List<Object> referencedUnityObjects, Format format)
        {
            var serializer = GetSerializerWithThrow<T>();

            var ios = EasySerializeNative.AllocStringIoStream();
            using (new EasySerializeNative.IoStreamWrapper(ios))
            {
                using var arch = new BinaryOutputArchive(ios);
                serializer.Process(arch, null, ref value);

                var cBuf = EasySerializeNative.GetIoStreamBuffer(ios);
                var buf = EasySerializeNative.ConvertBufferToBytesWithFree(cBuf);

                referencedUnityObjects = arch.GetReferencedUnityObjects();
                return buf;
            }
        }

        private T From<T>(byte[] data, List<Object> referencedUnityObjects, Format format)
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

                using var arch = new BinaryInputArchive(ios);
                arch.SetupReferencedUnityObjects(referencedUnityObjects);

                var ret = default(T);
                serializer.Process(arch, null, ref ret);
                return ret;
            }
        }
    }
}
