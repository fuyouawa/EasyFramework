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
    [Serializable]
    public class TerrainMap : ISerializationCallbackReceiver, IEnumerable<TerrainTilePosition>
    {
        [LabelText("地形瓦片定义表资产")]
        [HideLabel]
        [SerializeField] private TerrainDefinitionSet _definitionSet;

        private Dictionary<Vector3Int, Guid> _definitionGuidMap = new Dictionary<Vector3Int, Guid>();

        [SerializeField, HideInInspector] private byte[] _serializedDefinitionGuidMap;

        public TerrainDefinitionSet DefinitionSet => _definitionSet;

        public void SetTileAt(Vector3Int tilePosition, Guid terrainDefinitionGuid)
        {
            _definitionGuidMap[tilePosition] = terrainDefinitionGuid;
        }

        public TerrainDefinition TryGetDefinitionAt(Vector3Int tilePosition)
        {
            if (_definitionGuidMap.TryGetValue(tilePosition, out var guid))
            {
                return DefinitionSet.TryGetByGuid(guid);
            }

            return null;
        }

        public bool ClearInvalidTiles()
        {
            var invalidTilePositions = _definitionGuidMap
                .Where(kvp => !DefinitionSet.Contains(kvp.Value))
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var tilePosition in invalidTilePositions)
            {
                RemoveTileAt(tilePosition);
            }

            Debug.Log($"Clear invalid tiles: {invalidTilePositions.Count}");
            return invalidTilePositions.Count > 0;
        }

        public IEnumerable<TerrainTilePosition> EnumerateMatchedMap(Guid matchDefinitionGuid)
        {
            var definition = DefinitionSet.TryGetByGuid(matchDefinitionGuid);
            Assert.IsNotNull(definition);

            foreach (var kvp in _definitionGuidMap)
            {
                var tilePosition = kvp.Key;
                var guid = kvp.Value;
                Assert.IsTrue(guid != Guid.Empty);

                if (guid == matchDefinitionGuid)
                {
                    yield return new TerrainTilePosition
                    {
                        TilePosition = tilePosition,
                        Definition = definition,
                    };
                }
            }
        }

        public IEnumerable<TerrainTilePosition> EnumerateNearlyMatchedMap(Guid matchDefinitionGuid,
            Vector3Int tilePosition, int maxDepth = int.MaxValue)
        {
            var definition = DefinitionSet.TryGetByGuid(matchDefinitionGuid);
            Assert.IsNotNull(definition);

            // Use HashSet to track visited positions to avoid duplicates
            var visited = new HashSet<Vector3Int>();
            // Use Queue for breadth-first search with tuple of position and depth
            var queue = new Queue<(Vector3Int position, int depth)>();

            // Directions to check (only x and z axes, y stays fixed)
            var directions = new[]
            {
                new Vector3Int(1, 0, 0), // Right
                new Vector3Int(-1, 0, 0), // Left
                new Vector3Int(0, 0, 1), // Forward
                new Vector3Int(0, 0, -1), // Back
            };

            // Special handling for initial position
            visited.Add(tilePosition);

            // Check if the starting position has the matching definition
            bool hasStartingTile = _definitionGuidMap.TryGetValue(tilePosition, out var startGuid) &&
                                  startGuid == matchDefinitionGuid;

            // If starting position has the matching tile, add it to results and queue
            if (hasStartingTile)
            {
                yield return new TerrainTilePosition
                {
                    TilePosition = tilePosition,
                    Definition = definition,
                };

                queue.Enqueue((tilePosition, 0));
            }
            else
            {
                // If starting position doesn't have matching tile, directly check surrounding tiles
                foreach (var dir in directions)
                {
                    var neighborPos = tilePosition + dir;

                    // Skip already visited positions
                    if (visited.Contains(neighborPos))
                        continue;

                    // Mark as visited
                    visited.Add(neighborPos);

                    // Check if the neighbor has the matching definition
                    if (_definitionGuidMap.TryGetValue(neighborPos, out var neighborGuid) &&
                        neighborGuid == matchDefinitionGuid)
                    {
                        // Add to results
                        yield return new TerrainTilePosition
                        {
                            TilePosition = neighborPos,
                            Definition = definition,
                        };

                        // Add to queue for further exploration
                        queue.Enqueue((neighborPos, 1));
                    }
                }
            }

            while (queue.Count > 0)
            {
                var (currentPos, currentDepth) = queue.Dequeue();

                // Stop expanding if we've reached max depth
                if (currentDepth >= maxDepth)
                    continue;

                // Check adjacent tiles in the four directions
                foreach (var dir in directions)
                {
                    var neighborPos = currentPos + dir;

                    // Skip already visited positions
                    if (visited.Contains(neighborPos))
                        continue;

                    // Mark as visited
                    visited.Add(neighborPos);

                    // Only add to queue if it has the matching definition
                    if (_definitionGuidMap.TryGetValue(neighborPos, out var neighborGuid) &&
                        neighborGuid == matchDefinitionGuid)
                    {
                        yield return new TerrainTilePosition
                        {
                            TilePosition = neighborPos,
                            Definition = definition,
                        };

                        queue.Enqueue((neighborPos, currentDepth + 1));
                    }
                }
            }
        }

        private Guid? TryGetMatchedDefinitionGuidAt(Vector3Int tilePosition, Guid matchDefinitionGuid)
        {
            var guid = _definitionGuidMap.GetValueOrDefault(tilePosition, Guid.Empty);
            if (guid == Guid.Empty || guid != matchDefinitionGuid)
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

            sudoku[0, 2] = TryGetMatchedDefinitionGuidAt(tilePosition + Vector3Int.forward + Vector3Int.left, grid);
            sudoku[1, 2] = TryGetMatchedDefinitionGuidAt(tilePosition + Vector3Int.forward, grid);
            sudoku[2, 2] = TryGetMatchedDefinitionGuidAt(tilePosition + Vector3Int.forward + Vector3Int.right, grid);

            sudoku[0, 1] = TryGetMatchedDefinitionGuidAt(tilePosition + Vector3Int.left, grid);
            sudoku[1, 1] = grid;
            sudoku[2, 1] = TryGetMatchedDefinitionGuidAt(tilePosition + Vector3Int.right, grid);

            sudoku[0, 0] = TryGetMatchedDefinitionGuidAt(tilePosition + Vector3Int.back + Vector3Int.left, grid);
            sudoku[1, 0] = TryGetMatchedDefinitionGuidAt(tilePosition + Vector3Int.back, grid);
            sudoku[2, 0] = TryGetMatchedDefinitionGuidAt(tilePosition + Vector3Int.back + Vector3Int.right, grid);

            return sudoku;
        }

        private bool[,] ToBooleanDefinitionSudoku(Guid?[,] sudoku)
        {
            var booleanSudoku = new bool[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    booleanSudoku[i, j] = sudoku[i, j] != null;
                }
            }

            return booleanSudoku;
        }

        public TerrainTileRuleType CalculateRuleTypeAt(Vector3Int tilePosition)
        {
            var sudoku = GetDefinitionGuidSudokuAt(tilePosition);
            var booleanSudoku = ToBooleanDefinitionSudoku(sudoku);
            return TerrainConfigAsset.Instance.GetRuleTypeBySudoku(booleanSudoku);
        }

        public bool RemoveTileAt(Vector3Int tilePosition)
        {
            return _definitionGuidMap.Remove(tilePosition);
        }

        public bool ClearMatchedMap(Guid matchDefinitionGuid)
        {
            var tilePositionsToRemove = _definitionGuidMap
                .Where(kvp => kvp.Value == matchDefinitionGuid)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var tilePosition in tilePositionsToRemove)
            {
                _definitionGuidMap.Remove(tilePosition);
            }

            return tilePositionsToRemove.Count > 0;
        }

        public IEnumerator<TerrainTilePosition> GetEnumerator()
        {
            foreach (var kvp in _definitionGuidMap)
            {
                var tilePosition = kvp.Key;
                var guid = kvp.Value;
                var definition = DefinitionSet.TryGetByGuid(guid);
                Assert.IsNotNull(definition);
                Assert.IsTrue(guid != Guid.Empty);

                yield return new TerrainTilePosition
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

            _definitionGuidMap = SerializationUtility.DeserializeValue<Dictionary<Vector3Int, Guid>>(
                _serializedDefinitionGuidMap,
                DataFormat.Binary);
        }
    }
}
