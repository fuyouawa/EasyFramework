using EasyToolKit.Core;
using EasyToolKit.Inspector;
using EasyToolKit.ThirdParty.OdinSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Guid = System.Guid;

namespace EasyToolKit.Tilemap
{
    public struct TerrainTileBlock
    {
        public Vector3Int TilePosition;
        public TerrainTileDefinition Definition;
    }

    [CreateAssetMenu(menuName = "EasyToolKit/Tilemap/Create TilemapAsset", fileName = "TilemapAsset")]
    [EasyInspector]
    public class TilemapAsset : SerializedScriptableObject
    {
        [FoldoutGroup("设置")]
        [HideLabel]
        [SerializeField] private TilemapSettings _settings = new TilemapSettings();

        [EndFoldoutGroup]
        [MetroListDrawerSettings(OnRemovedElementCallback = nameof(OnRemovedTerrainTileDefinition))]
        [LabelText("地形瓦片定义表")]
        [SerializeField]
        private List<TerrainTileDefinition> _terrainTileDefinitions = new List<TerrainTileDefinition>();

        [OdinSerialize] private Dictionary<Vector3Int, Guid> _terrainTileMap = new Dictionary<Vector3Int, Guid>();

        public TilemapSettings Settings => _settings;
        public List<TerrainTileDefinition> TerrainTileDefinitions => _terrainTileDefinitions;

        public TerrainTileDefinition TryGetTerrainTileDefinitionByGuid(Guid guid)
        {
            return TerrainTileDefinitions.FirstOrDefault(terrainTile => terrainTile.Guid == guid);
        }

        public void SetTileAt(Vector3Int tilePosition, Guid terrainTileDefinitionGuid)
        {
            _terrainTileMap[tilePosition] = terrainTileDefinitionGuid;
        }

        public TerrainTileDefinition TryGetTerrainTileDefinitionAt(Vector3Int tilePosition)
        {
            if (_terrainTileMap.TryGetValue(tilePosition, out var guid))
            {
                return TryGetTerrainTileDefinitionByGuid(guid);
            }

            return null;
        }

        public IEnumerable<TerrainTileBlock> EnumerateAllTerrainTiles()
        {
            foreach (var kvp in _terrainTileMap)
            {
                var tilePosition = kvp.Key;
                var guid = kvp.Value;
                var definition = TryGetTerrainTileDefinitionByGuid(guid);
                Assert.IsNotNull(definition);
                Assert.IsTrue(guid != Guid.Empty);

                yield return new TerrainTileBlock
                {
                    TilePosition = tilePosition,
                    Definition = definition,
                };
            }
        }

        public IEnumerable<TerrainTileBlock> EnumerateTerrainTiles(Guid targetTerrainTileDefinitionGuid)
        {
            var definition = TryGetTerrainTileDefinitionByGuid(targetTerrainTileDefinitionGuid);
            Assert.IsNotNull(definition);

            foreach (var kvp in _terrainTileMap)
            {
                var tilePosition = kvp.Key;
                var guid = kvp.Value;
                Assert.IsTrue(guid != Guid.Empty);

                if (guid == targetTerrainTileDefinitionGuid)
                {
                    yield return new TerrainTileBlock
                    {
                        TilePosition = tilePosition,
                        Definition = definition,
                    };
                }
            }
        }

        private Guid? TryGetMatchedTileGuidAt(Vector3Int tilePosition, Guid matchGuid)
        {
            var guid = _terrainTileMap.GetValueOrDefault(tilePosition, Guid.Empty);
            if (guid == Guid.Empty || guid == matchGuid)
            {
                return null;
            }

            return guid;
        }

        private Guid?[,] GetTileGuidSudokuAt(Vector3Int tilePosition)
        {
            var sudoku = new Guid?[3, 3];
            var map = _terrainTileMap;
            var grid = map.GetValueOrDefault(tilePosition, Guid.Empty);

            sudoku[0, 2] = TryGetMatchedTileGuidAt(tilePosition + Vector3Int.up + Vector3Int.left, grid);
            sudoku[1, 2] = TryGetMatchedTileGuidAt(tilePosition + Vector3Int.up, grid);
            sudoku[2, 2] = TryGetMatchedTileGuidAt(tilePosition + Vector3Int.up + Vector3Int.right, grid);

            sudoku[0, 1] = TryGetMatchedTileGuidAt(tilePosition + Vector3Int.left, grid);
            sudoku[1, 1] = grid;
            sudoku[2, 1] = TryGetMatchedTileGuidAt(tilePosition + Vector3Int.right, grid);

            sudoku[0, 0] = TryGetMatchedTileGuidAt(tilePosition + Vector3Int.down + Vector3Int.left, grid);
            sudoku[1, 0] = TryGetMatchedTileGuidAt(tilePosition + Vector3Int.down, grid);
            sudoku[2, 0] = TryGetMatchedTileGuidAt(tilePosition + Vector3Int.down + Vector3Int.right, grid);

            return sudoku;
        }

