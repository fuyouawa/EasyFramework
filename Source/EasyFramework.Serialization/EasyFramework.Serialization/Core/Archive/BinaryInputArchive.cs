using EasyFramework.Core;

namespace EasyFramework.Serialization
{
    internal class BinaryInputArchive : InputArchive
    {
        public BinaryInputArchive(NativeIoStream stream)
            : base(NativeEasySerialize.AllocBinaryInputArchiveSafety(stream))
        {
        }

        public override ArchiveTypes ArchiveType => ArchiveTypes.Binary;
    }
}
