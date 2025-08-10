using EasyToolKit.ThirdParty.OdinSerializer;
using EasyToolKit.TileWorldPro;
using UnityEngine;

[assembly: RegisterFormatter(typeof(ChunkAreaFormatter))]
namespace EasyToolKit.TileWorldPro
{
    public class ChunkAreaFormatter : MinimalBaseFormatter<ChunkArea>
    {
        private static readonly Serializer<ChunkPosition> ChunkPositionSerializer = Serializer.Get<ChunkPosition>();
        private static readonly Serializer<Vector2Int> Vector2IntSerializer = Serializer.Get<Vector2Int>();

        protected override void Read(ref ChunkArea value, IDataReader reader)
        {
            var position = ChunkPositionSerializer.ReadValue(reader);
            var size = Vector2IntSerializer.ReadValue(reader);
            value = new ChunkArea(position, size);
        }

        protected override void Write(ref ChunkArea value, IDataWriter writer)
        {
            ChunkPositionSerializer.WriteValue(value.Position, writer);
            Vector2IntSerializer.WriteValue(value.Size, writer);
        }
    }
}