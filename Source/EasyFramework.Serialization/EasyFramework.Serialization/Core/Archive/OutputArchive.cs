using System.Collections.Generic;
using UnityEngine;

namespace EasyFramework.Serialization
{
    internal abstract class OutputArchive : IArchive
    {
        private readonly EasySerializeNative.OutputArchive _archive;

        private readonly List<Object> _referencedUnityObjects = new List<Object>();

        protected OutputArchive(EasySerializeNative.OutputArchive archive)
        {
            _archive = archive;
        }

        public ArchiveIoTypes ArchiveIoType => ArchiveIoTypes.Output;
        public abstract ArchiveTypes ArchiveType { get; }

        public List<Object> GetReferencedUnityObjects()
        {
            return _referencedUnityObjects;
        }
        
        public void SetNextName(string name)
        {
            EasySerializeNative.OutputArchiveSetNextName(_archive, name);
        }

        public void StartNode()
        {
            EasySerializeNative.OutputArchiveStartNode(_archive);
        }

        public void FinishNode()
        {
            EasySerializeNative.OutputArchiveFinishNode(_archive);
        }

        public bool Process(ref int value)
        {
            EasySerializeNative.WriteInt32ToOutputArchive(_archive, value);
            return true;
        }

        public bool Process(ref Varint32 value)
        {
            EasySerializeNative.WriteVarint32ToOutputArchive(_archive, value.Value);
            return true;
        }

        public bool Process(ref SizeTag sizeTag)
        {
            EasySerializeNative.WriteSizeToOutputArchive(_archive, sizeTag.Size);
            return true;
        }

        public bool Process(ref bool value)
        {
            int val = value ? 1 : 0;
            EasySerializeNative.WriteBoolToOutputArchive(_archive, (byte)val);
            return true;
        }

        public bool Process(ref float value)
        {
            EasySerializeNative.WriteFloatToOutputArchive(_archive, value);
            return true;
        }

        public bool Process(ref double value)
        {
            EasySerializeNative.WriteDoubleToOutputArchive(_archive, value);
            return true;
        }

        public bool Process(ref string str)
        {
            EasySerializeNative.WriteStringToOutputArchive(_archive, str);
            return true;
        }

        public bool Process(ref byte[] data)
        {
            var cBuf = EasySerializeNative.ConvertBytesToBuffer(data);
            EasySerializeNative.WriteBinaryToOutputArchive(_archive, cBuf);
            EasySerializeNative.FreeBuffer(cBuf);
            return true;
        }

        public bool Process(ref Object unityObject)
        {
            if (unityObject == null)
            {
                EasySerializeNative.WriteVarint32ToOutputArchive(_archive, 0);
                return true;
            }

            var idx = _referencedUnityObjects.Count + 1;
            EasySerializeNative.WriteVarint32ToOutputArchive(_archive, (uint)idx);
            _referencedUnityObjects.Add(unityObject);
            return true;
        }

        public void Dispose()
        {
            EasySerializeNative.FreeOutputArchive(_archive);
        }
    }
}
