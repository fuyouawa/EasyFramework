namespace EasyFramework.Serialization
{
    internal class BinaryInputArchive : InputArchive
    {
        public BinaryInputArchive(EasySerializeNative.IoStream stream)
            : base(EasySerializeNative.AllocBinaryInputArchive(stream))
        {
        }
    }
}
