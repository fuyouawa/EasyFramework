using System;
using System.Collections.Generic;

namespace EasyToolKit.TileWorldPro
{
    public interface ITileWorldDataStore
    {
        bool IsValid { get; }

        IEnumerable<Chunk> EnumerateChunks();
        Chunk TryGetChunk(ChunkPosition chunkPosition);

        void RemoveChunk(ChunkPosition chunkPosition);
        void UpdateChunk(Chunk chunk);
        void UpdateChunkRange(IEnumerable<Chunk> chunks);
    }
}