using System;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Tilemap.Editor
{
    internal class TilemapCreatorDrawer
    {
        private static readonly float Epsilon = TilemapCreator.Epsilon;

        private TilemapCreator _creator;
        private TerrainTileDefinition _selectedTerrainTileDefinition;

        public TilemapCreatorDrawer(TilemapCreator creator)
        {
            _creator = creator;
        }

        public TerrainTileDefinition SelectedTerrainTileDefinition
        {
            get => _selectedTerrainTileDefinition;
            set => _selectedTerrainTileDefinition = value;
        }

        public void DrawBase()
        {
            var baseRange = _creator.Asset.Settings.BaseRange;
            var tileSize = _creator.Asset.Settings.TileSize;

            EasyHandleHelper.PushColor(_creator.Asset.Settings.BaseColor);
            for (int x = 0; x <= baseRange.x; x++)
            {
                var start = _creator.transform.position + Vector3.right * (x * tileSize);
                var end = start + Vector3.forward * (tileSize * baseRange.y);
                Handles.DrawLine(start, end);
            }

            for (int y = 0; y <= baseRange.y; y++)
            {
                var start = _creator.transform.position + Vector3.forward * (y * tileSize);
                var end = start + Vector3.right * (tileSize * baseRange.x);
                Handles.DrawLine(start, end);
            }

            EasyHandleHelper.PopColor();
        }

        public bool DrawHit(out Vector3 hitPoint, out Vector3? hittedBlockPosition)
        {
            var tileSize = _creator.Asset.Settings.TileSize;
            bool handledHit = false;
            hitPoint = Vector3.zero;
            hittedBlockPosition = null;

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            float previousRaycastDistance = float.MaxValue;
            foreach (var block in _creator.Asset.TerrainTileMap)
            {
                var blockPosition = _creator.TilePositionToWorldPosition(block.TilePosition);

                if (block.Definition.DrawDebugCube)
                {
                    DrawCube(blockPosition, block.Definition.DebugCubeColor);
                }

                if (_selectedTerrainTileDefinition != null)
                {
                    var center = blockPosition + Vector3.one * (tileSize * 0.5f);
                    var bounds = new Bounds(center, tileSize * Vector3.one);

                    if (bounds.IntersectRay(ray, out var distance))
                    {
                        DrawDebugBlockGUI(blockPosition, distance);
                        if (distance < previousRaycastDistance)
                        {
                            hitPoint = ray.GetPoint(distance);
                            hittedBlockPosition = blockPosition;
                            handledHit = true;
                            previousRaycastDistance = distance;
                        }
                    }
                }
            }

            if (_selectedTerrainTileDefinition != null && !handledHit)
            {
                var plane = new Plane(Vector3.up, _creator.transform.position);
                if (plane.Raycast(ray, out float enter))
                {
                    hitPoint = ray.GetPoint(enter);
                    if (hitPoint.y < 0)
                    {
                        hitPoint = hitPoint.SetY(0);
                    }

                    DrawDebugBlockGUI(_creator.WorldPositionToBlockPosition(hitPoint), enter);
                    handledHit = true;
                }
            }

            return handledHit;
        }

        public void FixHitBlockPositionWithExclude(ref Vector3 blockPosition, Vector3 hitPoint)
        {
            var tileSize = _creator.Asset.Settings.TileSize;

            var front = new Rect(
                blockPosition.x, blockPosition.y,
                tileSize, tileSize);

            if (hitPoint.z.IsApproximatelyOf(blockPosition.z, Epsilon) &&
                front.Contains(new Vector2(hitPoint.x, hitPoint.y)))
            {
                blockPosition.z -= tileSize;
                return;
            }

            if (hitPoint.z.IsApproximatelyOf(blockPosition.z + 1, Epsilon) &&
                front.Contains(new Vector2(hitPoint.x, hitPoint.y)))
            {
                blockPosition.z += tileSize;
                return;
            }

            var side = new Rect(
                blockPosition.z, blockPosition.y,
                tileSize, tileSize);

            if (hitPoint.x.IsApproximatelyOf(blockPosition.x, Epsilon) &&
                side.Contains(new Vector2(hitPoint.z, hitPoint.y)))
            {
                blockPosition.x -= tileSize;
                return;
            }

            if (hitPoint.x.IsApproximatelyOf(blockPosition.x + 1, Epsilon) &&
                side.Contains(new Vector2(hitPoint.z, hitPoint.y)))
            {
                blockPosition.x += tileSize;
                return;
            }

            var top = new Rect(
                blockPosition.x, blockPosition.z,
                tileSize, tileSize);

            if (hitPoint.y.IsApproximatelyOf(blockPosition.y, Epsilon) &&
                top.Contains(new Vector2(hitPoint.x, hitPoint.z)))
            {
                blockPosition.y -= tileSize;
                return;
            }

            if (hitPoint.y.IsApproximatelyOf(blockPosition.y + 1, Epsilon) &&
                top.Contains(new Vector2(hitPoint.x, hitPoint.z)))
            {
                blockPosition.y += tileSize;
                return;
            }
        }

        public void DrawDebugHitPointGUI(Vector3 hitPoint)
        {
            if (!_creator.Asset.Settings.DrawDebugData)
                return;

            Handles.BeginGUI();

            Vector2 guiPosition = HandleUtility.WorldToGUIPoint(hitPoint);
            GUI.Label(new Rect(guiPosition.x + 10, guiPosition.y - 10, 200, 20), $"{hitPoint}");

            Handles.EndGUI();
        }

        public void DrawDebugRuleTypeGUI(Vector3 tileWorldPosition, TerrainRuleType ruleType)
        {
            var ruleTypeText = ruleType switch
            {
                TerrainRuleType.Fill => "填充",
                TerrainRuleType.TopEdge => "顶边缘",
                TerrainRuleType.LeftEdge => "左边缘",
                TerrainRuleType.BottomEdge => "底边缘",
                TerrainRuleType.RightEdge => "右边缘",
                TerrainRuleType.TopLeftExteriorCorner => "左上外角",
                TerrainRuleType.TopRightExteriorCorner => "右上外角",
                TerrainRuleType.BottomRightExteriorCorner => "右下外角",
                TerrainRuleType.BottomLeftExteriorCorner => "左下外角",
                TerrainRuleType.TopLeftInteriorCorner => "左上内角",
                TerrainRuleType.TopRightInteriorCorner => "右上内角",
                TerrainRuleType.BottomRightInteriorCorner => "右下内角",
                TerrainRuleType.BottomLeftInteriorCorner => "左下内角",
                _ => throw new NotImplementedException(),
            };

            Handles.BeginGUI();

            Vector2 guiPosition = HandleUtility.WorldToGUIPoint(tileWorldPosition);
            GUI.Label(new Rect(guiPosition.x, guiPosition.y - 20, 200, 20), $"{ruleTypeText}");

            Handles.EndGUI();
        }

        public void DrawDebugBlockGUI(Vector3 gridWorldPosition, float distance)
        {
            if (!_creator.Asset.Settings.DrawDebugData)
                return;

            Handles.BeginGUI();

            Vector2 guiPosition = HandleUtility.WorldToGUIPoint(gridWorldPosition);
            GUI.Label(new Rect(guiPosition.x, guiPosition.y - 20, 200, 20), $"{gridWorldPosition} - {distance:F2}");

            Handles.EndGUI();
        }

        public void DrawCube(Vector3 blockPosition)
        {
            DrawCube(blockPosition, _selectedTerrainTileDefinition.DebugCubeColor);
        }

        public void DrawCube(Vector3 blockPosition, Color color)
        {
            var tileSize = _creator.Asset.Settings.TileSize;
            EasyHandleHelper.PushColor(color.SetA(1f));

            var center = blockPosition + Vector3.one * (tileSize * 0.5f);
            Handles.DrawWireCube(center, Vector3.one * tileSize);

            if (!color.a.IsApproximatelyOf(0f, Epsilon))
            {
                Handles.color = color;
                Handles.CubeHandleCap(0, center, Quaternion.identity, tileSize, EventType.Repaint);
            }

            EasyHandleHelper.PopColor();
        }

        public bool IsInRange(Vector3 hitPoint)
        {
            if (hitPoint.y < _creator.transform.position.y)
            {
                return false;
            }

            var tileSize = _creator.Asset.Settings.TileSize;
            var baseRange = _creator.Asset.Settings.BaseRange;

            var local = hitPoint - _creator.transform.position;
            var gridX = (local.x / tileSize).SafeFloorToInt(Epsilon);
            var grisZ = (local.z / tileSize).SafeFloorToInt(Epsilon);

            if (gridX < 0 || gridX >= baseRange.x || grisZ < 0 || grisZ >= baseRange.y)
                return false;

            return true;
        }
    }
}