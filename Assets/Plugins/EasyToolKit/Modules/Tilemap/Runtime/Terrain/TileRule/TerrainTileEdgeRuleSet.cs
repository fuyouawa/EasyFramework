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
        [LabelText("使用完整定义")]
        [SerializeField] private bool _useFullDefinition;

        [Space(3)]
        [TerrainTileRuleType(TerrainType.TopEdge)]
        [SerializeField] private TerrainTileRule _topRule;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainType.BottomEdge)]
        [SerializeField] private TerrainTileRule _bottomRule;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainType.LeftEdge)]
        [SerializeField] private TerrainTileRule _leftRule;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainType.RightEdge)]
        [SerializeField] private TerrainTileRule _rightRule;

        public override TerrainTileRuleSetType RuleSetType => TerrainTileRuleSetType.Edge;

        /// <summary>
        /// 使用完整定义。
        /// 如果为 false，则使用<see cref="TopRule"/>作为基准规则，其他方向的规则将基于此规则进行调整。
        /// </summary>
        public bool UseFullDefinition => _useFullDefinition;

        public TerrainTileRule TopRule => _topRule;
        public TerrainTileRule BottomRule => _useFullDefinition ? _bottomRule : _topRule;
        public TerrainTileRule LeftRule => _useFullDefinition ? _leftRule : _topRule;
        public TerrainTileRule RightRule => _useFullDefinition ? _rightRule : _topRule;
    }
}
