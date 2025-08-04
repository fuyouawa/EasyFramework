using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Tilemap.Editor
{
    [Serializable]
    public class TilemapCreatorEditorContext
    {
        [SerializeField]
        private Dictionary<Vector3Int, TerrainTileRuleType> _ruleTypeMapCache =
            new Dictionary<Vector3Int, TerrainTileRuleType>();

        public TilemapCreator Target;
        public bool IsMarkingRuleType = false;

        public Dictionary<Vector3Int, TerrainTileRuleType> RuleTypeMapCache =>
            _ruleTypeMapCache ??= new Dictionary<Vector3Int, TerrainTileRuleType>();
    }

    [CustomEditor(typeof(TilemapCreator))]
    public class TilemapCreatorEditor : EasyEditor
    {
        private TilemapCreator _target;
        private LocalPersistentContext<TilemapCreatorEditorContext> _context;

        private static readonly Dictionary<DrawMode, IDrawTool> DrawToolsByMode = new Dictionary<DrawMode, IDrawTool>
        {
            { DrawMode.Brush, new BrushTool() },
            { DrawMode.Eraser, new EraseTool() },
            { DrawMode.Rectangle, new RectangeDrawTool() },
        };

        protected override void OnEnable()
        {
            base.OnEnable();
            _target = (TilemapCreator)target;

            _context = Tree.LogicRootProperty.GetPersistentContext(nameof(TilemapCreatorEditorContext),
                new TilemapCreatorEditorContext());

            if (_context.Value.Target != _target)
            {
                _context.Value.Target = _target;
                _context.Value.IsMarkingRuleType = false;
                _context.Value.RuleTypeMapCache.Clear();
            }
        }

        protected override void DrawTree()
        {
            Tree.BeginDraw();
            Tree.DrawProperties();

            EasyEditorGUI.Title("调试", textAlignment: TextAlignment.Center);
            var newMarkingState = GUILayout.Toggle(_context.Value.IsMarkingRuleType, "标注瓦片规则类型");
            if (newMarkingState != _context.Value.IsMarkingRuleType)
            {
                _context.Value.IsMarkingRuleType = newMarkingState;
                TilemapSceneViewHandler.SetRuleTypeMarking(_context.Value.IsMarkingRuleType);
            }

            if (GUILayout.Button("重新扫描规则类型", GUILayout.Height(30)))
            {
                _context.Value.RuleTypeMapCache.Clear();
                TilemapSceneViewHandler.ClearRuleTypeCache();

                foreach (var block in _target.Asset.TerrainMap)
                {
                    var ruleType = _target.Asset.TerrainMap.CalculateRuleTypeAt(block.TilePosition);
                    _context.Value.RuleTypeMapCache[block.TilePosition] = ruleType;
                    TilemapSceneViewHandler.AddRuleTypeToCache(block.TilePosition, ruleType);
                }
            }

            EasyEditorGUI.Title("地图构建", textAlignment: TextAlignment.Center);
            if (GUILayout.Button("初始化地图", GUILayout.Height(30)))
            {
                _target.EnsureInitializeMap();
            }

            if (GUILayout.Button("重新构建所有地图", GUILayout.Height(30)))
            {
                _target.ClearAllMap();
                _target.BuildAllMap();
            }

            if (GUILayout.Button("清除所有地图", GUILayout.Height(30)))
            {
                _target.ClearAllMap();
            }

            Tree.EndDraw();
        }

        void OnSceneGUI()
        {
            TilemapSceneViewHandler.DrawSceneGUIFor(
                _target,
                _context.Value.IsMarkingRuleType,
                _context.Value.RuleTypeMapCache);

            if (TryGetHit(out var hitPoint, out var hittedBlockPosition))
            {
                if (_target.Asset.Settings.DrawDebugData)
                {
                    TilemapHandles.DrawDebugHitPointGUI(hitPoint);
                }

                DrawToolsByMode[TerrainDefinitionDrawer.SelectedDrawMode].OnSceneGUI(_target, hitPoint, hittedBlockPosition);
                SceneView.RepaintAll();
            }
        }


        private bool TryGetHit(out Vector3 hitPoint, out Vector3? hittedBlockPosition)
        {
            hitPoint = Vector3.zero;
            hittedBlockPosition = null;
            if (TerrainDefinitionDrawer.SelectedGuid == null)
            {
                return false;
            }

            var tileSize = _target.Asset.Settings.TileSize;
            bool handledHit = false;

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            float previousRaycastDistance = float.MaxValue;

            foreach (var block in _target.Asset.TerrainMap.EnumerateMatchedMap(
                         TerrainDefinitionDrawer.SelectedGuid.Value))
            {
                var blockPosition = TilemapUtility.TilePositionToWorldPosition(
                    _target.transform.position,
                    block.TilePosition,
                        tileSize);

                var center = blockPosition + Vector3.one * (tileSize * 0.5f);
                var bounds = new Bounds(center, tileSize * Vector3.one);

                if (bounds.IntersectRay(ray, out var distance))
                {
                    if (_target.Asset.Settings.DrawDebugData)
                    {
                        TilemapHandles.DrawDebugBlockGUI(blockPosition, distance);
                    }

                    if (distance < previousRaycastDistance)
                    {
                        hitPoint = ray.GetPoint(distance);
                        hittedBlockPosition = blockPosition;
                        handledHit = true;
                        previousRaycastDistance = distance;
                    }
                }
            }

            if (!handledHit)
            {
                var plane = new Plane(Vector3.up, _target.transform.position);
                if (plane.Raycast(ray, out float enter))
                {
                    hitPoint = ray.GetPoint(enter);
                    if (hitPoint.y < 0)
                    {
                        hitPoint = hitPoint.SetY(0);
                    }

                    TilemapHandles.DrawDebugBlockGUI(
                        TilemapUtility.WorldPositionToBlockPosition(_target.transform.position, hitPoint, tileSize),
                        enter);
                    handledHit = true;
                }
            }

            return handledHit;
        }
    }
}
