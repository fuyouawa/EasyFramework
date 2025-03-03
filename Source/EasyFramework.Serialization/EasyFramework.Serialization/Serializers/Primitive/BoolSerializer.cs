namespace EasyFramework.Serialization
{
    [EasySerializerConfig(EasySerializerProiority.Primitive)]
    public class BoolSerializer : EasySerializer<bool>
    {
        public override void Process(string name, ref bool value, IArchive archive)
        {
            archive.SetNextName(name);
            archive.Process(ref value);
        }
    }
}
