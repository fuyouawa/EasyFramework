using System;

namespace EasyFramework.Serialization
{
    public static class EasySerialize
    {
        public static EasySerializeSettings DefaultSettings { get; set; } = new EasySerializeSettings();
        internal static EasySerializeSettings CurrentSettings { get; private set; }

        public static byte[] ToBinary<T>(T value)
        {
            var data = new EasySerializationData(EasyDataFormat.Binary);
            To(value, ref data);
            return data.BinaryData;
        }

        public static T FromBinary<T>(byte[] data)
        {
            var d = new EasySerializationData(data, EasyDataFormat.Binary);
            return From<T>(ref d);
        }

        public static string ToJson<T>(T value)
        {
            var data = new EasySerializationData(EasyDataFormat.Json);
            To(value, ref data);
            return data.StringData;
        }

        public static T FromJson<T>(string json)
        {
            var d = new EasySerializationData(json, EasyDataFormat.Json);
            return From<T>(ref d);
        }

        public static void To<T>(T value, ref EasySerializationData serializationData, EasySerializeSettings settings = null)
        {
            CurrentSettings = settings ?? DefaultSettings;

            var ios = GenericNative.AllocStringIoStream();
            using (new GenericNative.IoStreamWrapper(ios))
            {
                using (var arch = GetOutputArchive(serializationData.Format, ios))
                {
                    var serializer = GetSerializerWithThrow<T>();
                    ((IEasySerializer<T>)serializer).IsRoot = true;
                    serializer.Process(ref value, arch);
                    serializationData.ReferencedUnityObjects = arch.GetReferencedUnityObjects();
                }

                var cBuf = GenericNative.GetIoStreamBuffer(ios);
                serializationData.SetData(GenericNative.ConvertBufferToBytesWithFree(cBuf));
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
            using (new GenericNative.IoStreamWrapper(ios))
            {
                var cBuf = GenericNative.ConvertBytesToBuffer(buf);
                using (new GenericNative.BufferWrapper(cBuf))
                {
                    GenericNative.WriteToIoStreamBuffer(ios, cBuf);
                }

                using (var arch = GetInputArchive(serializationData.Format, ios))
                {
                    arch.SetupReferencedUnityObjects(serializationData.ReferencedUnityObjects);

                    var serializer = GetSerializerWithThrow<T>();
                    ((IEasySerializer<T>)serializer).IsRoot = true;
                    serializer.Process(ref res, arch);
                }
            }

            return res;
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
