namespace EasyFramework.Serialization
{
    internal class JsonInputArchive : InputArchive
    {
        public JsonInputArchive(EasySerializeNative.IoStream stream)
            : base(EasySerializeNative.AllocJsonInputArchive(stream))
        {
        }

        public override ArchiveTypes ArchiveType => ArchiveTypes.Json;
    }
}
