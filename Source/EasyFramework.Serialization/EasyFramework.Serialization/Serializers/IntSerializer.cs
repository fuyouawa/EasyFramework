namespace EasyFramework.Serialization
{
    public class IntSerializer : EasySerializerBase<int>
    {
        protected override void Process(IArchive archive, ref int value)
        {
            archive.Process(ref value);
        }
    }
}
