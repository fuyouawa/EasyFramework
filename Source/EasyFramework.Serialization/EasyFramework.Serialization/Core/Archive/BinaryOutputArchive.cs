using EasyFramework.Core;
using EasyFramework.Core.Internal;

namespace EasyFramework.Serialization
{
    internal class BinaryOutputArchive : OutputArchive
    {
        public BinaryOutputArchive(NativeIoStream stream)
            : base(NativeEasySerialize.AllocBinaryOutputArchiveSafety(stream))
        {
        }

        public override ArchiveTypes ArchiveType => ArchiveTypes.Binary;
    }
}
