using System;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Tilemap.Editor
{
    public class TilemapCreatorDrawer
    {
        private static readonly float Epsilon = TilemapCreator.Epsilon;

        private TilemapCreator _creator;
        private TerrainDefinition _selectedTerrainDefinition;

        public TilemapCreatorDrawer(TilemapCreator creator)
        {
            _creator = creator;
        }

        public TerrainDefinition SelectedTerrainDefinition
        {
            get => _selectedTerrainDefinition;
            set => _selectedTerrainDefinition = value;
        }

        public void DrawBaseRange()
        {
            var baseRange = _creator.Asset.Settings.BaseRange;
            var tileSize = _creator.Asset.Settings.TileSize;

            EasyHandleHelper.PushColor(_creator.Asset.Settings.BaseDebugColor.SetA(1f));

            var start = _creator.transform.position;
            var size = new Vector3(baseRange.x * tileSize, 0, baseRange.y * tileSize);
            var center = start + size * 0.5f;

            Handles.DrawWireCube(center, size);

            EasyHandleHelper.PopColor();
        }

        public void DrawBase()
        {
            var baseRange = _creator.Asset.Settings.BaseRange;
            var tileSize = _creator.Asset.Settings.TileSize;

            EasyHandleHelper.PushZTest(UnityEngine.Rendering.CompareFunction.LessEqual);
            EasyHandleHelper.PushColor(_creator.Asset.Settings.BaseDebugColor);
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
            EasyHandleHelper.PopZTest();
        }

        public bool DrawHit(out Vector3 hitPoint, out Vector3? hittedBlockPosition)
        {
            var tileSize = _creator.Asset.Settings.TileSize;
            bool handledHit = false;
            hitPoint = Vector3.zero;
            hittedBlockPosition = null;

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            float previousRaycastDistance = float.MaxValue;

            foreach (var block in _creator.Asset.TerrainMap)
            {
                var blockPosition = _creator.TilePositionToWorldPosition(block.TilePosition);

                if (block.Definition.DrawDebugCube)
                {
                    if (block.Definition.EnableZTestForDebugCube)
                    {
                        EasyHandleHelper.PushZTest(UnityEngine.Rendering.CompareFunction.LessEqual);
                    }

                    DrawCube(blockPosition, block.Definition.DebugCubeColor);

                    if (block.Definition.EnableZTestForDebugCube)
                    {
                        EasyHandleHelper.PopZTest();
                    }
                }

                if (_selectedTerrainDefinition != null)
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

            if (_selectedTerrainDefinition != null && !handledHit)
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

        public void DrawDebugRuleTypeGUI(Vector3 tileWorldPosition, TerrainTileRuleType ruleType)
        {
            var ruleTypeText = ruleType switch
            {
                TerrainTileRuleType.Fill => "填充",
                TerrainTileRuleType.TopEdge => "顶边缘",
                TerrainTileRuleType.LeftEdge => "左边缘",
                TerrainTileRuleType.BottomEdge => "底边缘",
                TerrainTileRuleType.RightEdge => "右边缘",
                TerrainTileRuleType.TopLeftExteriorCorner => "左上外角",
                TerrainTileRuleType.TopRightExteriorCorner => "右上外角",
                TerrainTileRuleType.BottomRightExteriorCorner => "右下外角",
                TerrainTileRuleType.BottomLeftExteriorCorner => "左下外角",
                TerrainTileRuleType.TopLeftInteriorCorner => "左上内角",
                TerrainTileRuleType.TopRightInteriorCorner => "右上内角",
                TerrainTileRuleType.BottomRightInteriorCorner => "右下内角",
                TerrainTileRuleType.BottomLeftInteriorCorner => "左下内角",
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

        public void DrawHitCube(Vector3 blockPosition, Color hitColor, Color? surroundingColor = null)
        {
            DrawCube(blockPosition, hitColor);
            DrawFillCube(blockPosition, hitColor.MulA(0.7f));

            if (surroundingColor != null)
            {
                DrawSquare(blockPosition + Vector3.forward, surroundingColor.Value);
                DrawSquare(blockPosition + Vector3.back, surroundingColor.Value);
                DrawSquare(blockPosition + Vector3.left, surroundingColor.Value);
                DrawSquare(blockPosition + Vector3.right, surroundingColor.Value);

                DrawSquare(blockPosition + Vector3.forward + Vector3.left, surroundingColor.Value.MulA(0.5f));
                DrawSquare(blockPosition + Vector3.forward + Vector3.right, surroundingColor.Value.MulA(0.5f));
                DrawSquare(blockPosition + Vector3.back + Vector3.left, surroundingColor.Value.MulA(0.5f));
                DrawSquare(blockPosition + Vector3.back + Vector3.right, surroundingColor.Value.MulA(0.5f));
            }
        }

        public void DrawSquare(Vector3 blockPosition, Color color)
        {
            var tileSize = _creator.Asset.Settings.TileSize;
            var size = new Vector3(1f, 0.05f, 1f) * tileSize;
            var center = blockPosition + 0.5f * tileSize * new Vector3(1f, 0f, 1f);
            EasyHandleHelper.PushColor(color);
            Handles.DrawWireCube(center, size);
            EasyHandleHelper.PopColor();
        }

        public void DrawCube(Vector3 blockPosition, Color color)
        {
            var tileSize = _creator.Asset.Settings.TileSize;
            EasyHandleHelper.PushColor(color);

            var center = blockPosition + Vector3.one * (tileSize * 0.5f);
            Handles.DrawWireCube(center, Vector3.one * tileSize);

            EasyHandleHelper.PopColor();
        }

        public void DrawFillCube(Vector3 blockPosition, Color color)
        {
            var tileSize = _creator.Asset.Settings.TileSize;
            EasyHandleHelper.PushColor(color);

            var center = blockPosition + Vector3.one * (tileSize * 0.5f);
            Handles.CubeHandleCap(0, center, Quaternion.identity, tileSize, EventType.Repaint);

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
