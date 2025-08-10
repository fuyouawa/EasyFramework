using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using EasyToolKit.Inspector;
using EasyToolKit.ThirdParty.OdinSerializer;
using EasyToolKit.TileWorldPro;
using UnityEngine;

[assembly: RegisterTileWorldDataStore(typeof(BuiltInTileWorldDataStore), "BuiltIn")]
namespace EasyToolKit.TileWorldPro
{
    [Serializable]
    public class BuiltInTileWorldDataStore : ITileWorldDataStore, ISerializationCallbackReceiver
    {
        private Dictionary<ChunkPosition, Chunk> _chunks;
        private Dictionary<ChunkPosition, BakedChunk> _bakedChunks;

        public IReadOnlyDictionary<ChunkPosition, Chunk> Chunks => _chunks;
        public IReadOnlyDictionary<ChunkPosition, BakedChunk> BakedChunks => _bakedChunks;

        public int ChunkMemorySize => _serializedChunks?.Length ?? 0;
        public int BakedChunkMemorySize => _serializedBakedChunks?.Length ?? 0;

        private bool _dirty = false;
        private bool _dirtyBaked = false;
        [SerializeField, HideInInspector] private byte[] _serializedChunks;
        [SerializeField, HideInInspector] private byte[] _serializedBakedChunks;

        public bool IsValid => _chunks != null || _bakedChunks != null;

        public void UpdateChunk(Chunk chunk)
        {
            _chunks[chunk.Area.Position] = chunk;
            _dirty = true;
        }

        public IEnumerable<Chunk> EnumerateChunks()
        {
            return _chunks.Values;
        }

        public void RemoveChunk(ChunkPosition chunkPosition)
        {
            _chunks.Remove(chunkPosition);
            _dirty = true;
        }

        public Chunk TryGetChunk(ChunkPosition chunkPosition)
        {
            return _chunks.GetValueOrDefault(chunkPosition);
        }

        public void UpdateChunkRange(IEnumerable<Chunk> chunks)
        {
            foreach (var chunk in chunks)
            {
                UpdateChunk(chunk);
            }
        }

        public bool ContainsAnyBakedChunk()
        {
            return _serializedBakedChunks.IsNotNullOrEmpty() || _bakedChunks.Count > 0;
        }

        public IEnumerable<BakedChunk> EnumerateBakedChunks()
        {
            return _bakedChunks.Values;
        }

        public BakedChunk TryGetBakedChunk(ChunkPosition chunkPosition)
        {
            return _bakedChunks.GetValueOrDefault(chunkPosition);
        }

        public void UpdateBakedChunk(BakedChunk bakedChunk)
        {
            _bakedChunks[bakedChunk.Area.Position] = bakedChunk;
            _dirtyBaked = true;
        }

        public void UpdateBakedChunkRange(IEnumerable<BakedChunk> bakedChunks)
        {
            foreach (var bakedChunk in bakedChunks)
            {
                UpdateBakedChunk(bakedChunk);
            }
        }

        public void TransferData(ITileWorldDataStore targetDataStore)
        {
            UpdateChunkRange(targetDataStore.EnumerateChunks());
            UpdateBakedChunkRange(targetDataStore.EnumerateBakedChunks());
        }

        public void ClearAllChunks()
        {
            _chunks.Clear();
            _dirty = true;
        }

        public void ClearAllBakedChunks()
        {
            _bakedChunks.Clear();
            _dirtyBaked = true;
        }

        public void Dispose()
        {
            ClearAllChunks();
            ClearAllBakedChunks();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (_dirty)
            {
                _serializedChunks = SerializationUtility.SerializeValue(_chunks.Values.ToArray(), DataFormat.Binary);
                _dirty = false;
            }

            if (_dirtyBaked)
            {
                _serializedBakedChunks = SerializationUtility.SerializeValue(_bakedChunks.Values.ToArray(), DataFormat.Binary);
                _dirtyBaked = false;
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (_serializedChunks != null && _serializedChunks.Length > 0)
            {
                var chunks = SerializationUtility.DeserializeValue<Chunk[]>(_serializedChunks, DataFormat.Binary);
                _chunks = chunks.ToDictionary(chunk => chunk.Area.Position, chunk => chunk);
            }

            if (_serializedBakedChunks != null && _serializedBakedChunks.Length > 0)
            {
                var bakedChunks = SerializationUtility.DeserializeValue<BakedChunk[]>(_serializedBakedChunks, DataFormat.Binary);
                _bakedChunks = bakedChunks.ToDictionary(chunk => chunk.Area.Position, chunk => chunk);
            }

            if (_chunks == null)
            {
                _chunks = new Dictionary<ChunkPosition, Chunk>();
            }

            if (_bakedChunks == null)
            {
                _bakedChunks = new Dictionary<ChunkPosition, BakedChunk>();
            }
        }
    }
}