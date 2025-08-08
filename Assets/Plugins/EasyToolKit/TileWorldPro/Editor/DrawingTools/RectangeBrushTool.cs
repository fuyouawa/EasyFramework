using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    public class RectangeBrushTool : BrushTool
    {
        protected override Vector3 AdjustTileWorldPosition(DrawingToolContext context, IReadOnlyList<TilePosition> dragTilePositionPath)
        {
            var newTileWorldPosition = base.AdjustTileWorldPosition(context, dragTilePositionPath);
            if (dragTilePositionPath.Count < 2)
            {
                return newTileWorldPosition;
            }

            var startTilePosition = dragTilePositionPath.First();
            var startTileWorldPosition = context.Target.StartPoint.TilePositionToWorldPosition(startTilePosition, context.Target.TileWorldAsset.TileSize);
            return newTileWorldPosition.SetY(startTileWorldPosition.y);
        }

        protected override IReadOnlyList<TilePosition> GetDrawingTilePositions(
            DrawingToolContext context,
            IReadOnlyList<TilePosition> dragTilePositionPath)
        {
            if (dragTilePositionPath.Count < 2)
            {
                return dragTilePositionPath;
            }

            var startTilePosition = dragTilePositionPath.First();
            var endTilePosition = context.Target.StartPoint.WorldPositionToTilePosition(context.HitTileWorldPosition, context.Target.TileWorldAsset.TileSize);

            var tilePositions = new List<TilePosition>();

            var startX = Mathf.Min(startTilePosition.X, endTilePosition.X);
            var endX = Mathf.Max(startTilePosition.X, endTilePosition.X);
            var startZ = Mathf.Min(startTilePosition.Z, endTilePosition.Z);
            var endZ = Mathf.Max(startTilePosition.Z, endTilePosition.Z);

            for (int x = startX; x <= endX; x++)
            {
                for (int z = startZ; z <= endZ; z++)
                {
                    tilePositions.Add(new TilePosition(x, startTilePosition.Y, z));
                }
            }
            return tilePositions;
        }
    }
}