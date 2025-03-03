namespace EasyFramework.Serialization
{
    [EasySerializerConfig(EasySerializerProiority.Value)]
    public class IntSerializer : EasySerializer<int>
    {
        protected override void Process(IArchive archive, ref int value)
        {
            archive.Process(ref value);
        }
    }
}
