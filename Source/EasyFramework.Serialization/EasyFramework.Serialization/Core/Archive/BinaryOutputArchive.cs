namespace EasyFramework.Serialization
{
    internal class BinaryOutputArchive : OutputArchive
    {
        public BinaryOutputArchive(GenericNative.IoStream stream)
            : base(EasySerializeNative.AllocBinaryOutputArchive(stream))
        {
        }

        public override ArchiveTypes ArchiveType => ArchiveTypes.Binary;
    }
}
