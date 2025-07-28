using System;
using UnityEngine;

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

        public abstract GameObject GetTileInstanceByRuleType(TerrainRuleType ruleType);
    }
}
