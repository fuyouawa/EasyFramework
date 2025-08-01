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
        private static readonly float Epsilon = TilemapCreator.Epsilon;

        private TilemapCreator _target;
        private TilemapCreatorDrawer _drawer;

        private bool _isMarkingRuleType = false;
        private readonly Dictionary<Vector3Int, TerrainRuleType> _ruleTypeMapCache = new Dictionary<Vector3Int, TerrainRuleType>();

        private Vector3? _hittedBlockPosition;

        protected override void OnEnable()
        {
            base.OnEnable();
            _target = (TilemapCreator)target;
            _drawer = new TilemapCreatorDrawer(_target);
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
            _drawer.SelectedTerrainTileDefinition = selectedItemGuid != null ? _target.Asset.TerrainTileMap.DefinitionsAsset.TryGetByGuid(selectedItemGuid.Value) : null;

            _drawer.DrawBase();

            if (_drawer.DrawHit(out var hitPoint, out _hittedBlockPosition))
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
                    _drawer.DrawDebugRuleTypeGUI(tileWorldPosition, ruleType);
                }
            }
        }

        private void DoHit(Vector3 hitPoint)
        {
            if (!_drawer.IsInRange(hitPoint))
                return;
            _drawer.DrawDebugHitPointGUI(hitPoint);

            var blockPosition = _hittedBlockPosition ?? _target.WorldPositionToBlockPosition(hitPoint);
            var tilePosition = _target.WorldPositionToTilePosition(blockPosition);
            var targetTerrainTileDefinition = _target.Asset.TerrainTileMap.TryGetDefinitionAt(tilePosition);

            var isErase = TerrainTileDefinitionDrawer.SelectedDrawMode == DrawMode.Eraser;
            bool drawCube = true;
            if (isErase)
            {
                if (targetTerrainTileDefinition != _drawer.SelectedTerrainTileDefinition)
                {
                    drawCube = false;
                }
            }
            else
            {
                if (targetTerrainTileDefinition != null)
                {
                    _drawer.FixHitBlockPositionWithExclude(ref blockPosition, hitPoint);
                    if (!_drawer.IsInRange(blockPosition))
                    {
                        return;
                    }

                    tilePosition = _target.WorldPositionToTilePosition(blockPosition);
                }
            }

            if (drawCube)
            {
                _drawer.DrawCube(blockPosition);
                if (isErase)
                {
                    _drawer.DrawCube(blockPosition, Color.red.SetA(0.2f));
                }
            }

            if (IsMouseDown())
            {

                switch (TerrainTileDefinitionDrawer.SelectedDrawMode)
                {
                    case DrawMode.Brush:
                        Undo.RecordObject(_target.Asset, $"Brush tile at {tilePosition} in {_target.Asset.name}");
                        _target.Asset.TerrainTileMap.SetTileAt(tilePosition, _drawer.SelectedTerrainTileDefinition.Guid);
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
