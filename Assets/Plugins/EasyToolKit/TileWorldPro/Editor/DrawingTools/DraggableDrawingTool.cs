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

        public void OnSceneGUI(DrawingToolContext context)
        {
            var tileSize = context.Target.TileWorldAsset.TileSize;

            if (!IsInRange(context.Target, context.HitTileWorldPosition))
            {
                return;
            }

            var tilePosition = context.Target.StartPoint.WorldPositionToTilePosition(context.HitTileWorldPosition, tileSize);

            var newTileWorldPosition = AdjustTileWorldPosition(context, _dragTilePositionPath);
            if (newTileWorldPosition != context.HitTileWorldPosition)
            {
                context.HitTileWorldPosition = newTileWorldPosition;
                if (!IsInRange(context.Target, context.HitTileWorldPosition))
                {
                    return;
                }

                tilePosition = context.Target.StartPoint.WorldPositionToTilePosition(context.HitTileWorldPosition, tileSize);
            }

            if (!FilterHitTile(context, tilePosition))
            {
                return;
            }

            var drawingTilePositions = GetDrawingTilePositions(context, _dragTilePositionPath);

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
                    Undo.RecordObject(context.Target.TileWorldAsset, $"Draw tiles in {context.Target.TileWorldAsset.name}");

                    DoTiles(context, drawingTilePositions);

                    // Mark the asset as dirty once for all changes
                    EasyEditorUtility.SetUnityObjectDirty(context.Target.TileWorldAsset);
                }

                _isDragging = false;
                _dragTilePositionPath.Clear();
                FinishMouse();
            }

            var hitColor = GetHitColor(context);

            var surroundingColor = Color.white.SetA(0.2f);
            TileWorldHandles.DrawHitCube(context.HitTileWorldPosition, tileSize, hitColor, surroundingColor);

            // Draw all tiles in the drag operation
            foreach (var dragTilePosition in drawingTilePositions)
            {
                var dragWorldPosition = context.Target.StartPoint.TilePositionToWorldPosition(dragTilePosition, tileSize);
                TileWorldHandles.DrawHitCube(dragWorldPosition, tileSize, hitColor.MulA(0.7f));
            }
        }

        protected virtual Vector3 AdjustTileWorldPosition(
            DrawingToolContext context,
            IReadOnlyList<TilePosition> dragTilePositionPath)
        {
            return context.HitTileWorldPosition;
        }

        protected virtual IReadOnlyList<TilePosition> GetDrawingTilePositions(
            DrawingToolContext context,
            IReadOnlyList<TilePosition> dragTilePositionPath)
        {
            return dragTilePositionPath;
        }

        protected abstract Color GetHitColor(DrawingToolContext context);

        protected abstract void DoTiles(DrawingToolContext context, IReadOnlyList<TilePosition> tilePositions);

        protected virtual bool FilterHitTile(DrawingToolContext context, TilePosition tilePosition)
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