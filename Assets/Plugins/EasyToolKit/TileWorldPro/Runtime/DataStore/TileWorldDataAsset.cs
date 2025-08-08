using System.Collections.Generic;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [CreateAssetMenu(menuName = "EasyToolKit/TileWorldPro/TileWorldDataAsset", fileName = "TileWorldDataAsset")]
    public class TileWorldDataAsset : ScriptableObject
    {
        private readonly Dictionary<ChunkPosition, Chunk> _chunks = new Dictionary<ChunkPosition, Chunk>();

        public IReadOnlyDictionary<ChunkPosition, Chunk> Chunks => _chunks;

        public void SetChunk(Chunk chunk)
        {
            _chunks[chunk.Area.Position] = chunk;
        }

        public void RemoveChunk(ChunkPosition chunkPosition)
        {
            _chunks.Remove(chunkPosition);
        }
    }
}