namespace EasyFramework.Serialization
{
    [EasySerializerConfig(EasySerializerProiority.Value)]
    public class BoolSerializer : EasySerializer<bool>
    {
        protected override void Process(IArchive archive, ref bool value)
        {
            archive.Process(ref value);
        }
    }
}
