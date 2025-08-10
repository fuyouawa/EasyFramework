using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.ThirdParty.OdinSerializer;
using EasyToolKit.TileWorldPro;

[assembly: RegisterFormatter(typeof(ChunkTerrainSectionFormatter))]
namespace EasyToolKit.TileWorldPro
{
    public class ChunkTerrainSectionFormatter : MinimalBaseFormatter<Chunk.TerrainSection>
    {
        private static readonly Serializer<Guid> GuidSerializer = Serializer.Get<Guid>();
        private static readonly Serializer<ushort> UShortSerializer = Serializer.Get<ushort>();
        private static readonly Serializer<uint> UIntSerializer = Serializer.Get<uint>();

        protected override void Read(ref Chunk.TerrainSection value, IDataReader reader)
        {
            var terrainGuid = GuidSerializer.ReadValue(reader);
            var tiles = new HashSet<ChunkTilePosition>();
            var tileCount = UIntSerializer.ReadValue(reader);
            for (int i = 0; i < tileCount; i++)
            {
                var x = UShortSerializer.ReadValue(reader);
                var y = UShortSerializer.ReadValue(reader);
                var z = UShortSerializer.ReadValue(reader);
                tiles.Add(new ChunkTilePosition(x, y, z));
            }
            value = new Chunk.TerrainSection(terrainGuid, tiles);
        }

        protected override void Write(ref Chunk.TerrainSection value, IDataWriter writer)
        {
            GuidSerializer.WriteValue(value.TerrainGuid, writer);
            UIntSerializer.WriteValue((uint)value.Tiles.Count, writer);
            foreach (var tile in value.Tiles)
            {
                UShortSerializer.WriteValue(tile.X, writer);
                UShortSerializer.WriteValue(tile.Y, writer);
                UShortSerializer.WriteValue(tile.Z, writer);
            }
        }
    }
}