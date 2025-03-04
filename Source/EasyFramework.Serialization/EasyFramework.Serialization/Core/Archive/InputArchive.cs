using System.Collections.Generic;
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
        }

        public void StartNode()
        {
            EasySerializeNative.InputArchiveStartNode(_archive);
        }

        public void FinishNode()
        {
            EasySerializeNative.InputArchiveFinishNode(_archive);
        }

        public bool Process(ref int value)
        {
            value = EasySerializeNative.ReadInt32FromInputArchive(_archive);
            return true;
        }

        public bool Process(ref Varint32 value)
        {
            value.Value = EasySerializeNative.ReadVarint32FromInputArchive(_archive);
            return true;
        }

        public bool Process(ref SizeTag sizeTag)
        {
            sizeTag.Size = EasySerializeNative.ReadSizeFromInputArchive(_archive);
            return true;
        }

        public bool Process(ref bool value)
        {
            byte val = EasySerializeNative.ReadBoolFromInputArchive(_archive);
            value = val != 0;
            return true;
        }

        public bool Process(ref float value)
        {
            value = EasySerializeNative.ReadFloatFromInputArchive(_archive);
            return true;
        }

        public bool Process(ref double value)
        {
            value = EasySerializeNative.ReadDoubleFromInputArchive(_archive);
            return true;
        }

        public bool Process(ref string str)
        {
            var cBuf = EasySerializeNative.ReadStringFromInputArchive(_archive);
            var buf = EasySerializeNative.ConvertBufferToBytesWithFree(cBuf);

            str = Encoding.UTF8.GetString(buf);
            return true;
        }

        public bool Process(ref byte[] data)
        {
            var cBuf = EasySerializeNative.ReadBinaryFromInputArchive(_archive);
            data = EasySerializeNative.ConvertBufferToBytesWithFree(cBuf);
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
