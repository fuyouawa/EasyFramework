// using EasyToolKit.Core;
// using EasyToolKit.Core.Editor;
// using EasyToolKit.Inspector.Editor;
// using System;
// using System.Collections.Generic;
// using UnityEditor;
// using UnityEngine;

// namespace EasyToolKit.TileWorldPro.Editor
// {
//     [InitializeOnLoad]
//     public static class TilemapSceneViewHandler
//     {
//         private static TilemapContext _context;

//         static TilemapSceneViewHandler()
//         {
//             SceneView.duringSceneGui += OnSceneGUI;
//         }

//         private static void OnSceneGUI(SceneView sceneView)
//         {
//             if (_context == null || Selection.activeGameObject == _context.Target.gameObject)
//                 return;

//             DrawSceneGUI(_context);
//         }

//         public static void DrawSceneGUI(TilemapContext context)
//         {
//             _context = context;

//             if (context.Target.Asset == null)
//                 return;

//             var baseRange = context.Target.Asset.Settings.BaseRange;
//             var tileSize = context.Target.Asset.Settings.TileSize;

//             if (context.Target.Asset.Settings.DrawDebugBase)
//             {
//                 TileWorldHandles.DrawBase(context.Target.transform.position, baseRange, tileSize, context.Target.Asset.Settings.BaseDebugColor);
//             }

//             TileWorldHandles.DrawTileCubes(context.Target.transform.position, context.Target.Asset.TerrainMap, tileSize);

//             if (context.IsMarkingRuleType)
//             {
//                 foreach (var kvp in context.RuleTypeMapCache)
//                 {
//                     var tilePosition = kvp.Key;
//                     var ruleType = kvp.Value;
//                     var tileWorldPosition = TilemapUtility.TilePositionToWorldPosition(context.Target.transform.position, tilePosition, tileSize);
//                     TileWorldHandles.DrawDebugRuleTypeGUI(tileWorldPosition, ruleType);
//                 }
//             }
//         }
//     }
// }
