using EasyToolKit.ThirdParty.OdinSerializer;
using EasyToolKit.TileWorldPro;

[assembly: RegisterFormatter(typeof(ChunkTilePositionFormatter))]
namespace EasyToolKit.TileWorldPro
{
    public class ChunkTilePositionFormatter : MinimalBaseFormatter<ChunkTilePosition>
    {
        private static readonly Serializer<ushort> UShortSerializer = Serializer.Get<ushort>();

        protected override void Read(ref ChunkTilePosition value, IDataReader reader)
        {
            var x = UShortSerializer.ReadValue(reader);
            var y = UShortSerializer.ReadValue(reader);
            var z = UShortSerializer.ReadValue(reader);
            value = new ChunkTilePosition(x, y, z);
        }

        protected override void Write(ref ChunkTilePosition value, IDataWriter writer)
        {
            UShortSerializer.WriteValue(value.X, writer);
            UShortSerializer.WriteValue(value.Y, writer);
            UShortSerializer.WriteValue(value.Z, writer);
        }
    }
}