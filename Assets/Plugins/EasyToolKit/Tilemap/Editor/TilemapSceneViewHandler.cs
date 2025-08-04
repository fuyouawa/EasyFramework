using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Tilemap.Editor
{
    [InitializeOnLoad]
    public static class TilemapSceneViewHandler
    {
        private static TilemapCreator currentTarget;
        private static TilemapCreatorDrawer drawer;
        private static bool isMarkingRuleType = false;
        private static Dictionary<Vector3Int, TerrainTileRuleType> ruleTypeMapCache = new Dictionary<Vector3Int, TerrainTileRuleType>();
        private static Vector3? hittedBlockPosition;
        private static HashSet<Vector3Int> dragTilePositions = new HashSet<Vector3Int>();
        private static bool isDragging = false;

        static TilemapSceneViewHandler()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            Selection.selectionChanged += OnSelectionChanged;
            OnSelectionChanged();
        }

        private static void OnSelectionChanged()
        {
            if (Selection.activeGameObject != null)
            {
                var newTarget = Selection.activeGameObject.GetComponent<TilemapCreator>();
                if (newTarget != null)
                {
                    currentTarget = newTarget;
                    drawer = new TilemapCreatorDrawer(currentTarget);
                }
            }
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (currentTarget == null || Selection.activeGameObject == currentTarget.gameObject)
                return;

            DrawSceneGUIInternal(currentTarget, drawer, isMarkingRuleType, ruleTypeMapCache, ref hittedBlockPosition);
        }

        public static void SetRuleTypeMarking(bool value)
        {
            isMarkingRuleType = value;
        }

        public static void ClearRuleTypeCache()
        {
            ruleTypeMapCache.Clear();
        }

        public static void AddRuleTypeToCache(Vector3Int tilePosition, TerrainTileRuleType ruleType)
        {
            ruleTypeMapCache[tilePosition] = ruleType;
        }

        public static void DrawSceneGUIFor(TilemapCreator target, TilemapCreatorDrawer targetDrawer,
            bool markingRuleType, Dictionary<Vector3Int, TerrainTileRuleType> ruleTypeCache, ref Vector3? hitBlockPosition)
        {
            DrawSceneGUIInternal(target, targetDrawer, markingRuleType, ruleTypeCache, ref hitBlockPosition);
        }

        private static void DrawSceneGUIInternal(TilemapCreator target, TilemapCreatorDrawer targetDrawer,
            bool markingRuleType, Dictionary<Vector3Int, TerrainTileRuleType> ruleTypeCache, ref Vector3? hitBlockPosition)
        {
            if (target.Asset == null)
                return;

            var selectedItemGuid = TerrainDefinitionDrawer.SelectedGuid;
            targetDrawer.SelectedTerrainDefinition = selectedItemGuid != null ?
                target.Asset.TerrainMap.DefinitionSet.TryGetByGuid(selectedItemGuid.Value) : null;

            // targetDrawer.DrawBaseRange();

            if (target.Asset.Settings.DrawDebugBase)
            {
                targetDrawer.DrawBase();
            }

            if (targetDrawer.DrawHit(out var hitPoint, out hitBlockPosition))
            {
                DoHit(target, targetDrawer, hitPoint, ref hitBlockPosition);
                SceneView.RepaintAll();
            }

            if (markingRuleType)
            {
                foreach (var kvp in ruleTypeCache)
                {
                    var tilePosition = kvp.Key;
                    var ruleType = kvp.Value;
                    var tileWorldPosition = target.TilePositionToWorldPosition(tilePosition);
                    targetDrawer.DrawDebugRuleTypeGUI(tileWorldPosition, ruleType);
                }
            }
        }

        private static void DoHit(TilemapCreator target, TilemapCreatorDrawer targetDrawer, Vector3 hitPoint, ref Vector3? hitBlockPosition)
        {
            if (!targetDrawer.IsInRange(hitPoint))
                return;
            targetDrawer.DrawDebugHitPointGUI(hitPoint);

            var blockPosition = hitBlockPosition ?? target.WorldPositionToBlockPosition(hitPoint);
            var tilePosition = target.WorldPositionToTilePosition(blockPosition);
            var targetTerrainDefinition = target.Asset.TerrainMap.TryGetDefinitionAt(tilePosition);

            var isErase = TerrainDefinitionDrawer.SelectedDrawMode == DrawMode.Eraser;
            bool drawCube = true;
            if (isErase)
            {
                if (targetTerrainDefinition != targetDrawer.SelectedTerrainDefinition)
                {
                    drawCube = false;
                }
            }
            else
            {
                if (targetTerrainDefinition != null)
                {
                    targetDrawer.FixHitBlockPositionWithExclude(ref blockPosition, hitPoint);
                    if (!targetDrawer.IsInRange(blockPosition))
                    {
                        return;
                    }

                    tilePosition = target.WorldPositionToTilePosition(blockPosition);
                }
            }

            var hitColor = isErase ? Color.red.SetA(0.4f) : targetDrawer.SelectedTerrainDefinition.DebugCubeColor;
            if (drawCube)
            {
                var surroundingColor = Color.white.SetA(0.2f);
                targetDrawer.DrawHitCube(blockPosition, hitColor, surroundingColor);
            }

            foreach (var dragTilePosition in dragTilePositions)
            {
                targetDrawer.DrawHitCube(dragTilePosition, hitColor.MulA(0.7f));
            }

            if (IsMouseDown())
            {
                // Start a new drag operation
                dragTilePositions.Clear();
                dragTilePositions.Add(tilePosition);
                isDragging = true;

                // We don't apply changes immediately anymore, just store the position
                FinishMouse();
            }
            else if (IsMouseDrag() && isDragging)
            {
                // Continue the drag operation
                if (!dragTilePositions.Contains(tilePosition))
                {
                    dragTilePositions.Add(tilePosition);
                }
                FinishMouse();
            }
            else if (IsMouseUp() && isDragging)
            {
                // End the drag operation - now apply all changes at once
                if (dragTilePositions.Count > 0)
                {
                    // Record a single undo operation for all changes
                    Undo.RecordObject(target.Asset, $"Draw tiles in {target.Asset.name}");

                    // Apply all tile changes
                    foreach (var pos in dragTilePositions)
                    {
                        ApplyTileChange(target, targetDrawer, pos, false);
                    }

                    // Mark the asset as dirty once for all changes
                    EasyEditorUtility.SetUnityObjectDirty(target.Asset);
                }

                isDragging = false;
                dragTilePositions.Clear();
                FinishMouse();
            }
        }

        private static void ApplyTileChange(TilemapCreator target, TilemapCreatorDrawer targetDrawer, Vector3Int tilePosition, bool markDirty = true)
        {
            switch (TerrainDefinitionDrawer.SelectedDrawMode)
            {
                case DrawMode.Brush:
                    target.Asset.TerrainMap.SetTileAt(tilePosition, targetDrawer.SelectedTerrainDefinition.Guid);
                    break;
                case DrawMode.Eraser:
                    target.Asset.TerrainMap.RemoveTileAt(tilePosition);

                    if (target.Asset.Settings.RealTimeIncrementalBuild)
                    {
                        target.DestroyTileAt(targetDrawer.SelectedTerrainDefinition.Guid, tilePosition);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (target.Asset.Settings.RealTimeIncrementalBuild)
            {
                target.IncrementalBuildAt(targetDrawer.SelectedTerrainDefinition.Guid, tilePosition);
            }

            if (markDirty)
            {
                EasyEditorUtility.SetUnityObjectDirty(target.Asset);
            }
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
    }
}