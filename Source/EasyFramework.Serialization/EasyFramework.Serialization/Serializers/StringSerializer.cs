namespace EasyFramework.Serialization
{
    [EasySerializerPriority(EasySerializerProiority.System)]
    public class StringSerializer : EasySerializerBase<string>
    {
        protected override void Process(IArchive archive, ref string value)
        {
            archive.Process(ref value);
        }
    }
}
