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
        void UpdateChunk(Chunk chunk);
        void UpdateChunkRange(IEnumerable<Chunk> chunks);
        void ClearAllChunks();

        bool ContainsAnyBakedChunk();
        IEnumerable<BakedChunk> EnumerateBakedChunks();

        BakedChunk TryGetBakedChunk(ChunkPosition chunkPosition);

        void UpdateBakedChunk(BakedChunk bakedChunk);
        void UpdateBakedChunkRange(IEnumerable<BakedChunk> bakedChunks);
        void ClearAllBakedChunks();

        void TransferData(ITileWorldDataStore targetDataStore);
    }
}