using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Tilemap.Editor
{
    public abstract class DraggableDrawingTool : IDrawingTool
    {
        private List<Vector3Int> _dragTilePositionPath = new List<Vector3Int>();
        private bool _isDragging = false;

        public void OnSceneGUI(TilemapCreator target, Vector3 hitPoint, Vector3? hitBlockPosition)
        {
            var tileSize = target.Asset.Settings.TileSize;

            var blockPosition = hitBlockPosition ?? TilemapUtility.WorldPositionToBlockPosition(
                target.transform.position, hitPoint, tileSize);

            var tilePosition = TilemapUtility.WorldPositionToTilePosition(
                target.transform.position, blockPosition, tileSize);

            var newBlockPosition = AdjustBlockPosition(target, blockPosition, hitPoint, _dragTilePositionPath);
            if (newBlockPosition != blockPosition)
            {
                blockPosition = newBlockPosition;
                if (!IsInRange(target.transform.position, target.Asset.Settings.BaseRange, tileSize, blockPosition))
                {
                    return;
                }

                tilePosition = TilemapUtility.WorldPositionToTilePosition(
                    target.transform.position,
                    blockPosition,
                    tileSize);
            }

            if (!FilterHitTile(target, tilePosition))
            {
                return;
            }

            // Handle mouse interactions
            if (IsMouseDown())
            {
                // Start a new drag operation
                _dragTilePositionPath.Clear();
                _dragTilePositionPath.Add(tilePosition);
                _isDragging = true;
                FinishMouse();
            }
            else if (IsMouseDrag() && _isDragging)
            {
                // Continue the drag operation
                if (!_dragTilePositionPath.Contains(tilePosition))
                {
                    _dragTilePositionPath.Add(tilePosition);
                }
                FinishMouse();
            }
            else if (IsMouseUp() && _isDragging)
            {
                // End the drag operation - now apply all changes at once
                if (_dragTilePositionPath.Count > 0)
                {
                    // Record a single undo operation for all changes
                    Undo.RecordObject(target.Asset, $"Draw tiles in {target.Asset.name}");

                    foreach (var dragTilePosition in GetDrawingTilePositions(target, tilePosition, _dragTilePositionPath))
                    {
                        DoTile(target, dragTilePosition);
                    }

                    // Mark the asset as dirty once for all changes
                    EasyEditorUtility.SetUnityObjectDirty(target.Asset);
                }

                _isDragging = false;
                _dragTilePositionPath.Clear();
                FinishMouse();
            }

            var hitColor = GetHitColor(target);

            var surroundingColor = Color.white.SetA(0.2f);
            TilemapHandles.DrawHitCube(blockPosition, tileSize, hitColor, surroundingColor);

            // Draw all tiles in the drag operation
            foreach (var dragTilePosition in GetDrawingTilePositions(target, tilePosition, _dragTilePositionPath))
            {
                var dragWorldPosition = TilemapUtility.TilePositionToWorldPosition(
                    target.transform.position, dragTilePosition, tileSize);
                TilemapHandles.DrawHitCube(dragWorldPosition, tileSize, hitColor.MulA(0.7f));
            }
        }

        protected virtual Vector3 AdjustBlockPosition(TilemapCreator target, Vector3 blockPosition, Vector3 hitPoint, List<Vector3Int> dragTilePositionPath)
        {
            return blockPosition;
        }

        protected virtual IEnumerable<Vector3Int> GetDrawingTilePositions(
            TilemapCreator target,
            Vector3Int hitTilePosition,
            List<Vector3Int> dragTilePositionPath)
        {
            return dragTilePositionPath;
        }

        protected abstract Color GetHitColor(TilemapCreator target);

        protected abstract void DoTile(TilemapCreator target, Vector3Int tilePosition);

        protected virtual bool FilterHitTile(TilemapCreator target, Vector3Int tilePosition)
        {
            return true;
        }

        private static bool IsMouseDown()
        {
            var e = Event.current;
            return e.type == EventType.MouseDown && e.button == 0;
        }

        private static bool IsMouseDrag()
        {
            var e = Event.current;
            return e.type == EventType.MouseDrag && e.button == 0;
        }

        private static bool IsMouseUp()
        {
            var e = Event.current;
            return e.type == EventType.MouseUp && e.button == 0;
        }

        private static void FinishMouse()
        {
            Event.current.Use();
        }

        private static bool IsInRange(Vector3 relativePosition, Vector2 range, float tileSize, Vector3 position)
        {
            if (relativePosition.y < 0)
            {
                return false;
            }

            var tilePosition = TilemapUtility.WorldPositionToTilePosition(
                relativePosition,
                position,
                tileSize);

            if (tilePosition.x < 0 || tilePosition.x >= range.x || tilePosition.z < 0 || tilePosition.z >= range.y)
                return false;

            return true;
        }
    }
}