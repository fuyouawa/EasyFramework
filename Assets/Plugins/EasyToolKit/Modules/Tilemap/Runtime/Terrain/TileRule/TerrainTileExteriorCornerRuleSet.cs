using EasyToolKit.Inspector;
using System;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [FoldoutGroup("外转角规则集")]
    [HideLabel]
    [Serializable]
    public class TerrainTileExteriorCornerRuleSet : TerrainTileRuleSetBase
    {
        /// <summary>
        /// 使用完整定义。
        /// 如果为 false，则使用<see cref="TopLeftRule"/>作为基准规则，其他方向的规则将基于此规则进行调整。
        /// </summary>
        [LabelText("使用完整定义")]
        public bool UseFullDefinition;

        [TerrainTileRuleType(TerrainType.TopLeftExteriorCorner)]
        [Space(3)]
        public TerrainTileRule TopLeftRule;

        [TerrainTileRuleType(TerrainType.TopRightExteriorCorner)]
        [Space(3)]
        public TerrainTileRule TopRightRule;

        [TerrainTileRuleType(TerrainType.BottomLeftExteriorCorner)]
        [Space(3)]
        public TerrainTileRule BottomLeftRule;

        [TerrainTileRuleType(TerrainType.BottomRightExteriorCorner)]
        [Space(3)]
        public TerrainTileRule BottomRightRule;

        public override TerrainTileRuleSetType RuleSetType => TerrainTileRuleSetType.ExteriorCorner;
    }
}
