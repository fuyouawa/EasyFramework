using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EasyFramework.Serialization
{
    public enum EasyDataFormat
    {
        Binary,
        Json,
        Xml,
        Yaml
    }


    [Serializable]
    public class EasySerializationData
    {
        [SerializeField] public EasyDataFormat Format;
        [SerializeField] public byte[] BinaryData;
        [SerializeField] public string StringData;
        [SerializeField] public List<UnityEngine.Object> ReferencedUnityObjects;

        public EasySerializationData(EasyDataFormat format)
        {
            if (format == EasyDataFormat.Binary)
            {
                BinaryData = Array.Empty<byte>();
            }
            else
            {
                StringData = string.Empty;
            }
            ReferencedUnityObjects = new List<UnityEngine.Object>();
        }

        public EasySerializationData(byte[] binaryData, EasyDataFormat format)
            : this(binaryData, new List<UnityEngine.Object>(), format)
        {
        }

        public EasySerializationData(string stringData, EasyDataFormat format)
            : this(stringData, new List<UnityEngine.Object>(), format)
        {
        }

        public EasySerializationData(byte[] binaryData, List<UnityEngine.Object> referencedUnityObjects, EasyDataFormat format)
        {
            if (format != EasyDataFormat.Binary)
            {
                throw new ArgumentException("Binary data can only be serialized by the EasyDataFormat.Binary mode");
            }
            BinaryData = binaryData;
            ReferencedUnityObjects = referencedUnityObjects;
            Format = format;
        }

        public EasySerializationData(string stringData, List<UnityEngine.Object> referencedUnityObjects, EasyDataFormat format)
        {
            if (format == EasyDataFormat.Binary)
            {
                throw new ArgumentException("String data can not be serialized by the EasyDataFormat.Binary mode");
            }
            StringData = stringData;
            ReferencedUnityObjects = referencedUnityObjects;
            Format = format;
        }

        public byte[] GetData()
        {
            if (Format == EasyDataFormat.Binary)
            {
                return BinaryData;
            }

            if (string.IsNullOrEmpty(StringData))
            {
                return Array.Empty<byte>();
            }
            return Encoding.UTF8.GetBytes(StringData);
        }

        public void SetData(byte[] data)
        {
            if (Format == EasyDataFormat.Binary)
            {
                BinaryData = data;
            }
            else
            {
                StringData = Encoding.UTF8.GetString(data);
            }
        }
    }
}
