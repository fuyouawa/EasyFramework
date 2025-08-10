using EasyToolKit.ThirdParty.OdinSerializer;
using EasyToolKit.TileWorldPro;

[assembly: RegisterFormatter(typeof(ChunkPositionFormatter))]
namespace EasyToolKit.TileWorldPro
{
    public class ChunkPositionFormatter : MinimalBaseFormatter<ChunkPosition>
    {
        private static readonly Serializer<ushort> UShortSerializer = Serializer.Get<ushort>();

        protected override void Read(ref ChunkPosition value, IDataReader reader)
        {
            var x = UShortSerializer.ReadValue(reader);
            var y = UShortSerializer.ReadValue(reader);
            value = new ChunkPosition(x, y);
        }

        protected override void Write(ref ChunkPosition value, IDataWriter writer)
        {
            UShortSerializer.WriteValue(value.X, writer);
            UShortSerializer.WriteValue(value.Y, writer);
        }
    }
}