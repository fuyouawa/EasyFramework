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
        [LabelText("使用完整定义")]
        [SerializeField] private bool _useFullDefinition;

        [Space(3)]
        [TerrainTileRuleType(TerrainType.TopLeftInteriorCorner)]
        [SerializeField] private TerrainTileRule _topLeftRule;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainType.TopRightInteriorCorner)]
        [SerializeField] private TerrainTileRule _topRightRule;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainType.BottomLeftInteriorCorner)]
        [SerializeField] private TerrainTileRule _bottomLeftRule;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainTileRuleType(TerrainType.BottomRightInteriorCorner)]
        [SerializeField] private TerrainTileRule _bottomRightRule;

        public override TerrainTileRuleSetType RuleSetType => TerrainTileRuleSetType.InteriorCorner;

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
