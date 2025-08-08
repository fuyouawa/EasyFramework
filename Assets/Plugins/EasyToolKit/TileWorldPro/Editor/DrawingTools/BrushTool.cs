using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    public class BrushTool : DraggableDrawingTool, IEasyEventTrigger
    {
        private static readonly float Epsilon = 0.0001f;

        protected override Vector3 AdjustTileWorldPosition(
            DrawingToolContext context,
            IReadOnlyList<TilePosition> dragTilePositionPath)
        {
            var tileSize = context.Target.TileWorldAsset.TileSize;
            var tilePosition = context.Target.StartPoint.WorldPositionToTilePosition(context.HitTileWorldPosition, tileSize);

            var targetTerrainDefinition = context.Target.TileWorldAsset.TryGetTerrainGuidAt(tilePosition);
            if (targetTerrainDefinition == null)
            {
                return context.HitTileWorldPosition;
            }

            var front = new Rect(
                context.HitTileWorldPosition.x, context.HitTileWorldPosition.y,
                tileSize, tileSize);

            if (context.HitPoint.z.IsApproximatelyOf(context.HitTileWorldPosition.z, Epsilon) &&
                front.Contains(new Vector2(context.HitPoint.x, context.HitPoint.y)))
            {
                context.HitTileWorldPosition.z -= tileSize;
                return context.HitTileWorldPosition;
            }

            if (context.HitPoint.z.IsApproximatelyOf(context.HitTileWorldPosition.z + 1, Epsilon) &&
                front.Contains(new Vector2(context.HitPoint.x, context.HitPoint.y)))
            {
                context.HitTileWorldPosition.z += tileSize;
                return context.HitTileWorldPosition;
            }

            var side = new Rect(
                context.HitTileWorldPosition.z, context.HitTileWorldPosition.y,
                tileSize, tileSize);

            if (context.HitPoint.x.IsApproximatelyOf(context.HitTileWorldPosition.x, Epsilon) &&
                side.Contains(new Vector2(context.HitPoint.z, context.HitPoint.y)))
            {
                context.HitTileWorldPosition.x -= tileSize;
                return context.HitTileWorldPosition;
            }

            if (context.HitPoint.x.IsApproximatelyOf(context.HitTileWorldPosition.x + 1, Epsilon) &&
                side.Contains(new Vector2(context.HitPoint.z, context.HitPoint.y)))
            {
                context.HitTileWorldPosition.x += tileSize;
                return context.HitTileWorldPosition;
            }

            var top = new Rect(
                context.HitTileWorldPosition.x, context.HitTileWorldPosition.z,
                tileSize, tileSize);

            if (context.HitPoint.y.IsApproximatelyOf(context.HitTileWorldPosition.y, Epsilon) &&
                top.Contains(new Vector2(context.HitPoint.x, context.HitPoint.z)))
            {
                context.HitTileWorldPosition.y -= tileSize;
                return context.HitTileWorldPosition;
            }

            if (context.HitPoint.y.IsApproximatelyOf(context.HitTileWorldPosition.y + 1, Epsilon) &&
                top.Contains(new Vector2(context.HitPoint.x, context.HitPoint.z)))
            {
                context.HitTileWorldPosition.y += tileSize;
                return context.HitTileWorldPosition;
            }

            return context.HitTileWorldPosition;
        }

        protected override IReadOnlyList<TilePosition> GetDrawingTilePositions(
            DrawingToolContext context,
            IReadOnlyList<TilePosition> dragTilePositionPath)
        {
            return dragTilePositionPath;
        }

        protected override void DoTiles(DrawingToolContext context, IReadOnlyList<TilePosition> tilePositions)
        {
            context.Target.TileWorldAsset.SetTilesAt(tilePositions, context.TerrainDefinition.Guid);
            this.TriggerEvent(new SetTilesEvent(context.TerrainDefinition.Guid, tilePositions.ToArray()));
        }

        protected override Color GetHitColor(DrawingToolContext context)
        {
            return context.TerrainDefinition.DebugCubeColor;
        }
    }
}