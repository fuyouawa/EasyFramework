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
    public class TileWorldAsset : ScriptableObject, ISerializationCallbackReceiver
    {
        [LabelText("瓦片大小")]
        [SerializeField] private float _tileSize = 1f;

        [LabelText("世界区块大小")]
        [ReadOnly]
        [SerializeField] private Vector3Int _worldChunkSize;

        [HideLabel]
        [SerializeField]
        private TerrainDefinitionSet _terrainDefinitionSet;

        private readonly Dictionary<WorldChunkPosition, WorldChunk> _chunks = new Dictionary<WorldChunkPosition, WorldChunk>();

        [SerializeField, HideInInspector] private bool _isInitialized = false;

        public float TileSize => _tileSize;
        public Vector3Int WorldChunkSize => _worldChunkSize;
        public TerrainDefinitionSet TerrainDefinitionSet => _terrainDefinitionSet;

        public IEnumerable<WorldChunk> EnumerateChunks()
        {
            return _chunks.Values;
        }

        public WorldChunk GetChunkAt(TilePosition tilePosition)
        {
            var chunkX = Mathf.FloorToInt((float)tilePosition.X / _worldChunkSize.x);
            var chunkY = Mathf.FloorToInt((float)tilePosition.Z / _worldChunkSize.z);
            var chunkIndex = new WorldChunkPosition((ushort)chunkX, (ushort)chunkY);
            if (_chunks.TryGetValue(chunkIndex, out var chunk))
            {
                chunkIndex.Chunk = chunk;
                return chunk;
            }

            var area = new WorldChunkArea(chunkIndex, _worldChunkSize);
            chunk = new WorldChunk(area);
            chunkIndex.Chunk = chunk;
            _chunks[chunkIndex] = chunk;
            return chunk;
        }

        public WorldChunkTilePosition GetChunkTilePositionOf(TilePosition tilePosition)
        {
            return GetChunkAt(tilePosition).Area.GetChunkTilePositionOf(tilePosition);
        }

        public Guid? TryGetTerrainGuidAt(TilePosition tilePosition)
        {
            return GetChunkAt(tilePosition).TryGetTerrainGuidAt(tilePosition);
        }

        public void SetTilesAt(IEnumerable<TilePosition> tilePositions, Guid terrainGuid)
        {
            WorldChunk chunkCache = null;
            var tilesCache = new TilePosition[1];
            foreach (var tilePosition in tilePositions)
            {
                if (chunkCache == null || !chunkCache.Area.Contains(tilePosition))
                {
                    chunkCache = GetChunkAt(tilePosition);
                }

                tilesCache[0] = tilePosition;
                chunkCache.SetTilesAt(tilesCache, terrainGuid);
            }
        }

        public void RemoveTilesAt(IEnumerable<TilePosition> tilePositions, Guid terrainGuid)
        {
            WorldChunk chunkCache = null;
            var tilesCache = new TilePosition[1];
            foreach (var tilePosition in tilePositions)
            {
                if (chunkCache == null || !chunkCache.Area.Contains(tilePosition))
                {
                    chunkCache = GetChunkAt(tilePosition);
                }

                tilesCache[0] = tilePosition;
                chunkCache.RemoveTilesAt(tilesCache, terrainGuid);
            }
        }

        public void SetTileAt(TilePosition tilePosition, Guid terrainGuid)
        {
            GetChunkAt(tilePosition).SetTilesAt(new[] { tilePosition }, terrainGuid);
        }

        public void RemoveTileAt(TilePosition tilePosition, Guid terrainGuid)
        {
            GetChunkAt(tilePosition).RemoveTilesAt(new[] { tilePosition }, terrainGuid);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (!_isInitialized)
            {
                _worldChunkSize = TileWorldConfigAsset.Instance.WorldChunkSize;
                _isInitialized = true;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }
    }
}