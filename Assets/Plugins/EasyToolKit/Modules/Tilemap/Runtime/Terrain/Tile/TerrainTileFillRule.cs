using System;
using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [MetroFoldoutGroup("填充规则集", IconTextureGetter = "-t:EasyToolKit.Tilemap.Editor.TilemapEditorIcons -p:Instance.TerrainFillTypeIcon")]
    [HideLabel]
    [Serializable]
    public class TerrainTileFillRule : TerrainTileRuleBase
    {
        [TerrainType(TerrainType.Fill)]
        [SerializeField] private TileObject _fillObject;

        public override TerrainTileRuleType RuleType => TerrainTileRuleType.Fill;

        public TileObject FillObject => _fillObject;
    }
}
