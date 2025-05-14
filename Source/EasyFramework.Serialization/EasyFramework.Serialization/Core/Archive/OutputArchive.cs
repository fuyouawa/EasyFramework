using System.Collections.Generic;
using EasyFramework.Core.Internal;
using UnityEngine;

namespace EasyFramework.Serialization
{
    internal abstract class OutputArchive : IArchive
    {
        private readonly NativeOutputArchive _archive;

        private readonly List<Object> _referencedUnityObjects = new List<Object>();

        protected OutputArchive(NativeOutputArchive archive)
        {
            _archive = archive;
        }

        public ArchiveIoType ArchiveIoType => ArchiveIoType.Output;
        public abstract ArchiveTypes ArchiveType { get; }

        public List<Object> GetReferencedUnityObjects()
        {
            return _referencedUnityObjects;
        }
        
        public void SetNextName(string name)
        {
            NativeEasySerialize.OutputArchiveSetNextNameSafety(_archive, name);
        }

        public void StartNode()
        {
            NativeEasySerialize.OutputArchiveStartNodeSafety(_archive);
        }

        public void FinishNode()
        {
            NativeEasySerialize.OutputArchiveFinishNodeSafety(_archive);
        }

        public bool Process(ref int value)
        {
            NativeEasySerialize.WriteInt32ToOutputArchiveSafety(_archive, value);
            return true;
        }

        public bool Process(ref Varint32 value)
        {
            NativeEasySerialize.WriteVarint32ToOutputArchiveSafety(_archive, value.Value);
            return true;
        }

        public bool Process(ref SizeTag sizeTag)
        {
            NativeEasySerialize.WriteSizeToOutputArchiveSafety(_archive, sizeTag.Size);
            return true;
        }

        public bool Process(ref bool value)
        {
            int val = value ? 1 : 0;
            NativeEasySerialize.WriteBoolToOutputArchiveSafety(_archive, (byte)val);
            return true;
        }

        public bool Process(ref float value)
        {
            NativeEasySerialize.WriteFloatToOutputArchiveSafety(_archive, value);
            return true;
        }

        public bool Process(ref double value)
        {
            NativeEasySerialize.WriteDoubleToOutputArchiveSafety(_archive, value);
            return true;
        }

        public bool Process(ref string str)
        {
            NativeEasySerialize.WriteStringToOutputArchiveSafety(_archive, str);
            return true;
        }

        public bool Process(ref byte[] data)
        {
            var cBuf = data.ToNativeBuffer();
            using (cBuf.GetWrapper())
            {
                NativeEasySerialize.WriteBinaryToOutputArchiveSafety(_archive, cBuf);
            }
            return true;
        }

        public bool Process(ref Object unityObject)
        {
            if (unityObject == null)
            {
                NativeEasySerialize.WriteVarint32ToOutputArchiveSafety(_archive, 0);
                return true;
            }

            var idx = _referencedUnityObjects.Count + 1;
            NativeEasySerialize.WriteVarint32ToOutputArchiveSafety(_archive, (uint)idx);
            _referencedUnityObjects.Add(unityObject);
            return true;
        }

        public void Dispose()
        {
            NativeEasySerialize.FreeOutputArchiveSafety(_archive);
        }
    }
}
