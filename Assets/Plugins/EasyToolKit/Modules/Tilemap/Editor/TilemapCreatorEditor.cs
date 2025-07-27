using System;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Tilemap.Editor
{
    [CustomEditor(typeof(TilemapCreator))]
    public class TilemapCreatorEditor : EasyEditor
    {
        private TerrainTileDefinition _terrainTileDefinition;
        private TilemapCreator _target;

        void OnSceneGUI()
        {
            _target = (TilemapCreator)target;

            if (_target.Asset == null)
                return;
            
            if (TerrainTileDefinitionDrawer.SelectedItemGuid.HasValue)
            {
                _terrainTileDefinition = _target.Asset.TryGetTerrainTileDefinitionByGuid(TerrainTileDefinitionDrawer.SelectedItemGuid.Value);
            }
            else
            {
                _terrainTileDefinition = null;
            }

            var baseRange = _target.Asset.Settings.BaseRange;
            var tileSize = _target.Asset.Settings.TileSize;

            EasyHandleHelper.PushColor(_target.Asset.Settings.BaseColor);
            for (int x = 0; x <= baseRange.x; x++)
            {
                var start = _target.transform.position + Vector3.right * x * tileSize;
                var end = start + Vector3.forward * tileSize * baseRange.y;
                Handles.DrawLine(start, end);
            }

            for (int y = 0; y <= baseRange.y; y++)
            {
                var start = _target.transform.position + Vector3.forward * y * tileSize;
                var end = start + Vector3.right * tileSize * baseRange.x;
                Handles.DrawLine(start, end);
            }
            EasyHandleHelper.PopColor();
            
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            Vector3? hitPoint = null;
            foreach (var block in _target.Asset.EnumerateTerrainTiles())
            {
                var blockPosition = _target.TilePositionToWorldPosition(block.TilePosition);

                if (block.Definition.DrawDebugCube)
                {
                    DrawCube(blockPosition, block.Definition.Color);
                }

                if (_terrainTileDefinition != null && !hitPoint.HasValue)
                {
                    var center = blockPosition + Vector3.one * tileSize * 0.5f;
                    var bounds = new Bounds(center, tileSize * Vector3.one);

                    if (bounds.IntersectRay(ray, out var distance))
                    {
                        hitPoint = ray.GetPoint(distance);
                    }
                }
            }

            if (_terrainTileDefinition != null && !hitPoint.HasValue)
            {
                Plane plane = new Plane(Vector3.up, _target.transform.position);
                if (plane.Raycast(ray, out float enter))
                {
                    hitPoint = ray.GetPoint(enter);
                }
            }

            if (hitPoint.HasValue)
            {
                DoHit(hitPoint.Value);
                SceneView.RepaintAll();
            }
        }

        private void DoHit(Vector3 hitPoint)
        {
            var tileSize = _target.Asset.Settings.TileSize;
            var baseRange = _target.Asset.Settings.BaseRange;

            Vector3 local = hitPoint - _target.transform.position;
            int gridX = Mathf.FloorToInt(local.x / tileSize);
            int gridY = Mathf.FloorToInt(local.y / tileSize);
            int gridZ = Mathf.FloorToInt(local.z / tileSize);

            if (gridX < 0 || gridX >= baseRange.x || gridZ < 0 || gridZ >= baseRange.y)
                return;

            Vector3 blockPos = _target.transform.position + new Vector3(gridX * tileSize, gridY * tileSize, gridZ * tileSize);

            DrawCube(blockPos, _terrainTileDefinition.Color, TerrainTileDefinitionDrawer.SelectedDrawMode == DrawMode.Eraser);

            UpdateMouseDown(blockPos);
        }

        private void DrawCube(Vector3 blockPosition, Color color, bool isErase = false)
        {
            var tileSize = _target.Asset.Settings.TileSize;
            EasyHandleHelper.PushColor(color);

            var center = blockPosition + Vector3.one * tileSize * 0.5f;
            Handles.DrawWireCube(center, Vector3.one * tileSize);

            Handles.color = color.SetA(0.2f);
            Handles.CubeHandleCap(0, center, Quaternion.identity, tileSize, EventType.Repaint);

            if (isErase)
            {
                Handles.color = Color.red.SetA(0.3f);
                Handles.CubeHandleCap(0, center, Quaternion.identity, tileSize * 0.9f, EventType.Repaint);
            }

            EasyHandleHelper.PopColor();
        }

        private void UpdateMouseDown(Vector3 blockPosition)
        {
            var e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                var tilePosition = _target.WorldPositionToTilePosition(blockPosition);

                switch (TerrainTileDefinitionDrawer.SelectedDrawMode)
                {
                    case DrawMode.Brush:
                        _target.Asset.SetTerrainTile(tilePosition, _terrainTileDefinition);
                        break;
                    case DrawMode.Eraser:
                        var definition = _target.Asset.TryGetTerrainTile(tilePosition);
                        if (definition == _terrainTileDefinition)
                        {
                            _target.Asset.RemoveTerrainTile(tilePosition);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                e.Use();
            }
        }
    }
}
