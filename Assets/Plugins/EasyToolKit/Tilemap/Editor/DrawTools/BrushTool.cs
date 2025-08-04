using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Tilemap.Editor
{
    public class BrushTool : DraggableDrawTool
    {
        private static readonly float Epsilon = TilemapUtility.Epsilon;

        protected override Vector3 AdjustBlockPosition(Vector3 blockPosition, float tileSize, Vector3 hitPoint)
        {
            var front = new Rect(
                blockPosition.x, blockPosition.y,
                tileSize, tileSize);

            if (hitPoint.z.IsApproximatelyOf(blockPosition.z, Epsilon) &&
                front.Contains(new Vector2(hitPoint.x, hitPoint.y)))
            {
                blockPosition.z -= tileSize;
                return blockPosition;
            }

            if (hitPoint.z.IsApproximatelyOf(blockPosition.z + 1, Epsilon) &&
                front.Contains(new Vector2(hitPoint.x, hitPoint.y)))
            {
                blockPosition.z += tileSize;
                return blockPosition;
            }

            var side = new Rect(
                blockPosition.z, blockPosition.y,
                tileSize, tileSize);

            if (hitPoint.x.IsApproximatelyOf(blockPosition.x, Epsilon) &&
                side.Contains(new Vector2(hitPoint.z, hitPoint.y)))
            {
                blockPosition.x -= tileSize;
                return blockPosition;
            }

            if (hitPoint.x.IsApproximatelyOf(blockPosition.x + 1, Epsilon) &&
                side.Contains(new Vector2(hitPoint.z, hitPoint.y)))
            {
                blockPosition.x += tileSize;
                return blockPosition;
            }

            var top = new Rect(
                blockPosition.x, blockPosition.z,
                tileSize, tileSize);

            if (hitPoint.y.IsApproximatelyOf(blockPosition.y, Epsilon) &&
                top.Contains(new Vector2(hitPoint.x, hitPoint.z)))
            {
                blockPosition.y -= tileSize;
                return blockPosition;
            }

            if (hitPoint.y.IsApproximatelyOf(blockPosition.y + 1, Epsilon) &&
                top.Contains(new Vector2(hitPoint.x, hitPoint.z)))
            {
                blockPosition.y += tileSize;
                return blockPosition;
            }

            return blockPosition;
        }

        protected override IEnumerable<Vector3Int> GetDrawingTiles(TilemapCreator target, List<Vector3Int> dragTilePath)
        {
            return dragTilePath;
        }

        protected override void DoTile(TilemapCreator target, Vector3Int tilePosition)
        {
            target.Asset.TerrainMap.SetTileAt(tilePosition, TerrainDefinitionDrawer.SelectedGuid.Value);

            if (target.Asset.Settings.RealTimeIncrementalBuild)
            {
                target.IncrementalBuildAt(TerrainDefinitionDrawer.SelectedGuid.Value, tilePosition);
            }
        }

        protected override Color GetHitColor(TilemapCreator target)
        {
            var selectedTerrainDefinition = target.Asset.TerrainMap.DefinitionSet.TryGetByGuid(
                TerrainDefinitionDrawer.SelectedGuid.Value);

            return selectedTerrainDefinition.DebugCubeColor;
        }
    }
}