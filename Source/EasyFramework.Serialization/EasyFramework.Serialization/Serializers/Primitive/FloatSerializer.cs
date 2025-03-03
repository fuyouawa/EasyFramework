namespace EasyFramework.Serialization
{
    [EasySerializerConfig(EasySerializerProiority.Primitive)]
    public class FloatSerializer : EasySerializer<float>
    {
        public override void Process(string name, ref float value, IArchive archive)
        {
            archive.SetNextName(name);
            archive.Process(ref value);
        }
    }
}
