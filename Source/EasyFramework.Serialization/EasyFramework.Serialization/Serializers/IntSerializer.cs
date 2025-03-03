namespace EasyFramework.Serialization
{
    [EasySerializerPriority(EasySerializerProiority.System)]
    public class IntSerializer : EasySerializerBase<int>
    {
        protected override void Process(IArchive archive, ref int value)
        {
            archive.Process(ref value);
        }
    }
}
