namespace EasyFramework.Serialization
{
    internal class JsonOutputArchive : OutputArchive
    {
        public JsonOutputArchive(EasySerializeNative.IoStream stream)
            : base(EasySerializeNative.AllocJsonOutputArchive(stream))
        {
        }

        public override ArchiveTypes ArchiveType => ArchiveTypes.Json;
    }
}
