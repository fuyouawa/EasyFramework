namespace EasyFramework.Serialization
{
    [EasySerializerConfig(EasySerializerProiority.Primitive)]
    public class StringSerializer : EasySerializer<string>
    {
        public override void Process(string name, ref string value, IArchive archive)
        {
            archive.SetNextName(name);
            archive.Process(ref value);
        }
    }
}
