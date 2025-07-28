using System;

namespace EasyToolKit.Tilemap
{
    public enum TerrainTileRuleType
    {
        Fill,
        Edge,
        ExteriorCorner,
        InteriorCorner,
    }

    public abstract class TerrainTileRuleBase
    {
        public abstract TerrainTileRuleType RuleType { get; }
    }
}
