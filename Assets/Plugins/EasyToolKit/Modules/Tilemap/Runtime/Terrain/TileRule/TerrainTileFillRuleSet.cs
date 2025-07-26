using System;
using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [MetroFoldoutGroup("填充规则集", IconTextureGetter = "-t:EasyToolKit.Tilemap.Editor.TilemapEditorIcons -p:Instance.TerrainFillTypeIcon")]
    [HideLabel]
    [Serializable]
    public class TerrainTileFillRuleSet : TerrainTileRuleSetBase
    {
        [TerrainTileRuleType(TerrainType.Fill)]
        [SerializeField] private TerrainTileRule _fillRule;

        public override TerrainTileRuleSetType RuleSetType => TerrainTileRuleSetType.Fill;

        public TerrainTileRule FillRule => _fillRule;
    }
}
