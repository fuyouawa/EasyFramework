using System;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    public struct TerrainTilePosition
    {
        public TilePosition TilePosition;
        public Guid TerrainGuid;

        public TerrainTilePosition(TilePosition tilePosition, Guid terrainGuid)
        {
            TilePosition = tilePosition;
            TerrainGuid = terrainGuid;
        }
    }
}
