using System;

namespace EasyToolKit.TileWorldPro
{
    public class RemoveTilesEvent
    {
        public Guid TerrainGuid;
        public TilePosition[] TilePositions;

        public RemoveTilesEvent(Guid terrainGuid, TilePosition[] tilePositions)
        {
            TerrainGuid = terrainGuid;
            TilePositions = tilePositions;
        }
    }
}