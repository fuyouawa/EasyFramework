using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using EasyToolKit.Inspector;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    public struct TerrainTileBlockDefinition
    {
        public Vector3Int TilePosition;
        public TerrainTileDefinition Definition;
    }

    [Serializable]
    public class TerrainTileMap : ISerializationCallbackReceiver, IEnumerable<TerrainTileBlockDefinition>
    {
        [LabelText("地形瓦片定义表资产")]
        [SerializeField, InlineEditor] private TerrainTileDefinitionsAsset _definitionsAsset;
        
        private Dictionary<Vector3Int, Guid> _definitionGuidMap = new Dictionary<Vector3Int, Guid>();

        [SerializeField, HideInInspector] private byte[] _serializedDefinitionGuidMap;

        public TerrainTileDefinitionsAsset DefinitionsAsset => _definitionsAsset;

        public void SetTileAt(Vector3Int tilePosition, Guid terrainTileDefinitionGuid)
        {
            _definitionGuidMap[tilePosition] = terrainTileDefinitionGuid;
        }

        public TerrainTileDefinition TryGetDefinitionAt(Vector3Int tilePosition)
        {
            if (_definitionGuidMap.TryGetValue(tilePosition, out var guid))
            {
                return DefinitionsAsset.TryGetByGuid(guid);
            }

            return null;
        }

        public void ClearInvalidTiles()
        {
            var invalidTilePositions = _definitionGuidMap
                .Where(kvp => !DefinitionsAsset.Definitions.Any(definition => definition.Guid == kvp.Value))
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var tilePosition in invalidTilePositions)
            {
                RemoveTileAt(tilePosition);
            }
        }

        public IEnumerable<TerrainTileBlockDefinition> EnumerateMatchedMap(Guid matchDefinitionGuid)
        {
            var definition = DefinitionsAsset.TryGetByGuid(matchDefinitionGuid);
            Assert.IsNotNull(definition);

            foreach (var kvp in _definitionGuidMap)
            {
                var tilePosition = kvp.Key;
                var guid = kvp.Value;
                Assert.IsTrue(guid != Guid.Empty);

                if (guid == matchDefinitionGuid)
                {
                    yield return new TerrainTileBlockDefinition
                    {
                        TilePosition = tilePosition,
                        Definition = definition,
                    };
                }
            }
        }

        private Guid? TryGetMatchedDefinitionGuidAt(Vector3Int tilePosition, Guid matchDefinitionGuid)
        {
            var guid = _definitionGuidMap.GetValueOrDefault(tilePosition, Guid.Empty);
            if (guid == Guid.Empty || guid == matchDefinitionGuid)
            {
                return null;
            }

            return guid;
        }

        private Guid?[,] GetDefinitionGuidSudokuAt(Vector3Int tilePosition)
        {
            var sudoku = new Guid?[3, 3];
            var map = _definitionGuidMap;
            var grid = map.GetValueOrDefault(tilePosition, Guid.Empty);

            sudoku[0, 2] = TryGetMatchedDefinitionGuidAt(tilePosition + Vector3Int.up + Vector3Int.left, grid);
            sudoku[1, 2] = TryGetMatchedDefinitionGuidAt(tilePosition + Vector3Int.up, grid);
            sudoku[2, 2] = TryGetMatchedDefinitionGuidAt(tilePosition + Vector3Int.up + Vector3Int.right, grid);

            sudoku[0, 1] = TryGetMatchedDefinitionGuidAt(tilePosition + Vector3Int.left, grid);
            sudoku[1, 1] = grid;
            sudoku[2, 1] = TryGetMatchedDefinitionGuidAt(tilePosition + Vector3Int.right, grid);

            sudoku[0, 0] = TryGetMatchedDefinitionGuidAt(tilePosition + Vector3Int.down + Vector3Int.left, grid);
            sudoku[1, 0] = TryGetMatchedDefinitionGuidAt(tilePosition + Vector3Int.down, grid);
            sudoku[2, 0] = TryGetMatchedDefinitionGuidAt(tilePosition + Vector3Int.down + Vector3Int.right, grid);

            return sudoku;
        }

        public TerrainRuleType CalculateRuleTypeAt(Vector3Int tilePosition)
        {
            var sudoku = GetDefinitionGuidSudokuAt(tilePosition);

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

        public bool RemoveTileAt(Vector3Int tilePosition)
        {
            return _definitionGuidMap.Remove(tilePosition);
        }

        public void ClearMatchedMap(Guid matchDefinitionGuid)
        {
            var tilePositionsToRemove = _definitionGuidMap
                .Where(kvp => kvp.Value == matchDefinitionGuid)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var tilePosition in tilePositionsToRemove)
            {
                _definitionGuidMap.Remove(tilePosition);
            }
        }

        public IEnumerator<TerrainTileBlockDefinition> GetEnumerator()
        {
            foreach (var kvp in _definitionGuidMap)
            {
                var tilePosition = kvp.Key;
                var guid = kvp.Value;
                var definition = DefinitionsAsset.TryGetByGuid(guid);
                Assert.IsNotNull(definition);
                Assert.IsTrue(guid != Guid.Empty);

                yield return new TerrainTileBlockDefinition
                {
                    TilePosition = tilePosition,
                    Definition = definition,
                };
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _definitionGuidMap ??= new Dictionary<Vector3Int, Guid>();

            if (_definitionGuidMap.Count == 0)
            {
                _serializedDefinitionGuidMap = null;
                return;
            }

            _serializedDefinitionGuidMap = SerializationUtility.SerializeValue(_definitionGuidMap, DataFormat.Binary);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (_serializedDefinitionGuidMap == null || _serializedDefinitionGuidMap.Length == 0)
            {
                _definitionGuidMap = new Dictionary<Vector3Int, Guid>();
                return;
            }

            _definitionGuidMap = SerializationUtility.DeserializeValue<Dictionary<Vector3Int, Guid>>(_serializedDefinitionGuidMap, DataFormat.Binary);
        }
    }
}