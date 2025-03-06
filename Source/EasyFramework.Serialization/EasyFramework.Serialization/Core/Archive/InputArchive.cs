using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace EasyFramework.Serialization
{
    internal abstract class InputArchive : IArchive
    {
        private readonly EasySerializeNative.InputArchive _archive;

        private List<UnityEngine.Object> _referencedUnityObjects;

        protected InputArchive(EasySerializeNative.InputArchive archive)
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
            EasySerializeNative.InputArchiveSetNextName(_archive, name);
            NativeUtility.HandleSerializerError();
        }

        public void StartNode()
        {
            EasySerializeNative.InputArchiveStartNode(_archive);
            NativeUtility.HandleSerializerError();
        }

        public void FinishNode()
        {
            EasySerializeNative.InputArchiveFinishNode(_archive);
            NativeUtility.HandleSerializerError();
        }

        public bool Process(ref int value)
        {
            value = EasySerializeNative.ReadInt32FromInputArchive(_archive);
            NativeUtility.HandleSerializerError();
            return true;
        }

        public bool Process(ref Varint32 value)
        {
            value.Value = EasySerializeNative.ReadVarint32FromInputArchive(_archive);
            NativeUtility.HandleSerializerError();
            return true;
        }

        public bool Process(ref SizeTag sizeTag)
        {
            sizeTag.Size = EasySerializeNative.ReadSizeFromInputArchive(_archive);
            NativeUtility.HandleSerializerError();
            return true;
        }

        public bool Process(ref bool value)
        {
            byte val = EasySerializeNative.ReadBoolFromInputArchive(_archive);
            NativeUtility.HandleSerializerError();
            value = val != 0;
            return true;
        }

        public bool Process(ref float value)
        {
            value = EasySerializeNative.ReadFloatFromInputArchive(_archive);
            NativeUtility.HandleSerializerError();
            return true;
        }

        public bool Process(ref double value)
        {
            value = EasySerializeNative.ReadDoubleFromInputArchive(_archive);
            NativeUtility.HandleSerializerError();
            return true;
        }

        public bool Process(ref string str)
        {
            var cBuf = EasySerializeNative.ReadStringFromInputArchive(_archive);
            NativeUtility.HandleSerializerError();
            str = cBuf.ToStringWithFree();
            return true;
        }

        public bool Process(ref byte[] data)
        {
            var cBuf = EasySerializeNative.ReadBinaryFromInputArchive(_archive);
            NativeUtility.HandleSerializerError();
            data = cBuf.ToBytesWithFree();
            return true;
        }

        public bool Process(ref UnityEngine.Object unityObject)
        {
            var idx = EasySerializeNative.ReadVarint32FromInputArchive(_archive);
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
            EasySerializeNative.FreeInputArchive(_archive);
        }
    }
}
