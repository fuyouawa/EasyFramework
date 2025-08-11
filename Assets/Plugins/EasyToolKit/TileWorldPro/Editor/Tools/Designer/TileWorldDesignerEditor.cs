using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using EasyToolKit.TileWorldPro;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.TileWorldPro.Editor
{
    [CustomEditor(typeof(TileWorldDesigner))]
    public class TileWorldDesignerEditor : EasyEditor
    {
        private static readonly float UpdateInterval = 0.1f;
        private static double LastUpdateTime = 0;

        private TileWorldDesigner _target;
        private LocalPersistentContext<TileWorldDesignerContext> _context;


        private bool _lastIsHit;
        private Vector3 _lastHitPoint;
        private Vector3 _lastHitTileWorldPosition;

        private static readonly Dictionary<DrawMode, IDrawingTool> DrawToolsByMode = new Dictionary<DrawMode, IDrawingTool>
        {
            { DrawMode.Brush, new BrushTool() },
            { DrawMode.Eraser, new EraseTool() },
            { DrawMode.LineBrush, new LineBrushTool() },
            { DrawMode.RectangleBrush, new RectangeBrushTool() },
        };

        protected override void OnEnable()
        {
            base.OnEnable();
            _target = (TileWorldDesigner)target;

            _context = Tree.LogicRootProperty.GetPersistentContext(nameof(TileWorldDesignerContext),
                new TileWorldDesignerContext());

            if (_context.Value.Target != _target)
            {
                _context.Value.Target = _target;
            }

            EditorApplication.update += OnUpdate;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EditorApplication.update -= OnUpdate;
        }

        protected override void DrawTree()
        {
            TileWorldAssetEditor.IsInDesigner = true;
            base.DrawTree();
            TileWorldAssetEditor.IsInDesigner = false;
        }

        private void OnUpdate()
        {
            if (EditorApplication.timeSinceStartup - LastUpdateTime > UpdateInterval)
            {
                LastUpdateTime = EditorApplication.timeSinceStartup;
                SceneView.RepaintAll();
            }
        }

        void OnSceneGUI()
        {
            TileWorldSceneViewHandler.DrawSceneGUI(_context.Value);

            if (TerrainDefinitionDrawer.SelectedGuid == null)
            {
                return;
            }

            if (Event.current.type == EventType.MouseMove || Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseUp)
            {
                _lastIsHit = TryGetHit(out _lastHitPoint, out _lastHitTileWorldPosition);
            }

            if (_lastIsHit)
            {
                if (_target.Settings.DrawDebugData)
                {
                    TileWorldHandles.DrawDebugHitPointGUI(_lastHitPoint);
                }

                DrawToolsByMode[TerrainDefinitionDrawer.SelectedDrawMode].OnSceneGUI(new DrawingToolContext
                {
                    Target = _target,
                    HitPoint = _lastHitPoint,
                    HitTileWorldPosition = _lastHitTileWorldPosition,
                    TerrainDefinition = _target.TileWorldAsset.TerrainDefinitionSet.TryGetByGuid(TerrainDefinitionDrawer.SelectedGuid.Value)
                });
            }
        }


        private bool TryGetHit(out Vector3 hitPoint, out Vector3 hitTileWorldPosition)
        {
            hitPoint = Vector3.zero;
            hitTileWorldPosition = Vector3.zero;
            if (TerrainDefinitionDrawer.SelectedGuid == null)
            {
                return false;
            }

            var tileSize = _target.TileWorldAsset.TileSize;
            bool handledHit = false;

            // Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            // float previousRaycastDistance = float.MaxValue;

            // foreach (var tile in _target.TileWorldAsset
            //         .EnumerateChunks()
            //         .SelectMany(chunk => chunk
            //             .EnumerateTerrainTiles(TerrainDefinitionDrawer.SelectedGuid.Value)))
            // {
            //     var tileWorldPosition = _target.StartPoint.TilePositionToWorldPosition(tile.TilePosition, tileSize);

            //     var center = tileWorldPosition + Vector3.one * (tileSize * 0.5f);
            //     var bounds = new Bounds(center, tileSize * Vector3.one);

            //     if (bounds.IntersectRay(ray, out var distance))
            //     {
            //         if (_target.Settings.DrawDebugData)
            //         {
            //             TileWorldHandles.DrawDebugBlockGUI(tileWorldPosition, distance);
            //         }

            //         if (distance < previousRaycastDistance)
            //         {
            //             hitPoint = ray.GetPoint(distance);
            //             hittedBlockPosition = tileWorldPosition;
            //             handledHit = true;
            //             previousRaycastDistance = distance;
            //         }
            //     }
            // }

            var picked = HandleUtility.PickGameObject(Event.current.mousePosition, false);

            if (picked != null)
            {
                var tileWorldPosition = _target.StartPoint.WorldPositionToTileWorldPosition(picked.transform.position, tileSize);
                var center = tileWorldPosition + Vector3.one * (tileSize * 0.5f);
                var bounds = new Bounds(center, tileSize * Vector3.one);

                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                if (bounds.IntersectRay(ray, out var distance))
                {
                    if (_target.Settings.DrawDebugData)
                    {
                        TileWorldHandles.DrawDebugBlockGUI(tileWorldPosition, distance);
                    }

                    hitPoint = ray.GetPoint(distance);
                    hitTileWorldPosition = tileWorldPosition;
                    handledHit = true;
                }
            }

            if (!handledHit)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                var plane = new Plane(Vector3.up, _target.StartPoint.transform.position);
                if (plane.Raycast(ray, out float enter))
                {
                    hitPoint = ray.GetPoint(enter);
                    if (hitPoint.y < 0)
                    {
                        hitPoint = hitPoint.SetY(0);
                    }

                    TileWorldHandles.DrawDebugBlockGUI(
                        _target.StartPoint.WorldPositionToTileWorldPosition(hitPoint, tileSize),
                        enter);
                    hitTileWorldPosition = _target.StartPoint.WorldPositionToTileWorldPosition(hitPoint, tileSize);
                    handledHit = true;
                }
            }

            return handledHit;
        }
    }
}