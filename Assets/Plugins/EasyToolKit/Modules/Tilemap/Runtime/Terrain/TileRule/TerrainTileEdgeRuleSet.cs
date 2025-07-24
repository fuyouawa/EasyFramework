using System;
using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [AwesomeFoldoutGroup("边缘规则集", IconTextureGetter = "-t:EasyToolKit.Tilemap.Editor.TilemapEditorIcons -p:Instance.TerrainTileEdgeRuleSetIcon")]
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

        [TerrainTileRuleType(TerrainType.TopEdge)]
        [Space(3)]
        public TerrainTileRule TopRule;

        [TerrainTileRuleType(TerrainType.BottomEdge)]
        [Space(3)]
        public TerrainTileRule BottomRule;

        [TerrainTileRuleType(TerrainType.LeftEdge)]
        [Space(3)]
        public TerrainTileRule LeftRule;

        [TerrainTileRuleType(TerrainType.RightEdge)]
        [Space(3)]
        public TerrainTileRule RightRule;

        public override TerrainTileRuleSetType RuleSetType => TerrainTileRuleSetType.Edge;
    }
}
