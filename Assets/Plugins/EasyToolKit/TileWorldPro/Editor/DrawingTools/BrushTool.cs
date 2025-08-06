using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    public class BrushTool : DraggableDrawingTool
    {
        private static readonly float Epsilon = TilemapUtility.Epsilon;

        protected override Vector3 AdjustTileWorldPosition(
            TileWorldDesigner target,
            Vector3 tileWorldPosition,
            Vector3 hitPoint,
            List<TilePosition> dragTilePositionPath)
        {
            var tileSize = target.TileWorldAsset.TileSize;
            var tilePosition = target.StartPoint.WorldPositionToTilePosition(tileWorldPosition, tileSize);

            var targetTerrainDefinition = target.TileWorldAsset.TryGetTerrainGuidAt(tilePosition);
            if (targetTerrainDefinition == null)
            {
                return tileWorldPosition;
            }

            var front = new Rect(
                tileWorldPosition.x, tileWorldPosition.y,
                tileSize, tileSize);

            if (hitPoint.z.IsApproximatelyOf(tileWorldPosition.z, Epsilon) &&
                front.Contains(new Vector2(hitPoint.x, hitPoint.y)))
            {
                tileWorldPosition.z -= tileSize;
                return tileWorldPosition;
            }

            if (hitPoint.z.IsApproximatelyOf(tileWorldPosition.z + 1, Epsilon) &&
                front.Contains(new Vector2(hitPoint.x, hitPoint.y)))
            {
                tileWorldPosition.z += tileSize;
                return tileWorldPosition;
            }

            var side = new Rect(
                tileWorldPosition.z, tileWorldPosition.y,
                tileSize, tileSize);

            if (hitPoint.x.IsApproximatelyOf(tileWorldPosition.x, Epsilon) &&
                side.Contains(new Vector2(hitPoint.z, hitPoint.y)))
            {
                tileWorldPosition.x -= tileSize;
                return tileWorldPosition;
            }

            if (hitPoint.x.IsApproximatelyOf(tileWorldPosition.x + 1, Epsilon) &&
                side.Contains(new Vector2(hitPoint.z, hitPoint.y)))
            {
                tileWorldPosition.x += tileSize;
                return tileWorldPosition;
            }

            var top = new Rect(
                tileWorldPosition.x, tileWorldPosition.z,
                tileSize, tileSize);

            if (hitPoint.y.IsApproximatelyOf(tileWorldPosition.y, Epsilon) &&
                top.Contains(new Vector2(hitPoint.x, hitPoint.z)))
            {
                tileWorldPosition.y -= tileSize;
                return tileWorldPosition;
            }

            if (hitPoint.y.IsApproximatelyOf(tileWorldPosition.y + 1, Epsilon) &&
                top.Contains(new Vector2(hitPoint.x, hitPoint.z)))
            {
                tileWorldPosition.y += tileSize;
                return tileWorldPosition;
            }

            return tileWorldPosition;
        }

        protected override IEnumerable<TilePosition> GetDrawingTilePositions(
            TileWorldDesigner target,
            TilePosition hitTilePosition,
            List<TilePosition> dragTilePositionPath)
        {
            return dragTilePositionPath;
        }

        protected override void DoTiles(TileWorldDesigner target, IEnumerable<TilePosition> tilePositions)
        {
            target.TileWorldAsset.SetTilesAt(tilePositions, TerrainDefinitionDrawer.SelectedGuid.Value);

            // if (target.Settings.RealTimeIncrementalBuild)
            // {
            //     target.IncrementalBuildAt(TerrainDefinitionDrawer.SelectedGuid.Value, tilePosition);
            // }
        }

        protected override Color GetHitColor(TileWorldDesigner target)
        {
            var selectedTerrainDefinition = target.TileWorldAsset.TerrainDefinitionSet.TryGetByGuid(
                TerrainDefinitionDrawer.SelectedGuid.Value);

            return selectedTerrainDefinition.DebugCubeColor;
        }
    }
}