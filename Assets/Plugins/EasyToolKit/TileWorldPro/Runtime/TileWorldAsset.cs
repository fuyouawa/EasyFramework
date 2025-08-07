using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Inspector;
using EasyToolKit.TileWorldPro;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [EasyInspector]
    [CreateAssetMenu(menuName = "EasyToolKit/TileWorldPro/TileWorld", fileName = "TileWorld")]
    public class TileWorldAsset : ScriptableObject
    {
        [LabelText("瓦片大小")]
        [SerializeField] private float _tileSize = 1f;

        [LabelText("世界区块大小")]
        [ReadOnly]
        [SerializeField] private Vector2Int _chunkSize;

        [HideLabel]
        [SerializeField]
        private TerrainDefinitionSet _terrainDefinitionSet;

        private readonly Dictionary<ChunkPosition, Chunk> _chunks = new Dictionary<ChunkPosition, Chunk>();

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

        public IEnumerable<Chunk> EnumerateChunks()
        {
            return _chunks.Values;
        }

        public Chunk GetChunkAt(TilePosition tilePosition)
        {
            EnsureInitialized();
            var chunkIndex = tilePosition.ToChunkPosition(_chunkSize);
            if (_chunks.TryGetValue(chunkIndex, out var chunk))
            {
                return chunk;
            }

            var area = new ChunkArea(chunkIndex, _chunkSize);
            chunk = new Chunk(area);
            _chunks[chunkIndex] = chunk;
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

        public void SetTilesAt(IEnumerable<TilePosition> tilePositions, Guid terrainGuid)
        {
            Chunk chunkCache = null;
            var tilesCache = new ChunkTilePosition[1];
            foreach (var tilePosition in tilePositions)
            {
                if (chunkCache == null || !chunkCache.Area.Contains(tilePosition))
                {
                    chunkCache = GetChunkAt(tilePosition);
                }

                tilesCache[0] = TilePositionToChunkTilePosition(tilePosition);
                chunkCache.SetTilesAt(tilesCache, terrainGuid);
            }
        }

        public void RemoveTilesAt(IEnumerable<TilePosition> tilePositions, Guid terrainGuid)
        {
            Chunk chunkCache = null;
            var tilesCache = new ChunkTilePosition[1];
            foreach (var tilePosition in tilePositions)
            {
                if (chunkCache == null || !chunkCache.Area.Contains(tilePosition))
                {
                    chunkCache = GetChunkAt(tilePosition);
                }

                tilesCache[0] = TilePositionToChunkTilePosition(tilePosition);
                chunkCache.RemoveTilesAt(tilesCache, terrainGuid);
            }
        }

        public void SetTileAt(TilePosition tilePosition, Guid terrainGuid)
        {
            GetChunkAt(tilePosition).SetTilesAt(new[] { TilePositionToChunkTilePosition(tilePosition) }, terrainGuid);
        }

        public void RemoveTileAt(TilePosition tilePosition, Guid terrainGuid)
        {
            GetChunkAt(tilePosition).RemoveTilesAt(new[] { TilePositionToChunkTilePosition(tilePosition) }, terrainGuid);
        }

        private void EnsureInitialized()
        {
            if (_isInitialized)
            {
                return;
            }

            _chunkSize = TileWorldConfigAsset.Instance.ChunkSize;
            _isInitialized = true;
        }
    }
}