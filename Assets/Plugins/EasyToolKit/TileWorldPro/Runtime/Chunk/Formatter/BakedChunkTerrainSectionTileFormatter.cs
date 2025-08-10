using EasyToolKit.ThirdParty.OdinSerializer;
using EasyToolKit.TileWorldPro;

[assembly: RegisterFormatter(typeof(BakedChunkTerrainSectionTileFormatter))]
namespace EasyToolKit.TileWorldPro
{
    public class BakedChunkTerrainSectionTileFormatter : MinimalBaseFormatter<BakedChunk.TerrainSection.Tile>
    {
        private static readonly Serializer<ChunkTilePosition> ChunkTilePositionSerializer = Serializer.Get<ChunkTilePosition>();
        private static readonly Serializer<TerrainTileRuleType> TerrainTileRuleTypeSerializer = Serializer.Get<TerrainTileRuleType>();
        private static readonly Serializer<bool> BoolSerializer = Serializer.Get<bool>();

        protected override void Read(ref BakedChunk.TerrainSection.Tile value, IDataReader reader)
        {
            var tilePosition = ChunkTilePositionSerializer.ReadValue(reader);
            var ruleType = TerrainTileRuleTypeSerializer.ReadValue(reader);
            var canBeHidden = BoolSerializer.ReadValue(reader);
            value = new BakedChunk.TerrainSection.Tile(tilePosition, ruleType, canBeHidden);
        }

        protected override void Write(ref BakedChunk.TerrainSection.Tile value, IDataWriter writer)
        {
            ChunkTilePositionSerializer.WriteValue(value.TilePosition, writer);
            TerrainTileRuleTypeSerializer.WriteValue(value.RuleType, writer);
            BoolSerializer.WriteValue(value.CanBeHidden, writer);
        }
    }
}