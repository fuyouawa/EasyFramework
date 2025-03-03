namespace EasyFramework.Serialization
{
    [EasySerializerConfig(EasySerializerProiority.Value)]
    public class StringSerializer : EasySerializer<string>
    {
        protected override void Process(IArchive archive, ref string value)
        {
            archive.Process(ref value);
        }
    }
}
