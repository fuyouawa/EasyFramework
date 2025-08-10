using System.Collections.Generic;
using System.Linq;
using EasyToolKit.ThirdParty.OdinSerializer;
using EasyToolKit.TileWorldPro;

[assembly: RegisterFormatter(typeof(BakedChunkFormatter))]
namespace EasyToolKit.TileWorldPro
{
    public class BakedChunkFormatter : MinimalBaseFormatter<BakedChunk>
    {
        private static readonly Serializer<ChunkArea> ChunkAreaSerializer = Serializer.Get<ChunkArea>();
        private static readonly Serializer<List<BakedChunk.TerrainSection>> BakedChunkTerrainSectionsSerializer = Serializer.Get<List<BakedChunk.TerrainSection>>();

        protected override void Read(ref BakedChunk value, IDataReader reader)
        {
            var area = ChunkAreaSerializer.ReadValue(reader);
            var terrainSections = BakedChunkTerrainSectionsSerializer.ReadValue(reader);
            value = new BakedChunk(area, terrainSections);
        }

        protected override void Write(ref BakedChunk value, IDataWriter writer)
        {
            ChunkAreaSerializer.WriteValue(value.Area, writer);
            BakedChunkTerrainSectionsSerializer.WriteValue(value.TerrainSections.ToList(), writer);
        }
    }
}