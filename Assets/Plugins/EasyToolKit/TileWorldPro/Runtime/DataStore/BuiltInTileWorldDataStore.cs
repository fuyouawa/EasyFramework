using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Inspector;
using EasyToolKit.TileWorldPro;
using UnityEngine;

[assembly: RegisterTileWorldDataStore(typeof(BuiltInTileWorldDataStore), "BuiltIn")]
namespace EasyToolKit.TileWorldPro
{
    [Serializable]
    public class BuiltInTileWorldDataStore : ITileWorldDataStore
    {
        private Dictionary<ChunkPosition, Chunk> _chunks = new Dictionary<ChunkPosition, Chunk>();

        public IReadOnlyDictionary<ChunkPosition, Chunk> Chunks => _chunks;

        public bool IsValid => true;

        public void UpdateChunk(Chunk chunk)
        {
            _chunks[chunk.Area.Position] = chunk;
        }

        public IEnumerable<Chunk> EnumerateChunks()
        {
            return _chunks.Values;
        }

        public void RemoveChunk(ChunkPosition chunkPosition)
        {
            _chunks.Remove(chunkPosition);
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

        public void Dispose()
        {
            _chunks.Clear();
        }
    }
}