using EasyToolKit.Inspector;
using System;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [MetroFoldoutGroup("内转角规则集", IconTextureGetter = "-t:EasyToolKit.Tilemap.Editor.TilemapEditorIcons -p:Instance.TerrainInteriorCornerTypeIcon")]
    [HideLabel]
    [Serializable]
    public class TerrainTileInteriorCornerRuleSet : TerrainTileRuleSetBase
    {
        /// <summary>
        /// 使用完整定义。
        /// 如果为 false，则使用<see cref="TopLeftRule"/>作为基准规则，其他方向的规则将基于此规则进行调整。
        /// </summary>
        [LabelText("使用完整定义")]
        public bool UseFullDefinition;

        [Space(3)]
        [TerrainTileRuleType(TerrainType.TopLeftInteriorCorner)]
        public TerrainTileRule TopLeftRule;
        
        [ShowIf(nameof(UseFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainType.TopRightInteriorCorner)]
        public TerrainTileRule TopRightRule;
        
        [ShowIf(nameof(UseFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainType.BottomLeftInteriorCorner)]
        public TerrainTileRule BottomLeftRule;
        
        [ShowIf(nameof(UseFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainType.BottomRightInteriorCorner)]
        public TerrainTileRule BottomRightRule;

        public override TerrainTileRuleSetType RuleSetType => TerrainTileRuleSetType.InteriorCorner;
    }
}
