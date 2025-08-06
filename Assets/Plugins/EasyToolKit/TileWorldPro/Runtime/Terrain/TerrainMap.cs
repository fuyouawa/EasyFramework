// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using EasyToolKit.Core;
// using EasyToolKit.Inspector;
// using EasyToolKit.ThirdParty.OdinSerializer;
// using UnityEngine;

// namespace EasyToolKit.TileWorldPro
// {
//     [Serializable]
//     public class TerrainMap : ISerializationCallbackReceiver, IEnumerable<TerrainTilePosition>
//     {
//         public struct GridInfo : IEquatable<GridInfo>
//         {
//             public readonly Guid DefinitionGuid;
//             public bool IsVirtual;

//             public GridInfo(Guid definitionGuid, bool isVirtual = false)
//             {
//                 DefinitionGuid = definitionGuid;
//                 IsVirtual = isVirtual;
//             }

//             public readonly bool Equals(GridInfo other)
//             {
//                 return DefinitionGuid == other.DefinitionGuid && IsVirtual == other.IsVirtual;
//             }

//             public override readonly bool Equals(object obj)
//             {
//                 return obj is GridInfo other && Equals(other);
//             }

//             public override readonly int GetHashCode() => DefinitionGuid.GetHashCode();
//             public override readonly string ToString() => $"{DefinitionGuid:D}{(IsVirtual ? ", Virtual" : "")}";

//             public static bool operator ==(GridInfo left, GridInfo right) => left.Equals(right);
//             public static bool operator !=(GridInfo left, GridInfo right) => !left.Equals(right);
//         }

//         [LabelText("地形瓦片定义表")]
//         [HideLabel]
//         [SerializeField] private TerrainDefinitionSet _definitionSet;

//         private Dictionary<Vector3Int, GridInfo> _gridMap = new Dictionary<Vector3Int, GridInfo>();

//         [SerializeField, HideInInspector] private byte[] _serializedDefinitionGuidMap;

//         public TerrainDefinitionSet DefinitionSet => _definitionSet;

//         public void SetTileAt(Vector3Int tilePosition, GridInfo gridInfo)
//         {
//             _gridMap[tilePosition] = gridInfo;
//         }

//         public TerrainDefinition TryGetDefinitionAt(Vector3Int tilePosition)
//         {
//             if (_gridMap.TryGetValue(tilePosition, out var gridInfo))
//             {
//                 return DefinitionSet.TryGetByGuid(gridInfo.DefinitionGuid);
//             }

//             return null;
//         }

//         public bool ClearInvalidTiles()
//         {
//             var invalidTilePositions = _gridMap
//                 .Where(kvp => !DefinitionSet.Contains(kvp.Value.DefinitionGuid))
//                 .Select(kvp => kvp.Key)
//                 .ToList();

//             foreach (var tilePosition in invalidTilePositions)
//             {
//                 RemoveTileAt(tilePosition);
//             }

//             Debug.Log($"Clear invalid tiles: {invalidTilePositions.Count}");
//             return invalidTilePositions.Count > 0;
//         }

//         public IEnumerable<TerrainTilePosition> EnumerateMatchedMap(Guid matchDefinitionGuid)
//         {
//             var definition = DefinitionSet.TryGetByGuid(matchDefinitionGuid);
//             Assert.IsNotNull(definition);

//             foreach (var kvp in _gridMap)
//             {
//                 var tilePosition = kvp.Key;
//                 var gridInfo = kvp.Value;
//                 Assert.IsTrue(gridInfo.DefinitionGuid != Guid.Empty);

//                 if (gridInfo.DefinitionGuid == matchDefinitionGuid)
//                 {
//                     yield return new TerrainTilePosition
//                     {
//                         TilePosition = tilePosition,
//                         Definition = definition,
//                     };
//                 }
//             }
//         }

//         public IEnumerable<TerrainTilePosition> EnumerateNearlyMatchedMap(Guid matchDefinitionGuid,
//             Vector3Int tilePosition, int maxDepth = int.MaxValue)
//         {
//             var definition = DefinitionSet.TryGetByGuid(matchDefinitionGuid);
//             Assert.IsNotNull(definition);

