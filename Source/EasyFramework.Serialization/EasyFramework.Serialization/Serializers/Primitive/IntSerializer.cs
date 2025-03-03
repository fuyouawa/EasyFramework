namespace EasyFramework.Serialization
{
    [EasySerializerConfig(EasySerializerProiority.Primitive)]
    public class IntSerializer : EasySerializer<int>
    {
        public override void Process(string name, ref int value, IArchive archive)
        {
            archive.SetNextName(name);
            archive.Process(ref value);
        }
    }
}
