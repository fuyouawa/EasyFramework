using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Tilemap.Editor
{
    [CustomEditor(typeof(TilemapCreator))]
    public class TilemapCreatorEditor : EasyEditor
    {
        private TerrainTileDefinition _terrainTileDefinition;
        private TilemapCreator _target;

        private bool _isMarkingRuleType = false;
        private Dictionary<Vector3Int, TerrainRuleType> _ruleTypeMapCache = new Dictionary<Vector3Int, TerrainRuleType>();

        protected override void OnEnable()
        {
            base.OnEnable();
            _target = (TilemapCreator)target;
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw();
            Tree.DrawProperties();

            EasyEditorGUI.Title("调试", textAlignment: TextAlignment.Center);
            _isMarkingRuleType = GUILayout.Toggle(_isMarkingRuleType, "标注瓦片规则类型");
            if (GUILayout.Button("重新扫描规则类型", GUILayout.Height(30)))
            {
                _ruleTypeMapCache.Clear();
                foreach (var block in _target.Asset.TerrainTileMap)
                {
                    _ruleTypeMapCache[block.TilePosition] = _target.Asset.TerrainTileMap.CalculateRuleTypeAt(block.TilePosition);
                }
            }

            EasyEditorGUI.Title("地图生成", textAlignment: TextAlignment.Center);
            if (GUILayout.Button("初始化地图", GUILayout.Height(30)))
            {
                _target.EnsureInitializeMap();
            }

            if (GUILayout.Button("重新生成所有地图", GUILayout.Height(30)))
            {
                _target.ClearAllMap();
                _target.GenerateAllMap();
            }

            if (GUILayout.Button("清除所有地图", GUILayout.Height(30)))
            {
                _target.ClearAllMap();
            }

            Tree.EndDraw();
        }

        void OnSceneGUI()
        {
            if (_target.Asset == null)
                return;

            var selectedItemGuid = TerrainTileDefinitionDrawer.SelectedItemGuid;
            if (selectedItemGuid.HasValue)
            {
                _terrainTileDefinition = _target.Asset.TerrainTileMap.DefinitionsAsset.TryGetByGuid(selectedItemGuid.Value);
            }
            else
            {
                _terrainTileDefinition = null;
            }

            DrawBase();

            if (DrawHit(out var hitPoint))
            {
                DoHit(hitPoint);
                SceneView.RepaintAll();
            }
            
            if (_isMarkingRuleType)
            {
                foreach (var kvp in _ruleTypeMapCache)
                {
                    var tilePosition = kvp.Key;
                    var ruleType = kvp.Value;
                    var tileWorldPosition = _target.TilePositionToWorldPosition(tilePosition);
                    DrawDebugRuleTypeGUI(tileWorldPosition, ruleType);
                }
            }
        }

        private void DrawBase()
        {
            var baseRange = _target.Asset.Settings.BaseRange;
            var tileSize = _target.Asset.Settings.TileSize;

            EasyHandleHelper.PushColor(_target.Asset.Settings.BaseColor);
            for (int x = 0; x <= baseRange.x; x++)
            {
                var start = _target.transform.position + Vector3.right * (x * tileSize);
                var end = start + Vector3.forward * (tileSize * baseRange.y);
                Handles.DrawLine(start, end);
            }

            for (int y = 0; y <= baseRange.y; y++)
            {
                var start = _target.transform.position + Vector3.forward * (y * tileSize);
                var end = start + Vector3.right * (tileSize * baseRange.x);
                Handles.DrawLine(start, end);
            }

            EasyHandleHelper.PopColor();
        }

        private bool DrawHit(out Vector3 hitPoint)
        {
            var tileSize = _target.Asset.Settings.TileSize;
            bool handledHit = false;
            hitPoint = Vector3.zero;

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            float previousRaycastDistance = float.MaxValue;
            foreach (var block in _target.Asset.TerrainTileMap)
            {
                var blockPosition = _target.TilePositionToWorldPosition(block.TilePosition);

                if (block.Definition.DrawDebugCube)
                {
                    DrawCube(blockPosition, block.Definition.DebugCubeColor);
                }

                if (_terrainTileDefinition != null)
                {
                    var center = blockPosition + Vector3.one * (tileSize * 0.5f);
                    var bounds = new Bounds(center, tileSize * Vector3.one);

                    if (bounds.IntersectRay(ray, out var distance))
                    {
                        DrawDebugBlockGUI(blockPosition, distance);
                        if (distance < previousRaycastDistance)
                        {
                            hitPoint = ray.GetPoint(distance);
                            handledHit = true;
                            previousRaycastDistance = distance;
                        }
                    }
                }
            }

            if (_terrainTileDefinition != null && !handledHit)
            {
                var plane = new Plane(Vector3.up, _target.transform.position);
                if (plane.Raycast(ray, out float enter))
                {
                    hitPoint = ray.GetPoint(enter);
                    if (hitPoint.y < 0)
                    {
                        hitPoint = hitPoint.SetY(0);
                    }

                    DrawDebugBlockGUI(_target.WorldPositionToBlockPosition(hitPoint), enter);
                    handledHit = true;
                }
            }

            return handledHit;
        }

        private void DoHit(Vector3 hitPoint)
        {
            if (!IsInRange(hitPoint))
                return;
            DrawDebugHitPointGUI(hitPoint);

            var tilePosition = _target.WorldPositionToTilePosition(hitPoint);
            var blockPosition = _target.TilePositionToWorldPosition(tilePosition);
            var targetTerrainTileDefinition = _target.Asset.TerrainTileMap.TryGetDefinitionAt(tilePosition);

            var isErase = TerrainTileDefinitionDrawer.SelectedDrawMode == DrawMode.Eraser;
            bool drawCube = true;
            if (isErase)
            {
                if (targetTerrainTileDefinition != _terrainTileDefinition)
                {
                    drawCube = false;
                }
            }
            else
            {
                if (targetTerrainTileDefinition != null)
                {
                    FixHitBlockPosition(ref blockPosition, hitPoint);
                    if (!IsInRange(blockPosition))
                    {
                        return;
                    }

                    tilePosition = _target.WorldPositionToTilePosition(blockPosition);
                }
            }

            if (drawCube)
            {
                DrawCube(blockPosition, _terrainTileDefinition.DebugCubeColor);
                if (isErase)
                {
                    DrawCube(blockPosition, Color.red.SetA(0.2f));
                }
            }

            if (IsMouseDown())
            {

                switch (TerrainTileDefinitionDrawer.SelectedDrawMode)
                {
                    case DrawMode.Brush:
                        Undo.RecordObject(_target.Asset, $"Brush tile at {tilePosition} in {_target.Asset.name}");
                        _target.Asset.TerrainTileMap.SetTileAt(tilePosition, _terrainTileDefinition.Guid);
                        break;
                    case DrawMode.Eraser:
                        Undo.RecordObject(_target.Asset, $"Erase tile at {tilePosition} in {_target.Asset.name}");
                        _target.Asset.TerrainTileMap.RemoveTileAt(tilePosition);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                EasyEditorUtility.SetUnityObjectDirty(_target.Asset);

                FinishMouseDown();
            }
        }

        private void FixHitBlockPosition(ref Vector3 blockPosition, Vector3 hitPoint)
        {
            var tileSize = _target.Asset.Settings.TileSize;

            var front = new Rect(
                blockPosition.x, blockPosition.y,
                tileSize, tileSize);

            if (hitPoint.z.Approximately(blockPosition.z) &&
                front.Contains(new Vector2(hitPoint.x, hitPoint.y)))
            {
                blockPosition.z -= tileSize;
                return;
            }

            if (hitPoint.z.Approximately(blockPosition.z + 1) &&
                front.Contains(new Vector2(hitPoint.x, hitPoint.y)))
            {
                blockPosition.z += tileSize;
                return;
            }

            var side = new Rect(
                blockPosition.z, blockPosition.y,
                tileSize, tileSize);

            if (hitPoint.x.Approximately(blockPosition.x) &&
                side.Contains(new Vector2(hitPoint.z, hitPoint.y)))
            {
                blockPosition.x -= tileSize;
                return;
            }

            if (hitPoint.x.Approximately(blockPosition.x + 1) &&
                side.Contains(new Vector2(hitPoint.z, hitPoint.y)))
            {
                blockPosition.x += tileSize;
                return;
            }

            var top = new Rect(
                blockPosition.x, blockPosition.z,
                tileSize, tileSize);

            if (hitPoint.y.Approximately(blockPosition.y) &&
                top.Contains(new Vector2(hitPoint.x, hitPoint.z)))
            {
                blockPosition.y -= tileSize;
                return;
            }

            if (hitPoint.y.Approximately(blockPosition.y + 1) &&
                top.Contains(new Vector2(hitPoint.x, hitPoint.z)))
            {
                blockPosition.y += tileSize;
                return;
            }
        }

        private void DrawDebugHitPointGUI(Vector3 hitPoint)
        {
            if (!_target.Asset.Settings.DrawDebugData)
                return;

            Handles.BeginGUI();

            Vector2 guiPosition = HandleUtility.WorldToGUIPoint(hitPoint);
            GUI.Label(new Rect(guiPosition.x + 10, guiPosition.y - 10, 200, 20), $"{hitPoint}");

            Handles.EndGUI();
        }

        private void DrawDebugRuleTypeGUI(Vector3 tileWorldPosition, TerrainRuleType ruleType)
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

        private void DrawDebugBlockGUI(Vector3 gridWorldPosition, float distance)
        {
            if (!_target.Asset.Settings.DrawDebugData)
                return;

            Handles.BeginGUI();

            Vector2 guiPosition = HandleUtility.WorldToGUIPoint(gridWorldPosition);
            GUI.Label(new Rect(guiPosition.x, guiPosition.y - 20, 200, 20), $"{gridWorldPosition} - {distance:F2}");

            Handles.EndGUI();
        }

        private bool IsInRange(Vector3 hitPoint)
        {
            if (hitPoint.y < _target.transform.position.y)
            {
                return false;
            }

            var tileSize = _target.Asset.Settings.TileSize;
            var baseRange = _target.Asset.Settings.BaseRange;

            var local = hitPoint - _target.transform.position;
            var gridX = Mathf.FloorToInt(local.x / tileSize);
            var grisZ = Mathf.FloorToInt(local.z / tileSize);

            if (gridX < 0 || gridX >= baseRange.x || grisZ < 0 || grisZ >= baseRange.y)
                return false;

            return true;
        }

        private void DrawCube(Vector3 blockPosition, Color color)
        {
            var tileSize = _target.Asset.Settings.TileSize;
            EasyHandleHelper.PushColor(color.SetA(1f));

            var center = blockPosition + Vector3.one * (tileSize * 0.5f);
            Handles.DrawWireCube(center, Vector3.one * tileSize);

            if (!color.a.Approximately(0f))
            {
                Handles.color = color;
                Handles.CubeHandleCap(0, center, Quaternion.identity, tileSize, EventType.Repaint);
            }

            EasyHandleHelper.PopColor();
        }

        private bool IsMouseDown()
        {
            var e = Event.current;
            return e.type == EventType.MouseDown && e.button == 0;
        }

        private void FinishMouseDown()
        {
            Event.current.Use();
        }
    }
}
