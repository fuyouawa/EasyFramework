using System;
using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [MetroFoldoutGroup("边缘规则集", IconTextureGetter = "-t:EasyToolKit.Tilemap.Editor.TilemapEditorIcons -p:Instance.TerrainEdgeTypeIcon")]
    [HideLabel]
    [Serializable]
    public class TerrainTileEdgeRule : TerrainTileRuleBase
    {
        [LabelText("使用完整定义")]
        [SerializeField] private bool _useFullDefinition;

        [Space(3)]
        [TerrainType(TerrainType.TopEdge)]
        [SerializeField] private TileObject _topObject;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainType(TerrainType.BottomEdge)]
        [SerializeField] private TileObject _bottomObject;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainType(TerrainType.LeftEdge)]
        [SerializeField] private TileObject _leftObject;

        [ShowIf(nameof(_useFullDefinition))]
        [Space(3)]
        [TerrainType(TerrainType.RightEdge)]
        [SerializeField] private TileObject _rightObject;

        public override TerrainTileRuleType RuleType => TerrainTileRuleType.Edge;

        /// <summary>
        /// 使用完整定义。
        /// 如果为 false，则使用<see cref="TopObject"/>作为基准规则，其他方向的规则将基于此规则进行调整。
        /// </summary>
        public bool UseFullDefinition => _useFullDefinition;

        public TileObject TopObject => _topObject;
        public TileObject BottomObject => _bottomObject;
        public TileObject LeftObject => _leftObject;
        public TileObject RightObject => _rightObject;
    }
}
