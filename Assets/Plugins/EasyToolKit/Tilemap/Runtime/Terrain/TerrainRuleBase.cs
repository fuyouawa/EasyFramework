using UnityEngine;

namespace EasyToolKit.Tilemap
{
    public enum TerrainRuleType
    {
        Fill,
        Edge,
        ExteriorCorner,
        InteriorCorner,
    }

    public abstract class TerrainRuleBase
    {
        public abstract TerrainRuleType RuleType { get; }

        public abstract GameObject GetTileInstanceByRuleType(TerrainTileRuleType ruleType);
    }
}
