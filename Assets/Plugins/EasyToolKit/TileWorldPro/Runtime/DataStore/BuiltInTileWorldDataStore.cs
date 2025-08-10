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
        private Dictionary<ChunkPosition, Chunk> _chunks = new Dictionary<ChunkPosition, Chunk>();

        public IReadOnlyDictionary<ChunkPosition, Chunk> Chunks => _chunks;

        private bool _dirty = false;
        [SerializeField, HideInInspector] private byte[] _serializedChunks;

        public bool IsValid => true;

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

        public void TransferData(ITileWorldDataStore targetDataStore)
        {
            UpdateChunkRange(targetDataStore.EnumerateChunks());
        }

        public void ClearAllChunks()
        {
            _chunks.Clear();
            _dirty = true;
        }

        public void Dispose()
        {
            ClearAllChunks();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (_dirty)
            {
                _serializedChunks = SerializationUtility.SerializeValue(_chunks.Values.ToArray(), DataFormat.Binary);
                _dirty = false;
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (_serializedChunks != null && _serializedChunks.Length > 0)
            {
                var chunks = SerializationUtility.DeserializeValue<Chunk[]>(_serializedChunks, DataFormat.Binary);
                _chunks = chunks.ToDictionary(chunk => chunk.Area.Position, chunk => chunk);
            }

            if (_chunks == null)
            {
                _chunks = new Dictionary<ChunkPosition, Chunk>();
            }
        }
    }
}