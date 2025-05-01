using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using EasyFramework.Core;

namespace EasyFramework.Serialization
{
    internal abstract class InputArchive : IArchive
    {
        private readonly NativeInputArchive _archive;

        private List<UnityEngine.Object> _referencedUnityObjects;

        protected InputArchive(NativeInputArchive archive)
        {
            _archive = archive;
        }

        public ArchiveIoTypes ArchiveIoType => ArchiveIoTypes.Input;
        public abstract ArchiveTypes ArchiveType { get; }

        public void SetupReferencedUnityObjects(List<UnityEngine.Object> referencedUnityObjects)
        {
            _referencedUnityObjects = referencedUnityObjects;
        }


        public void SetNextName(string name)
        {
            NativeEasySerialize.InputArchiveSetNextNameSafety(_archive, name);
        }

        public void StartNode()
        {
            NativeEasySerialize.InputArchiveStartNodeSafety(_archive);
        }

        public void FinishNode()
        {
            NativeEasySerialize.InputArchiveFinishNodeSafety(_archive);
        }

        public bool Process(ref int value)
        {
            value = NativeEasySerialize.ReadInt32FromInputArchiveSafety(_archive);
            return true;
        }

        public bool Process(ref Varint32 value)
        {
            value.Value = NativeEasySerialize.ReadVarint32FromInputArchiveSafety(_archive);
            return true;
        }

        public bool Process(ref SizeTag sizeTag)
        {
            sizeTag.Size = NativeEasySerialize.ReadSizeFromInputArchiveSafety(_archive);
            return true;
        }

        public bool Process(ref bool value)
        {
            byte val = NativeEasySerialize.ReadBoolFromInputArchiveSafety(_archive);
            value = val != 0;
            return true;
        }

        public bool Process(ref float value)
        {
            value = NativeEasySerialize.ReadFloatFromInputArchiveSafety(_archive);
            return true;
        }

        public bool Process(ref double value)
        {
            value = NativeEasySerialize.ReadDoubleFromInputArchiveSafety(_archive);
            return true;
        }

        public bool Process(ref string str)
        {
            var cBuf = NativeEasySerialize.ReadStringFromInputArchiveSafety(_archive);
            str = cBuf.ToStringWithFree();
            return true;
        }

        public bool Process(ref byte[] data)
        {
            var cBuf = NativeEasySerialize.ReadBinaryFromInputArchiveSafety(_archive);
            data = cBuf.ToBytesWithFree();
            return true;
        }

        public bool Process(ref UnityEngine.Object unityObject)
        {
            var idx = NativeEasySerialize.ReadVarint32FromInputArchiveSafety(_archive);
            if (idx == 0)
            {
                unityObject = null;
                return true;
            }

            if (idx > _referencedUnityObjects.Count)
            {
                unityObject = null;
                return false;
            }

            unityObject = _referencedUnityObjects[(int)idx - 1];
            return true;
        }

        public void Dispose()
        {
            NativeEasySerialize.FreeInputArchiveSafety(_archive);
        }
    }
}
