namespace EasyFramework.Serialization
{
    [EasySerializerPriority(EasySerializerProiority.System)]
    public class FloatSerializer : EasySerializerBase<float>
    {
        protected override void Process(IArchive archive, ref float value)
        {
            archive.Process(ref value);
        }
    }
}
