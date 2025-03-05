namespace EasyFramework.Serialization
{
    internal class JsonInputArchive : InputArchive
    {
        public JsonInputArchive(GenericNative.IoStream stream)
            : base(EasySerializeNative.AllocJsonInputArchive(stream))
        {
        }

        public override ArchiveTypes ArchiveType => ArchiveTypes.Json;
    }
}