//             // Use HashSet to track visited positions to avoid duplicates
//             var visited = new HashSet<Vector3Int>();
//             // Use Queue for breadth-first search with tuple of position and depth
//             var queue = new Queue<(Vector3Int position, int depth)>();

//             // Directions to check (only x and z axes, y stays fixed)
//             var directions = new[]
//             {
//                 new Vector3Int(1, 0, 0), // Right
//                 new Vector3Int(-1, 0, 0), // Left
//                 new Vector3Int(0, 0, 1), // Forward
//                 new Vector3Int(0, 0, -1), // Back
//             };

//             // Special handling for initial position
//             visited.Add(tilePosition);

//             // Check if the starting position has the matching definition
//             bool hasStartingTile = _gridMap.TryGetValue(tilePosition, out var startGridInfo) &&
//                                   startGridInfo.DefinitionGuid == matchDefinitionGuid;

//             // If starting position has the matching tile, add it to results and queue
//             if (hasStartingTile)
//             {
//                 yield return new TerrainTilePosition
//                 {
//                     TilePosition = tilePosition,
//                     Definition = definition,
//                 };

//                 queue.Enqueue((tilePosition, 0));
//             }
//             else
//             {
//                 // If starting position doesn't have matching tile, directly check surrounding tiles
//                 foreach (var dir in directions)
//                 {
//                     var neighborPos = tilePosition + dir;

//                     // Skip already visited positions
//                     if (visited.Contains(neighborPos))
//                         continue;

//                     // Mark as visited
//                     visited.Add(neighborPos);

//                     // Check if the neighbor has the matching definition
//                     if (_gridMap.TryGetValue(neighborPos, out var neighborGridInfo) &&
//                         neighborGridInfo.DefinitionGuid == matchDefinitionGuid)
//                     {
//                         // Add to results
//                         yield return new TerrainTilePosition
//                         {
//                             TilePosition = neighborPos,
//                             Definition = definition,
//                         };

//                         // Add to queue for further exploration
//                         queue.Enqueue((neighborPos, 1));
//                     }
//                 }
//             }

//             while (queue.Count > 0)
//             {
//                 var (currentPos, currentDepth) = queue.Dequeue();

//                 // Stop expanding if we've reached max depth
//                 if (currentDepth >= maxDepth)
//                     continue;

//                 // Check adjacent tiles in the four directions
//                 foreach (var dir in directions)
//                 {
//                     var neighborPos = currentPos + dir;

//                     // Skip already visited positions
//                     if (visited.Contains(neighborPos))
//                         continue;

//                     // Mark as visited
//                     visited.Add(neighborPos);

//                     // Only add to queue if it has the matching definition
//                     if (_gridMap.TryGetValue(neighborPos, out var neighborGridInfo) &&
//                         neighborGridInfo.DefinitionGuid == matchDefinitionGuid)
//                     {
//                         yield return new TerrainTilePosition
//                         {
//                             TilePosition = neighborPos,
//                             Definition = definition,
//                         };

//                         queue.Enqueue((neighborPos, currentDepth + 1));
//                     }
//                 }
//             }
//         }

//         private GridInfo? TryGetMatchedGridAt(Vector3Int tilePosition, Guid matchDefinitionGuid)
//         {
//             var gridInfo = _gridMap.GetValueOrDefault(tilePosition);
//             if (gridInfo == null || gridInfo.DefinitionGuid != matchDefinitionGuid)
//             {
//                 return null;
//             }

//             return gridInfo;
//         }

//         private GridInfo?[,] GetDefinitionGuidSudokuAt(Vector3Int tilePosition)
//         {
//             var sudoku = new GridInfo?[3, 3];
//             var map = _gridMap;
//             var gridInfo = map.GetValueOrDefault(tilePosition);

//             sudoku[0, 2] = TryGetMatchedGridAt(tilePosition + Vector3Int.forward + Vector3Int.left, gridInfo.DefinitionGuid);
//             sudoku[1, 2] = TryGetMatchedGridAt(tilePosition + Vector3Int.forward, gridInfo.DefinitionGuid);
//             sudoku[2, 2] = TryGetMatchedGridAt(tilePosition + Vector3Int.forward + Vector3Int.right, gridInfo.DefinitionGuid);

