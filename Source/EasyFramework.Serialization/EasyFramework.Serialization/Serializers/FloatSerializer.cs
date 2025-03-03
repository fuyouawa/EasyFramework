namespace EasyFramework.Serialization
{
    [EasySerializerConfig(EasySerializerProiority.Value)]
    public class FloatSerializer : EasySerializer<float>
    {
        protected override void Process(IArchive archive, ref float value)
        {
            archive.Process(ref value);
        }
    }
}
