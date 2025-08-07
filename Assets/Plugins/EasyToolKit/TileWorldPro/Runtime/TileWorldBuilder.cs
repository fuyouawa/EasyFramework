using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using EasyToolKit.Inspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EasyToolKit.TileWorldPro
{
    public class TileWorldBuilder : MonoBehaviour
    {
        [LabelText("起始点")]
        [SerializeField] private TileWorldStartPoint _startPoint;

        [FoldoutBoxGroup("设置")]
        [HideLabel]
        [SerializeField] private TileWorldBuilderSettings _settings;

        [EndFoldoutBoxGroup]
        [LabelText("瓦片世界")]
        [SerializeField] private TileWorldAsset _tileWorldAsset;

        [LabelText("地形配置")]
        [InlineEditor(Style = InlineEditorStyle.Foldout)]
        [SerializeField] private TerrainConfigAsset _terrainConfigAsset;

        private TerrainObjectManager _terrainObjectManager;

        public TileWorldStartPoint StartPoint => _startPoint;

        public TileWorldBuilderSettings Settings => _settings;

        public TileWorldAsset TileWorldAsset => _tileWorldAsset;

        public TerrainObjectManager TerrainObjectManager
        {
            get
            {
                if (_terrainObjectManager == null)
                {
                    _terrainObjectManager = transform.GetComponentInChildren<TerrainObjectManager>();
                    if (_terrainObjectManager == null)
                    {
                        _terrainObjectManager = new GameObject("TerrainObjectManager").AddComponent<TerrainObjectManager>();
                        _terrainObjectManager.transform.SetParent(transform);
                    }

                    _terrainObjectManager.Initialize(this);
                }

                return _terrainObjectManager;
            }
        }

        private bool[,] GetSudokuAt(TilePosition tilePosition)
        {
            var sudoku = new bool[3, 3];
            var terrainGuid = _tileWorldAsset.TryGetTerrainGuidAt(tilePosition);
            Assert.IsNotNull(terrainGuid);
            sudoku[0, 2] = _tileWorldAsset.TryGetTerrainGuidAt(tilePosition + TilePosition.ForwardLeft) == terrainGuid;
            sudoku[1, 2] = _tileWorldAsset.TryGetTerrainGuidAt(tilePosition + TilePosition.Forward) == terrainGuid;
            sudoku[2, 2] = _tileWorldAsset.TryGetTerrainGuidAt(tilePosition + TilePosition.ForwardRight) == terrainGuid;

            sudoku[0, 1] = _tileWorldAsset.TryGetTerrainGuidAt(tilePosition + TilePosition.Left) == terrainGuid;
            sudoku[1, 1] = true;
            sudoku[2, 1] = _tileWorldAsset.TryGetTerrainGuidAt(tilePosition + TilePosition.Right) == terrainGuid;

            sudoku[0, 0] = _tileWorldAsset.TryGetTerrainGuidAt(tilePosition + TilePosition.BackLeft) == terrainGuid;
            sudoku[1, 0] = _tileWorldAsset.TryGetTerrainGuidAt(tilePosition + TilePosition.Back) == terrainGuid;
            sudoku[2, 0] = _tileWorldAsset.TryGetTerrainGuidAt(tilePosition + TilePosition.BackRight) == terrainGuid;

            return sudoku;
        }

        public TerrainTileRuleType CalculateRuleTypeOf(TilePosition tilePosition)
        {
            var sudoku = GetSudokuAt(tilePosition);
            return _terrainConfigAsset.GetRuleTypeBySudoku(sudoku);
        }

        public void RebuildAll()
        {
            foreach (var terrainDefinition in _tileWorldAsset.TerrainDefinitionSet)
            {
                RebuildTerrain(terrainDefinition.Guid);
            }
        }

        public void RebuildTerrain(Guid terrainGuid)
        {
            ClearTerrain(terrainGuid);
            foreach (var position in _tileWorldAsset
                    .EnumerateChunks()
                    .SelectMany(chunk => chunk.EnumerateTerrainTiles(terrainGuid)))
            {
                BuildTile(position);
            }
        }

        public bool BuildTile(TerrainTilePosition terrainTilePosition)
        {
            var tilePosition = terrainTilePosition.TilePosition;
            var ruleType = CalculateRuleTypeOf(tilePosition);
            var terrainObject = TerrainObjectManager.GetTerrainObject(terrainTilePosition.TerrainGuid);
            return terrainObject.AddTile(tilePosition, ruleType);
        }

        public void ClearTerrain(Guid terrainGuid)
        {
            TerrainObjectManager.GetTerrainObject(terrainGuid).Clear();
        }
    }
}