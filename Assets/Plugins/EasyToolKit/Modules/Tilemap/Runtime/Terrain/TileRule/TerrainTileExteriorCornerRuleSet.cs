using EasyToolKit.Inspector;
using System;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [MetroFoldoutGroup("外转角规则集", IconTextureGetter = "-t:EasyToolKit.Tilemap.Editor.TilemapEditorIcons -p:Instance.TerrainExteriorCornerTypeIcon")]
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

        [Space(3)]
        [TerrainTileRuleType(TerrainType.TopLeftExteriorCorner)]
        public TerrainTileRule TopLeftRule;
        
        [ShowIf(nameof(UseFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainType.TopRightExteriorCorner)]
        public TerrainTileRule TopRightRule;
        
        [ShowIf(nameof(UseFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainType.BottomLeftExteriorCorner)]
        public TerrainTileRule BottomLeftRule;
        
        [ShowIf(nameof(UseFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainType.BottomRightExteriorCorner)]
        public TerrainTileRule BottomRightRule;

        public override TerrainTileRuleSetType RuleSetType => TerrainTileRuleSetType.ExteriorCorner;
    }
}
