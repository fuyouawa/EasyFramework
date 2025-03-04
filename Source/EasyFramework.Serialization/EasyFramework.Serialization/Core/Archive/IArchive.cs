using System;

namespace EasyFramework.Serialization
{
    public enum ArchiveIoTypes
    {
        Input,
        Output
    }

    public enum ArchiveTypes
    {
        Binary,
        Json,
        Xml,
        Yaml
    }

    public struct Varint32
    {
        public uint Value { get; set; }

        public Varint32(uint value)
        {
            Value = value;
        }
    }

    public struct SizeTag
    {
        public uint Size { get; set; }

        public SizeTag(uint size)
        {
            Size = size;
        }
    }

    public interface IArchive : IDisposable
    {
        ArchiveIoTypes ArchiveIoType { get; }
        ArchiveTypes ArchiveType { get; }

        void SetNextName(string name);
        void StartNode();
        void FinishNode();

        bool Process(ref int value);
        bool Process(ref Varint32 value);
        bool Process(ref SizeTag sizeTag);
        bool Process(ref bool value);
        bool Process(ref float value);
        bool Process(ref double value);
        bool Process(ref string str);
        bool Process(ref byte[] data);
        bool Process(ref UnityEngine.Object unityObject);
    }
}
