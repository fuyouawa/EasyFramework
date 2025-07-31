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
        [InlineEditor(Style = InlineEditorStyle.PlaceWithHide)]
        [SerializeField] private TerrainTileDefinitionsAsset _definitionsAsset;
        
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

        public bool ClearInvalidTiles()
        {
            var invalidTilePositions = _definitionGuidMap
                .Where(kvp => !DefinitionsAsset.Contains(kvp.Value))
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var tilePosition in invalidTilePositions)
            {
                RemoveTileAt(tilePosition);
            }

            Debug.Log($"Clear invalid tiles: {invalidTilePositions.Count}");
            return invalidTilePositions.Count > 0;
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

        public TerrainRuleType CalculateRuleTypeAt(Vector3Int tilePosition)
        {
            var sudoku = GetDefinitionGuidSudokuAt(tilePosition);
            var booleanSudoku = ToBooleanDefinitionSudoku(sudoku);
            return TerrainTileConfigAsset.Instance.GetRuleTypeBySudoku(booleanSudoku);
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
