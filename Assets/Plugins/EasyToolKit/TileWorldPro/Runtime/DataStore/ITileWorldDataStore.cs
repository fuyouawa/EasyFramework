using System;
using System.Collections.Generic;
using EasyToolKit.Inspector;

namespace EasyToolKit.TileWorldPro
{
    [HideLabel]
    public interface ITileWorldDataStore : IDisposable
    {
        bool IsValid { get; }

        IEnumerable<Chunk> EnumerateChunks();
        Chunk TryGetChunk(ChunkPosition chunkPosition);

        void RemoveChunk(ChunkPosition chunkPosition);

        void TransferData(ITileWorldDataStore targetDataStore);
        void UpdateChunk(Chunk chunk);
        void UpdateChunkRange(IEnumerable<Chunk> chunks);

        void ClearAllChunks();
    }
}