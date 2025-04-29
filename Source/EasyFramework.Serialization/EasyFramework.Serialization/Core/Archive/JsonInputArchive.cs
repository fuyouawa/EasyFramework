using EasyFramework.Core.Native;

namespace EasyFramework.Serialization
{
    internal class JsonInputArchive : InputArchive
    {
        public JsonInputArchive(NativeIoStream stream)
            : base(NativeEasySerialize.AllocJsonInputArchiveSafety(stream))
        {
        }

        public override ArchiveTypes ArchiveType => ArchiveTypes.Json;
    }
}
