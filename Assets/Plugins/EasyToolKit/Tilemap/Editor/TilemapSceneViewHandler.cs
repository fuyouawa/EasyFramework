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
        private static bool isMarkingRuleType = false;
        private static Dictionary<Vector3Int, TerrainTileRuleType> ruleTypeMapCache = new Dictionary<Vector3Int, TerrainTileRuleType>();

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
                }
            }
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (currentTarget == null || Selection.activeGameObject == currentTarget.gameObject)
                return;

            DrawSceneGUIInternal(currentTarget, isMarkingRuleType, ruleTypeMapCache);
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

        public static void DrawSceneGUIFor(TilemapCreator target, bool markingRuleType, Dictionary<Vector3Int, TerrainTileRuleType> ruleTypeCache)
        {
            DrawSceneGUIInternal(target,  markingRuleType, ruleTypeCache);
        }

        private static void DrawSceneGUIInternal(TilemapCreator target, bool markingRuleType, Dictionary<Vector3Int, TerrainTileRuleType> ruleTypeCache)
        {
            if (target.Asset == null)
                return;

            var baseRange = target.Asset.Settings.BaseRange;
            var tileSize = target.Asset.Settings.TileSize;

            if (target.Asset.Settings.DrawDebugBase)
            {
                TilemapHandles.DrawBase(target.transform.position, baseRange, tileSize, target.Asset.Settings.BaseDebugColor);
            }

            TilemapHandles.DrawTileCubes(target.transform.position, target.Asset.TerrainMap, tileSize);

            if (markingRuleType)
            {
                foreach (var kvp in ruleTypeCache)
                {
                    var tilePosition = kvp.Key;
                    var ruleType = kvp.Value;
                    var tileWorldPosition = TilemapUtility.TilePositionToWorldPosition(target.transform.position, tilePosition, tileSize);
                    TilemapHandles.DrawDebugRuleTypeGUI(tileWorldPosition, ruleType);
                }
            }
        }
    }
}
