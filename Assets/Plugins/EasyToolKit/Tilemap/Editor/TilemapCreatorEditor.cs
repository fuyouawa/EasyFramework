using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using EasyToolKit.Inspector.Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EasyToolKit.Tilemap.Editor
{
    public class TilemapCreatorEditorContext
    {
        public TilemapCreator Target;
        public bool IsMarkingRuleType = false;
        public Dictionary<Vector3Int, TerrainRuleType> RuleTypeMapCache = new Dictionary<Vector3Int, TerrainRuleType>();
    }

    [CustomEditor(typeof(TilemapCreator))]
    public class TilemapCreatorEditor : EasyEditor
    {
        private static readonly float Epsilon = TilemapCreator.Epsilon;

        private TilemapCreator _target;
        private TilemapCreatorDrawer _drawer;

        private LocalPersistentContext<TilemapCreatorEditorContext> _context;

        private Vector3? _hittedBlockPosition;

        protected override void OnEnable()
        {
            base.OnEnable();
            _target = (TilemapCreator)target;
            _drawer = new TilemapCreatorDrawer(_target);

            _context = Tree.LogicRootProperty.GetPersistentContext(nameof(TilemapCreatorEditorContext), new TilemapCreatorEditorContext());

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

                foreach (var block in _target.Asset.TerrainTileMap)
                {
                    var ruleType = _target.Asset.TerrainTileMap.CalculateRuleTypeAt(block.TilePosition);
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
                _drawer,
                _context.Value.IsMarkingRuleType,
                _context.Value.RuleTypeMapCache,
                ref _hittedBlockPosition);
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
