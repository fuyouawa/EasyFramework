using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    public abstract class DraggableDrawingTool : IDrawingTool
    {
        private List<TilePosition> _dragTilePositionPath = new List<TilePosition>();
        private bool _isDragging = false;

        public void OnSceneGUI(TileWorldDesigner target, Vector3 hitPoint, Vector3? hitTileWorldPosition)
        {
            var tileSize = target.TileWorldAsset.TileSize;

            var tileWorldPosition = hitTileWorldPosition ?? target.StartPoint.WorldPositionToTileWorldPosition(hitPoint, tileSize);
            if (!IsInRange(target, tileWorldPosition))
            {
                return;
            }

            var tilePosition = target.StartPoint.WorldPositionToTilePosition(tileWorldPosition, tileSize);

            var newTileWorldPosition = AdjustTileWorldPosition(target, tileWorldPosition, hitPoint, _dragTilePositionPath);
            if (newTileWorldPosition != tileWorldPosition)
            {
                tileWorldPosition = newTileWorldPosition;
                if (!IsInRange(target, tileWorldPosition))
                {
                    return;
                }

                tilePosition = target.StartPoint.WorldPositionToTilePosition(tileWorldPosition, tileSize);
            }

            if (!FilterHitTile(target, tilePosition))
            {
                return;
            }

            var drawingTilePositions = GetDrawingTilePositions(target, tilePosition, _dragTilePositionPath);

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
                    Undo.RecordObject(target.TileWorldAsset, $"Draw tiles in {target.TileWorldAsset.name}");

                    DoTiles(target, drawingTilePositions);

                    // Mark the asset as dirty once for all changes
                    EasyEditorUtility.SetUnityObjectDirty(target.TileWorldAsset);
                }

                _isDragging = false;
                _dragTilePositionPath.Clear();
                FinishMouse();
            }

            var hitColor = GetHitColor(target);

            var surroundingColor = Color.white.SetA(0.2f);
            TileWorldHandles.DrawHitCube(tileWorldPosition, tileSize, hitColor, surroundingColor);

            // Draw all tiles in the drag operation
            foreach (var dragTilePosition in drawingTilePositions)
            {
                var dragWorldPosition = target.StartPoint.TilePositionToWorldPosition(dragTilePosition, tileSize);
                TileWorldHandles.DrawHitCube(dragWorldPosition, tileSize, hitColor.MulA(0.7f));
            }
        }

        protected virtual Vector3 AdjustTileWorldPosition(
            TileWorldDesigner target,
            Vector3 tileWorldPosition,
            Vector3 hitPoint,
            List<TilePosition> dragTilePositionPath)
        {
            return tileWorldPosition;
        }

        protected virtual IEnumerable<TilePosition> GetDrawingTilePositions(
            TileWorldDesigner target,
            TilePosition hitTilePosition,
            List<TilePosition> dragTilePositionPath)
        {
            return dragTilePositionPath;
        }

        protected abstract Color GetHitColor(TileWorldDesigner target);

        protected abstract void DoTiles(TileWorldDesigner target, IEnumerable<TilePosition> tilePositions);

        protected virtual bool FilterHitTile(TileWorldDesigner target, TilePosition tilePosition)
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

        private static bool IsInRange(TileWorldDesigner target, Vector3 worldPosition)
        {
            var tileSize = target.TileWorldAsset.TileSize;
            var range = target.Settings.BaseRange;

            var tilePosition = target.StartPoint.WorldPositionToTilePosition(worldPosition, tileSize);

            if (tilePosition.X < 0 || tilePosition.X >= range.x ||
                tilePosition.Z < 0 || tilePosition.Z >= range.y ||
                tilePosition.Y < 0)
                return false;

            return true;
        }
    }
}