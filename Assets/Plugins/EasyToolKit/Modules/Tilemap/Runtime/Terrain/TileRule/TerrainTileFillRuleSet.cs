using System;
using EasyToolKit.Inspector;

namespace EasyToolKit.Tilemap
{
    [FoldoutGroup("填充规则集")]
    [HideLabel]
    [Serializable]
    public class TerrainTileFillRuleSet : TerrainTileRuleSetBase
    {
        [TerrainTileRuleType(TerrainType.Fill)]
        public TerrainTileRule FillRule;

        public override TerrainTileRuleSetType RuleSetType => TerrainTileRuleSetType.Fill;
    }
}
