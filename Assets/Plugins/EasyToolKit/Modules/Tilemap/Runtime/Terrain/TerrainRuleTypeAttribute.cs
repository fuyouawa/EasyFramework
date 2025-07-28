using System;

namespace EasyToolKit.Tilemap
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TerrainRuleTypeAttribute : Attribute
    {
        public TerrainRuleType TerrainRuleType;

        public TerrainRuleTypeAttribute(TerrainRuleType terrainRuleType)
        {
            TerrainRuleType = terrainRuleType;
        }
    }
}
