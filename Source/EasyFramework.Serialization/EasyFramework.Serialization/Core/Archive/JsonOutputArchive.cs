namespace EasyFramework.Serialization
{
    internal class JsonOutputArchive : OutputArchive
    {
        public JsonOutputArchive(GenericNative.IoStream stream)
            : base(EasySerializeNative.AllocJsonOutputArchive(stream))
        {
        }

        public override ArchiveTypes ArchiveType => ArchiveTypes.Json;
    }
}
