using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using EasyToolKit.Inspector;
using EasyToolKit.ThirdParty.OdinSerializer;
using EasyToolKit.TileWorldPro;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [CreateAssetMenu(menuName = "EasyToolKit/TileWorldPro/TileWorld", fileName = "TileWorld")]
    public class TileWorldAsset : SerializedScriptableObject
    {
        [LabelText("瓦片大小")]
        [SerializeField] private float _tileSize = 1f;

        [LabelText("世界区块大小")]
        [ReadOnly]
        [SerializeField] private Vector2Int _chunkSize;

        [HideLabel]
        [SerializeField] private TerrainDefinitionSet _terrainDefinitionSet;

        [OdinSerialize, ShowInInspector] private ITileWorldDataStore _dataStore;

        [SerializeField, HideInInspector] private bool _isInitialized = false;

        public float TileSize => _tileSize;
        public Vector2Int ChunkSize
        {
            get
            {
                EnsureInitialized();
                return _chunkSize;
            }
        }
        public TerrainDefinitionSet TerrainDefinitionSet => _terrainDefinitionSet;
        public ITileWorldDataStore DataStore
        {
            get => _dataStore;
            set => _dataStore = value;
        }

        public IEnumerable<Chunk> EnumerateChunks()
        {
            if (_dataStore == null || !_dataStore.IsValid)
            {
                return Enumerable.Empty<Chunk>();
            }

            return _dataStore.EnumerateChunks();
        }

        public Chunk GetChunkAt(TilePosition tilePosition)
        {
            if (_dataStore == null || !_dataStore.IsValid)
            {
                throw new InvalidOperationException("The data store is not valid");
            }

            EnsureInitialized();
            var chunkIndex = tilePosition.ToChunkPosition(_chunkSize);
            var chunk = _dataStore.TryGetChunk(chunkIndex);
            if (chunk != null)
            {
                return chunk;
            }

            var area = new ChunkArea(chunkIndex, _chunkSize);
            chunk = new Chunk(area);
            _dataStore.UpdateChunk(chunk);
            return chunk;
        }

        public ChunkTilePosition TilePositionToChunkTilePosition(TilePosition tilePosition)
        {
            EnsureInitialized();
            return new ChunkArea(tilePosition.ToChunkPosition(_chunkSize), _chunkSize)
                .TilePositionToChunkTilePosition(tilePosition);
        }

        public Guid? TryGetTerrainGuidAt(TilePosition tilePosition)
        {
            return GetChunkAt(tilePosition).TryGetTerrainGuidAt(tilePosition);
        }

        public void SetTilesAt(IReadOnlyList<TilePosition> tilePositions, Guid terrainGuid)
        {
            if (_dataStore == null || !_dataStore.IsValid)
            {
                throw new InvalidOperationException("The data store is not valid");
            }

            var affectedChunks = new List<Chunk>();
            Chunk chunkCache = null;
            var tilesCache = new ChunkTilePosition[1];
            foreach (var tilePosition in tilePositions)
            {
                if (chunkCache == null || !chunkCache.Area.Contains(tilePosition))
                {
                    chunkCache = GetChunkAt(tilePosition);
                    affectedChunks.Add(chunkCache);
                }

                tilesCache[0] = TilePositionToChunkTilePosition(tilePosition);
                chunkCache.SetTilesAt(tilesCache, terrainGuid);
            }

            _dataStore.UpdateChunkRange(affectedChunks);
        }

        public void RemoveTilesAt(IReadOnlyList<TilePosition> tilePositions, Guid terrainGuid)
        {
            if (_dataStore == null || !_dataStore.IsValid)
            {
                throw new InvalidOperationException("The data store is not valid");
            }

            var affectedChunks = new List<Chunk>();
            Chunk chunkCache = null;
            var tilesCache = new ChunkTilePosition[1];
            foreach (var tilePosition in tilePositions)
            {
                if (chunkCache == null || !chunkCache.Area.Contains(tilePosition))
                {
                    chunkCache = GetChunkAt(tilePosition);
                    affectedChunks.Add(chunkCache);
                }

                tilesCache[0] = TilePositionToChunkTilePosition(tilePosition);
                chunkCache.RemoveTilesAt(tilesCache, terrainGuid);
            }

            _dataStore.UpdateChunkRange(affectedChunks);
        }

        public void SetTileAt(TilePosition tilePosition, Guid terrainGuid)
        {
            GetChunkAt(tilePosition).SetTilesAt(new[] { TilePositionToChunkTilePosition(tilePosition) }, terrainGuid);
        }

        public void RemoveTileAt(TilePosition tilePosition, Guid terrainGuid)
        {
            GetChunkAt(tilePosition).RemoveTilesAt(new[] { TilePositionToChunkTilePosition(tilePosition) }, terrainGuid);
        }

        private bool[,] GetSudokuAt(TilePosition tilePosition)
        {
            var sudoku = new bool[3, 3];
            var terrainGuid = TryGetTerrainGuidAt(tilePosition);
            Assert.IsNotNull(terrainGuid);

            var position = tilePosition + TilePosition.ForwardLeft;
            if (IsValidTilePosition(position))
                sudoku[0, 2] = TryGetTerrainGuidAt(tilePosition + TilePosition.ForwardLeft) == terrainGuid;
            position = tilePosition + TilePosition.Forward;
            if (IsValidTilePosition(position))
                sudoku[1, 2] = TryGetTerrainGuidAt(position) == terrainGuid;
            position = tilePosition + TilePosition.ForwardRight;
            if (IsValidTilePosition(position))
                sudoku[2, 2] = TryGetTerrainGuidAt(position) == terrainGuid;

            position = tilePosition + TilePosition.Left;
            if (IsValidTilePosition(position))
                sudoku[0, 1] = TryGetTerrainGuidAt(position) == terrainGuid;
            sudoku[1, 1] = true;
            position = tilePosition + TilePosition.Right;
            if (IsValidTilePosition(position))
                sudoku[2, 1] = TryGetTerrainGuidAt(position) == terrainGuid;

            position = tilePosition + TilePosition.BackLeft;
            if (IsValidTilePosition(position))
                sudoku[0, 0] = TryGetTerrainGuidAt(position) == terrainGuid;
            position = tilePosition + TilePosition.Back;
            if (IsValidTilePosition(position))
                sudoku[1, 0] = TryGetTerrainGuidAt(position) == terrainGuid;
            position = tilePosition + TilePosition.BackRight;
            if (IsValidTilePosition(position))
                sudoku[2, 0] = TryGetTerrainGuidAt(position) == terrainGuid;

            return sudoku;
        }

        public TerrainTileRuleType CalculateRuleTypeOf(TerrainConfigAsset terrainConfigAsset, TilePosition tilePosition)
        {
            var sudoku = GetSudokuAt(tilePosition);
            return terrainConfigAsset.GetRuleTypeBySudoku(sudoku);
        }

        private bool IsValidTilePosition(TilePosition tilePosition)
        {
            if (tilePosition.X < 0 ||
                tilePosition.Y < 0 ||
                tilePosition.Z < 0)
            {
                return false;
            }
            return true;
        }

        private void EnsureInitialized()
        {
            if (_isInitialized)
            {
                return;
            }

            _chunkSize = TileWorldConfigAsset.Instance.ChunkSize;
            _dataStore = TileWorldDataStoreUtility.GetDefaultDataStore();
            _isInitialized = true;
        }
    }
}