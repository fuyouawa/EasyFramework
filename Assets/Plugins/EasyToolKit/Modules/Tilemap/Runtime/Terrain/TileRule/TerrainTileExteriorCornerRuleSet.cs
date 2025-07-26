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
        [LabelText("使用完整定义")]
        [SerializeField] private bool _useFullDefinition;

        [Space(3)]
        [TerrainTileRuleType(TerrainType.TopLeftExteriorCorner)]
        [SerializeField] private TerrainTileRule _topLeftRule;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainType.TopRightExteriorCorner)]
        [SerializeField] private TerrainTileRule _topRightRule;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainType.BottomLeftExteriorCorner)]
        [SerializeField] private TerrainTileRule _bottomLeftRule;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainType.BottomRightExteriorCorner)]
        [SerializeField] private TerrainTileRule _bottomRightRule;

        public override TerrainTileRuleSetType RuleSetType => TerrainTileRuleSetType.ExteriorCorner;

        /// <summary>
        /// 使用完整定义。
        /// 如果为 false，则使用<see cref="TopLeftRule"/>作为基准规则，其他方向的规则将基于此规则进行调整。
        /// </summary>
        public bool UseFullDefinition => _useFullDefinition;

        public TerrainTileRule TopLeftRule => _topLeftRule;
        public TerrainTileRule TopRightRule => _useFullDefinition ? _topRightRule : _topLeftRule;
        public TerrainTileRule BottomLeftRule => _useFullDefinition ? _bottomLeftRule : _topLeftRule;
        public TerrainTileRule BottomRightRule => _useFullDefinition ? _bottomRightRule : _topLeftRule;
    }
}
