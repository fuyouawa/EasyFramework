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
        private static readonly Serializer<HashSet<ChunkTilePosition>> ChunkTilePositionsSerializer = Serializer.Get<HashSet<ChunkTilePosition>>();

        protected override void Read(ref Chunk.TerrainSection value, IDataReader reader)
        {
            var terrainGuid = GuidSerializer.ReadValue(reader);
            var tiles = ChunkTilePositionsSerializer.ReadValue(reader);
            value = new Chunk.TerrainSection(terrainGuid, tiles);
        }

        protected override void Write(ref Chunk.TerrainSection value, IDataWriter writer)
        {
            GuidSerializer.WriteValue(value.TerrainGuid, writer);
            ChunkTilePositionsSerializer.WriteValue(value.Tiles, writer);
        }
    }
}