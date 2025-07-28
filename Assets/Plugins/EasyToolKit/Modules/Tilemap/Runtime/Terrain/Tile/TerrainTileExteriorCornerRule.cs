using EasyToolKit.Inspector;
using System;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [MetroFoldoutGroup("外转角规则集", IconTextureGetter = "-t:EasyToolKit.Tilemap.Editor.TilemapEditorIcons -p:Instance.TerrainExteriorCornerTypeIcon")]
    [HideLabel]
    [Serializable]
    public class TerrainTileExteriorCornerRule : TerrainTileRuleBase
    {
        [LabelText("使用完整定义")]
        [SerializeField] private bool _useFullDefinition;

        [Space(3)]
        [TerrainType(TerrainType.TopLeftExteriorCorner)]
        [SerializeField] private TileObject _topLeftObject;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainType(TerrainType.TopRightExteriorCorner)]
        [SerializeField] private TileObject _topRightObject;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainType(TerrainType.BottomLeftExteriorCorner)]
        [SerializeField] private TileObject _bottomLeftObject;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainType(TerrainType.BottomRightExteriorCorner)]
        [SerializeField] private TileObject _bottomRightObject;

        public override TerrainTileRuleType RuleType => TerrainTileRuleType.ExteriorCorner;

        /// <summary>
        /// 使用完整定义。
        /// 如果为 false，则使用<see cref="TopLeftObject"/>作为基准规则，其他方向的规则将基于此规则进行调整。
        /// </summary>
        public bool UseFullDefinition => _useFullDefinition;

        public TileObject TopLeftObject => _topLeftObject;
        public TileObject TopRightObject => _topRightObject;
        public TileObject BottomLeftObject => _bottomLeftObject;
        public TileObject BottomRightObject => _bottomRightObject;
    }
}
