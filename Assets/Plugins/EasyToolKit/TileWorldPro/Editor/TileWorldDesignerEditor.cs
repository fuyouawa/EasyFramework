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
        private TileWorldDesigner _target;
        private LocalPersistentContext<TileWorldDesignerContext> _context;
        private static readonly float UpdateInterval = 0.1f;
        private static double LastUpdateTime = 0;

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

            if (TryGetHit(out var hitPoint, out var hittedBlockPosition))
            {
                if (_target.Settings.DrawDebugData)
                {
                    TileWorldHandles.DrawDebugHitPointGUI(hitPoint);
                }

                DrawToolsByMode[TerrainDefinitionDrawer.SelectedDrawMode].OnSceneGUI(_target, hitPoint, hittedBlockPosition);
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

            var tileSize = _target.TileWorldAsset.TileSize;
            bool handledHit = false;

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            float previousRaycastDistance = float.MaxValue;

            foreach (var tile in _target.TileWorldAsset
                    .EnumerateChunks()
                    .SelectMany(chunk => chunk
                        .EnumerateTerrainTiles(TerrainDefinitionDrawer.SelectedGuid.Value)))
            {
                var tileWorldPosition = _target.StartPoint.TilePositionToWorldPosition(tile.TilePosition, tileSize);

                var center = tileWorldPosition + Vector3.one * (tileSize * 0.5f);
                var bounds = new Bounds(center, tileSize * Vector3.one);

                if (bounds.IntersectRay(ray, out var distance))
                {
                    if (_target.Settings.DrawDebugData)
                    {
                        TileWorldHandles.DrawDebugBlockGUI(tileWorldPosition, distance);
                    }

                    if (distance < previousRaycastDistance)
                    {
                        hitPoint = ray.GetPoint(distance);
                        hittedBlockPosition = tileWorldPosition;
                        handledHit = true;
                        previousRaycastDistance = distance;
                    }
                }
            }

            if (!handledHit)
            {
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
                    handledHit = true;
                }
            }

            return handledHit;
        }
    }
}