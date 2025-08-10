using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.ThirdParty.OdinSerializer;
using EasyToolKit.TileWorldPro;

[assembly: RegisterFormatter(typeof(BakedChunkTerrainSectionFormatter))]
namespace EasyToolKit.TileWorldPro
{
    public class BakedChunkTerrainSectionFormatter : MinimalBaseFormatter<BakedChunk.TerrainSection>
    {
        private static readonly Serializer<Guid> GuidSerializer = Serializer.Get<Guid>();
        private static readonly Serializer<List<BakedChunk.TerrainSection.Tile>> BakedChunkTerrainSectionTilesSerializer = Serializer.Get<List<BakedChunk.TerrainSection.Tile>>();

        protected override void Read(ref BakedChunk.TerrainSection value, IDataReader reader)
        {
            var terrainGuid = GuidSerializer.ReadValue(reader);
            var tiles = BakedChunkTerrainSectionTilesSerializer.ReadValue(reader);
            value = new BakedChunk.TerrainSection(terrainGuid, tiles);
        }

        protected override void Write(ref BakedChunk.TerrainSection value, IDataWriter writer)
        {
            GuidSerializer.WriteValue(value.TerrainGuid, writer);
            BakedChunkTerrainSectionTilesSerializer.WriteValue(value.Tiles.Values.ToList(), writer);
        }
    }
}