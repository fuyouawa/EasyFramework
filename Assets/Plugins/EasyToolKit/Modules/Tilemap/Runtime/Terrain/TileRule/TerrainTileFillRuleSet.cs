using System;
using EasyToolKit.Inspector;

namespace EasyToolKit.Tilemap
{
    [AwesomeFoldoutGroup("填充规则集", IconTextureGetter = "-t:EasyToolKit.Tilemap.Editor.TilemapEditorIcons -p:Instance.TerrainFillTypeIcon")]
    [HideLabel]
    [Serializable]
    public class TerrainTileFillRuleSet : TerrainTileRuleSetBase
    {
        [TerrainTileRuleType(TerrainType.Fill)]
        public TerrainTileRule FillRule;

        public override TerrainTileRuleSetType RuleSetType => TerrainTileRuleSetType.Fill;
    }
}
