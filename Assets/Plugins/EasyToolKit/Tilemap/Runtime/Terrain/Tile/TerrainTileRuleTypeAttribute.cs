using System;

namespace EasyToolKit.Tilemap
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TerrainTileRuleTypeAttribute : Attribute
    {
        public TerrainTileRuleType RuleType;

        public TerrainTileRuleTypeAttribute(TerrainTileRuleType ruleType)
        {
            RuleType = ruleType;
        }
    }
}
