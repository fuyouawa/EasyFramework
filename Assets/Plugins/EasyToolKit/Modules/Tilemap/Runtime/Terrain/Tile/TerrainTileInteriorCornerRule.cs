using EasyToolKit.Inspector;
using System;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [MetroFoldoutGroup("内转角规则集", IconTextureGetter = "-t:EasyToolKit.Tilemap.Editor.TilemapEditorIcons -p:Instance.TerrainInteriorCornerTypeIcon")]
    [HideLabel]
    [Serializable]
    public class TerrainTileInteriorCornerRule : TerrainTileRuleBase
    {
        [LabelText("使用完整定义")]
        [SerializeField] private bool _useFullDefinition;

        [Space(3)]
        [TerrainType(TerrainType.TopLeftInteriorCorner)]
        [SerializeField] private TileObject _topLeftObject;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainType(TerrainType.TopRightInteriorCorner)]
        [SerializeField] private TileObject _topRightObject;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainType(TerrainType.BottomLeftInteriorCorner)]
        [SerializeField] private TileObject _bottomLeftObject;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainType(TerrainType.BottomRightInteriorCorner)]
        [SerializeField] private TileObject _bottomRightObject;

        public override TerrainTileRuleType RuleType => TerrainTileRuleType.InteriorCorner;

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
