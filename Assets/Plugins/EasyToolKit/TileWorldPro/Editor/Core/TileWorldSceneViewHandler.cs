using UnityEditor;

namespace EasyToolKit.TileWorldPro.Editor
{
    public static class TileWorldSceneViewHandler
    {
        private static TileWorldDesignerContext _context;

        static TileWorldSceneViewHandler()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (_context == null || Selection.activeGameObject == _context.Target.gameObject)
                return;

            DrawSceneGUI(_context);
        }

        public static void DrawSceneGUI(TileWorldDesignerContext context)
        {
            _context = context;

            if (context.Target.TileWorldAsset == null)
                return;

            if (context.Target.Settings.DrawDebugBase)
            {
                TileWorldHandles.DrawBase(context.Target);
            }

            TileWorldHandles.DrawTileCubes(context.Target);

            // if (context.IsMarkingRuleType)
            // {
            //     foreach (var kvp in context.RuleTypeMapCache)
            //     {
            //         var tilePosition = kvp.Key;
            //         var ruleType = kvp.Value;
            //         var tileWorldPosition = TilemapUtility.TilePositionToWorldPosition(context.Target.transform.position, tilePosition, tileSize);
            //         TilemapHandles.DrawDebugRuleTypeGUI(tileWorldPosition, ruleType);
            //     }
            // }
        }
    }
}