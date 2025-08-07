using System;

namespace EasyToolKit.TileWorldPro
{
    public class SetTilesEvent
    {
        public Guid TerrainGuid;
        public TilePosition[] TilePositions;

        public SetTilesEvent(Guid terrainGuid, TilePosition[] tilePositions)
        {
            TerrainGuid = terrainGuid;
            TilePositions = tilePositions;
        }
    }
}