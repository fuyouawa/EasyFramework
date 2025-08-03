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

            if (drawCube)
            {
                var hitColor = isErase ? Color.red.SetA(0.4f) : targetDrawer.SelectedTerrainDefinition.DebugCubeColor;
                var surroundingColor = Color.white.SetA(0.2f);
                targetDrawer.DrawHitCube(blockPosition, hitColor, surroundingColor);
            }

            if (IsMouseDown())
            {
                switch (TerrainDefinitionDrawer.SelectedDrawMode)
                {
                    case DrawMode.Brush:
                        Undo.RecordObject(target.Asset, $"Brush tile at {tilePosition} in {target.Asset.name}");
                        target.Asset.TerrainMap.SetTileAt(tilePosition, targetDrawer.SelectedTerrainDefinition.Guid);
                        break;
                    case DrawMode.Eraser:
                        Undo.RecordObject(target.Asset, $"Erase tile at {tilePosition} in {target.Asset.name}");
                        target.Asset.TerrainMap.RemoveTileAt(tilePosition);

                        if (target.Asset.Settings.RealTimeIncrementalBuild)
                        {
                            target.DestroyTileAt(targetDrawer.SelectedTerrainDefinition.Guid, tilePosition);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                EasyEditorUtility.SetUnityObjectDirty(target.Asset);

                if (target.Asset.Settings.RealTimeIncrementalBuild)
                {
                    target.IncrementalBuildAt(targetDrawer.SelectedTerrainDefinition.Guid, tilePosition);
                }

                FinishMouseDown();
            }
        }

        private static bool IsMouseDown()
        {
            var e = Event.current;
            return e.type == EventType.MouseDown && e.button == 0;
        }

        private static void FinishMouseDown()
        {
            Event.current.Use();
        }
    }
}
