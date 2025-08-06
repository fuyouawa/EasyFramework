using EasyToolKit.Inspector;
using System;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [MetroFoldoutGroup("填充规则集", IconTextureGetter = "-t:EasyToolKit.TileWorldPro.Editor.TileWorldIcons -p:Instance.TerrainFillTypeIcon")]
    [HideLabel]
    [Serializable]
    public class TerrainFillRule : TerrainRuleBase
    {
        [TerrainTileRuleType(TerrainTileRuleType.Fill)]
        [SerializeField] private TerrainTileDefinition _fillTileDefinition;

        public override TerrainRuleType RuleType => TerrainRuleType.Fill;

        public TerrainTileDefinition FillTileDefinition => _fillTileDefinition;

        public override GameObject GetTileInstanceByRuleType(TerrainTileRuleType ruleType)
        {
            if (ruleType == TerrainTileRuleType.Fill)
            {
                return FillTileDefinition.TryInstantiate();
            }

            throw new ArgumentOutOfRangeException(nameof(ruleType), ruleType, null);
        }
    }
}
