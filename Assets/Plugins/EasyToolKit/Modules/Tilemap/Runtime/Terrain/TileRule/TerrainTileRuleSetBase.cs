using System;

namespace EasyToolKit.Tilemap
{
    public enum TerrainTileRuleSetType
    {
        Fill,
        Edge,
        ExteriorCorner,
        InteriorCorner,
    }

    public abstract class TerrainTileRuleSetBase
    {
        public abstract TerrainTileRuleSetType RuleSetType { get; }
    }
}
