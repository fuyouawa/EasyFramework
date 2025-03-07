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
    public struct EasySerializationData
    {
        [SerializeField] public EasyDataFormat Format;
        [SerializeField] public byte[] BinaryData;
        [SerializeField] public string StringData;
        [SerializeField] public List<UnityEngine.Object> ReferencedUnityObjects;

        public bool ContainsData => BinaryData != null || StringData != null || ReferencedUnityObjects != null;

        public EasySerializationData(EasyDataFormat format)
        {
            Format = format;
            if (format == EasyDataFormat.Binary)
            {
                BinaryData = Array.Empty<byte>();
                StringData = null;
            }
            else
            {
                StringData = string.Empty;
                BinaryData = null;
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
            StringData = null;
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
            BinaryData = null;
            ReferencedUnityObjects = referencedUnityObjects;
            Format = format;
        }

        public byte[] GetData()
        {
            if (!ContainsData)
            {
                return Array.Empty<byte>();
            }

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
