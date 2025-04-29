using EasyFramework.Core.Native;

namespace EasyFramework.Serialization
{
    internal class JsonOutputArchive : OutputArchive
    {
        public JsonOutputArchive(NativeIoStream stream)
            : base(NativeEasySerialize.AllocJsonOutputArchiveSafety(stream))
        {
        }

        public override ArchiveTypes ArchiveType => ArchiveTypes.Json;
    }
}
