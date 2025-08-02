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
        private static Dictionary<Vector3Int, TerrainRuleType> ruleTypeMapCache = new Dictionary<Vector3Int, TerrainRuleType>();
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

        public static void AddRuleTypeToCache(Vector3Int tilePosition, TerrainRuleType ruleType)
        {
            ruleTypeMapCache[tilePosition] = ruleType;
        }

        public static void DrawSceneGUIFor(TilemapCreator target, TilemapCreatorDrawer targetDrawer,
            bool markingRuleType, Dictionary<Vector3Int, TerrainRuleType> ruleTypeCache, ref Vector3? hitBlockPosition)
        {
            DrawSceneGUIInternal(target, targetDrawer, markingRuleType, ruleTypeCache, ref hitBlockPosition);
        }

        private static void DrawSceneGUIInternal(TilemapCreator target, TilemapCreatorDrawer targetDrawer,
            bool markingRuleType, Dictionary<Vector3Int, TerrainRuleType> ruleTypeCache, ref Vector3? hitBlockPosition)
        {
            if (target.Asset == null)
                return;

            var selectedItemGuid = TerrainTileDefinitionDrawer.SelectedItemGuid;
            targetDrawer.SelectedTerrainTileDefinition = selectedItemGuid != null ?
                target.Asset.TerrainTileMap.DefinitionSet.TryGetByGuid(selectedItemGuid.Value) : null;

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
            var targetTerrainTileDefinition = target.Asset.TerrainTileMap.TryGetDefinitionAt(tilePosition);

            var isErase = TerrainTileDefinitionDrawer.SelectedDrawMode == DrawMode.Eraser;
            bool drawCube = true;
            if (isErase)
            {
                if (targetTerrainTileDefinition != targetDrawer.SelectedTerrainTileDefinition)
                {
                    drawCube = false;
                }
            }
            else
            {
                if (targetTerrainTileDefinition != null)
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
                targetDrawer.DrawCube(blockPosition, targetDrawer.SelectedTerrainTileDefinition.DebugCubeColor.SetA(1f));
                if (isErase)
                {
                    targetDrawer.DrawFillCube(blockPosition, Color.red.SetA(0.2f));
                }
            }

            if (IsMouseDown())
            {
                switch (TerrainTileDefinitionDrawer.SelectedDrawMode)
                {
                    case DrawMode.Brush:
                        Undo.RecordObject(target.Asset, $"Brush tile at {tilePosition} in {target.Asset.name}");
                        target.Asset.TerrainTileMap.SetTileAt(tilePosition, targetDrawer.SelectedTerrainTileDefinition.Guid);
                        break;
                    case DrawMode.Eraser:
                        Undo.RecordObject(target.Asset, $"Erase tile at {tilePosition} in {target.Asset.name}");
                        target.Asset.TerrainTileMap.RemoveTileAt(tilePosition);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                EasyEditorUtility.SetUnityObjectDirty(target.Asset);

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