using System;

namespace EasyToolKit.Tilemap
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TerrainTypeAttribute : Attribute
    {
        public TerrainType TerrainType;

        public TerrainTypeAttribute(TerrainType terrainType)
        {
            TerrainType = terrainType;
        }
    }
}