//             sudoku[0, 1] = TryGetMatchedGridAt(tilePosition + Vector3Int.left, gridInfo.DefinitionGuid);
//             sudoku[1, 1] = gridInfo;
//             sudoku[2, 1] = TryGetMatchedGridAt(tilePosition + Vector3Int.right, gridInfo.DefinitionGuid);

//             sudoku[0, 0] = TryGetMatchedGridAt(tilePosition + Vector3Int.back + Vector3Int.left, gridInfo.DefinitionGuid);
//             sudoku[1, 0] = TryGetMatchedGridAt(tilePosition + Vector3Int.back, gridInfo.DefinitionGuid);
//             sudoku[2, 0] = TryGetMatchedGridAt(tilePosition + Vector3Int.back + Vector3Int.right, gridInfo.DefinitionGuid);

//             return sudoku;
//         }

//         private bool[,] ToBooleanDefinitionSudoku(GridInfo?[,] sudoku)
//         {
//             var booleanSudoku = new bool[3, 3];
//             for (int i = 0; i < 3; i++)
//             {
//                 for (int j = 0; j < 3; j++)
//                 {
//                     booleanSudoku[i, j] = sudoku[i, j] != null;
//                 }
//             }

//             return booleanSudoku;
//         }

//         public TerrainTileRuleType CalculateRuleTypeAt(TerrainConfigAsset terrainConfigAsset, Vector3Int tilePosition)
//         {
//             var sudoku = GetDefinitionGuidSudokuAt(tilePosition);
//             var booleanSudoku = ToBooleanDefinitionSudoku(sudoku);
//             return terrainConfigAsset.GetRuleTypeBySudoku(booleanSudoku);
//         }

//         public bool RemoveTileAt(Vector3Int tilePosition)
//         {
//             return _gridMap.Remove(tilePosition);
//         }

//         public bool ClearMatchedMap(Guid matchDefinitionGuid)
//         {
//             var tilePositionsToRemove = _gridMap
//                 .Where(kvp => kvp.Value.DefinitionGuid == matchDefinitionGuid)
//                 .Select(kvp => kvp.Key)
//                 .ToList();

//             foreach (var tilePosition in tilePositionsToRemove)
//             {
//                 _gridMap.Remove(tilePosition);
//             }

//             return tilePositionsToRemove.Count > 0;
//         }

//         public IEnumerator<TerrainTilePosition> GetEnumerator()
//         {
//             foreach (var kvp in _gridMap)
//             {
//                 var tilePosition = kvp.Key;
//                 var gridInfo = kvp.Value;
//                 var definition = DefinitionSet.TryGetByGuid(gridInfo.DefinitionGuid);
//                 Assert.IsNotNull(definition);

//                 yield return new TerrainTilePosition
//                 {
//                     TilePosition = tilePosition,
//                     Definition = definition,
//                 };
//             }
//         }

//         IEnumerator IEnumerable.GetEnumerator()
//         {
//             return GetEnumerator();
//         }

//         void ISerializationCallbackReceiver.OnBeforeSerialize()
//         {
//             _gridMap ??= new Dictionary<Vector3Int, GridInfo>();

//             if (_gridMap.Count == 0)
//             {
//                 _serializedDefinitionGuidMap = null;
//                 return;
//             }

//             _serializedDefinitionGuidMap = SerializationUtility.SerializeValue(_gridMap, DataFormat.Binary);
//         }

//         void ISerializationCallbackReceiver.OnAfterDeserialize()
//         {
//             if (_serializedDefinitionGuidMap == null || _serializedDefinitionGuidMap.Length == 0)
//             {
//                 _gridMap = new Dictionary<Vector3Int, GridInfo>();
//                 return;
//             }

//             _gridMap = SerializationUtility.DeserializeValue<Dictionary<Vector3Int, GridInfo>>(
//                 _serializedDefinitionGuidMap,
//                 DataFormat.Binary);
//         }
//     }
// }
