using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.Tilemap.Editor
{
    public class RectangeBrushTool : BrushTool
    {
        protected override Vector3 AdjustBlockPosition(TilemapCreator target, Vector3 blockPosition, Vector3 hitPoint, List<Vector3Int> dragTilePositionPath)
        {
            var newBlockPosition = base.AdjustBlockPosition(target, blockPosition, hitPoint, dragTilePositionPath);
            if (dragTilePositionPath.Count < 2)
            {
                return newBlockPosition;
            }

            var startTilePosition = dragTilePositionPath.First();
            return newBlockPosition.SetY(startTilePosition.y);
        }

        protected override IEnumerable<Vector3Int> GetDrawingTilePositions(
            TilemapCreator target,
            Vector3Int hitTilePosition,
            List<Vector3Int> dragTilePositionPath)
        {
            if (dragTilePositionPath.Count < 2)
            {
                return dragTilePositionPath;
            }

            var startTilePosition = dragTilePositionPath.First();
            var endTilePosition = hitTilePosition;

            var tilePositions = new List<Vector3Int>();

            var startX = Mathf.Min(startTilePosition.x, endTilePosition.x);
            var endX = Mathf.Max(startTilePosition.x, endTilePosition.x);
            var startZ = Mathf.Min(startTilePosition.z, endTilePosition.z);
            var endZ = Mathf.Max(startTilePosition.z, endTilePosition.z);

            for (int x = startX; x <= endX; x++)
            {
                for (int z = startZ; z <= endZ; z++)
                {
                    tilePositions.Add(new Vector3Int(x, startTilePosition.y, z));
                }
            }
            return tilePositions;
        }
    }
}