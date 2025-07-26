using System;
using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [MetroFoldoutGroup("边缘规则集", IconTextureGetter = "-t:EasyToolKit.Tilemap.Editor.TilemapEditorIcons -p:Instance.TerrainEdgeTypeIcon")]
    [HideLabel]
    [Serializable]
    public class TerrainTileEdgeRuleSet : TerrainTileRuleSetBase
    {
        /// <summary>
        /// 使用完整定义。
        /// 如果为 false，则使用<see cref="TopRule"/>作为基准规则，其他方向的规则将基于此规则进行调整。
        /// </summary>
        [LabelText("使用完整定义")]
        public bool UseFullDefinition;

        [Space(3)]
        [TerrainTileRuleType(TerrainType.TopEdge)]
        public TerrainTileRule TopRule;
        
        [ShowIf(nameof(UseFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainType.BottomEdge)]
        public TerrainTileRule BottomRule;
        
        [ShowIf(nameof(UseFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainType.LeftEdge)]
        public TerrainTileRule LeftRule;
        
        [ShowIf(nameof(UseFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainType.RightEdge)]
        public TerrainTileRule RightRule;

        public override TerrainTileRuleSetType RuleSetType => TerrainTileRuleSetType.Edge;
    }
}