        public TerrainRuleType CalculateTerrainRuleTypeAt(Vector3Int tilePosition)
        {
            var sudoku = GetTileGuidSudokuAt(tilePosition);

            // Fill
            if (sudoku[0, 2] != null && sudoku[1, 2] != null && sudoku[2, 2] != null &&
                sudoku[0, 1] != null && sudoku[1, 1] != null && sudoku[2, 1] != null &&
                sudoku[0, 0] != null && sudoku[1, 0] != null && sudoku[2, 0] != null)
            {
                return TerrainRuleType.Fill;
            }

            // Edge
            if (sudoku[0, 2] == null && sudoku[1, 2] == null && sudoku[2, 2] == null &&
                sudoku[0, 1] != null && sudoku[1, 1] != null && sudoku[2, 1] != null &&
                sudoku[0, 0] != null && sudoku[1, 0] != null && sudoku[2, 0] != null)
            {
                return TerrainRuleType.TopEdge;
            }

            if (sudoku[0, 2] != null && sudoku[1, 2] != null && sudoku[2, 2] != null &&
                sudoku[0, 1] != null && sudoku[1, 1] != null && sudoku[2, 1] != null &&
                sudoku[0, 0] == null && sudoku[1, 0] == null && sudoku[2, 0] == null)
            {
                return TerrainRuleType.BottomEdge;
            }

            if (sudoku[0, 2] == null && sudoku[1, 2] != null && sudoku[2, 2] != null &&
                sudoku[0, 1] == null && sudoku[1, 1] != null && sudoku[2, 1] != null &&
                sudoku[0, 0] == null && sudoku[1, 0] != null && sudoku[2, 0] != null)
            {
                return TerrainRuleType.LeftEdge;
            }

            if (sudoku[0, 2] != null && sudoku[1, 2] != null && sudoku[2, 2] == null &&
                sudoku[0, 1] != null && sudoku[1, 1] != null && sudoku[2, 1] == null &&
                sudoku[0, 0] != null && sudoku[1, 0] != null && sudoku[2, 0] == null)
            {
                return TerrainRuleType.RightEdge;
            }

            // Exterior corner
            if (sudoku[0, 2] == null && sudoku[1, 2] == null && sudoku[2, 2] == null &&
                sudoku[0, 1] == null && sudoku[1, 1] != null && sudoku[2, 1] != null &&
                sudoku[0, 0] == null && sudoku[1, 0] != null && sudoku[2, 0] != null)
            {
                return TerrainRuleType.TopLeftExteriorCorner;
            }

            if (sudoku[0, 2] == null && sudoku[1, 2] == null && sudoku[2, 2] == null &&
                sudoku[0, 1] != null && sudoku[1, 1] != null && sudoku[2, 1] == null &&
                sudoku[0, 0] != null && sudoku[1, 0] != null && sudoku[2, 0] == null)
            {
                return TerrainRuleType.TopRightExteriorCorner;
            }

            if (sudoku[0, 2] == null && sudoku[1, 2] != null && sudoku[2, 2] != null &&
                sudoku[0, 1] == null && sudoku[1, 1] != null && sudoku[2, 1] != null &&
                sudoku[0, 0] == null && sudoku[1, 0] == null && sudoku[2, 0] == null)
            {
                return TerrainRuleType.BottomLeftExteriorCorner;
            }

            if (sudoku[0, 2] != null && sudoku[1, 2] != null && sudoku[2, 2] == null &&
                sudoku[0, 1] != null && sudoku[1, 1] != null && sudoku[2, 1] == null &&
                sudoku[0, 0] == null && sudoku[1, 0] == null && sudoku[2, 0] == null)
            {
                return TerrainRuleType.BottomRightExteriorCorner;
            }

            // Interior corner
            if (sudoku[0, 2] != null && sudoku[1, 2] != null && sudoku[2, 2] != null &&
                sudoku[0, 1] != null && sudoku[1, 1] != null && sudoku[2, 1] != null &&
                sudoku[0, 0] != null && sudoku[1, 0] != null && sudoku[2, 0] == null)
            {
                return TerrainRuleType.TopLeftInteriorCorner;
            }

            if (sudoku[0, 2] != null && sudoku[1, 2] != null && sudoku[2, 2] != null &&
                sudoku[0, 1] != null && sudoku[1, 1] != null && sudoku[2, 1] != null &&
                sudoku[0, 0] == null && sudoku[1, 0] != null && sudoku[2, 0] != null)
            {
                return TerrainRuleType.TopRightInteriorCorner;
            }

            if (sudoku[0, 2] != null && sudoku[1, 2] != null && sudoku[2, 2] == null &&
                sudoku[0, 1] != null && sudoku[1, 1] != null && sudoku[2, 1] != null &&
                sudoku[0, 0] != null && sudoku[1, 0] != null && sudoku[2, 0] != null)
            {
                return TerrainRuleType.BottomLeftInteriorCorner;
            }

            if (sudoku[0, 2] == null && sudoku[1, 2] != null && sudoku[2, 2] != null &&
                sudoku[0, 1] != null && sudoku[1, 1] != null && sudoku[2, 1] != null &&
                sudoku[0, 0] != null && sudoku[1, 0] != null && sudoku[2, 0] != null)
            {
                return TerrainRuleType.BottomRightInteriorCorner;
            }

            return TerrainRuleType.Fill;
        }

        public bool RemoveTerrainTileAt(Vector3Int tilePosition)
        {
            return _terrainTileMap.Remove(tilePosition);
        }

        public void ClearTerrainTileMap(Guid targetTerrainTileDefinitionGuid)
        {
            var tilePositionsToRemove = _terrainTileMap
                .Where(kvp => kvp.Value == targetTerrainTileDefinitionGuid)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var tilePosition in tilePositionsToRemove)
            {
                _terrainTileMap.Remove(tilePosition);
            }
        }

#if UNITY_EDITOR
        private void OnRemovedTerrainTileDefinition(object weakTerrainTileDefinition)
        {
            var terrainTileDefinition = weakTerrainTileDefinition as TerrainTileDefinition;
            if (terrainTileDefinition == null)
            {
                return;
            }

            ClearTerrainTileMap(terrainTileDefinition.Guid);
        }
#endif
    }
}
