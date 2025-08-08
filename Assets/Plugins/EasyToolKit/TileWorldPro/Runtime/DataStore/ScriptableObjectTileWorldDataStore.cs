using System;
using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Inspector;
using EasyToolKit.TileWorldPro;
using UnityEngine;

[assembly: RegisterTileWorldDataStore(typeof(ScriptableObjectTileWorldDataStore), "（默认）ScriptableObject")]
namespace EasyToolKit.TileWorldPro
{
    [Serializable]
    public class ScriptableObjectTileWorldDataStore : ITileWorldDataStore
    {
        [LabelText("数据资产")]
        [SerializeField] private TileWorldDataAsset _tileWorldDataAsset;

        public bool IsValid => _tileWorldDataAsset != null;

        public void UpdateChunk(Chunk chunk)
        {
            _tileWorldDataAsset.SetChunk(chunk);
        }

        public IEnumerable<Chunk> EnumerateChunks()
        {
            return _tileWorldDataAsset.Chunks.Values;
        }

        public void RemoveChunk(ChunkPosition chunkPosition)
        {
            _tileWorldDataAsset.RemoveChunk(chunkPosition);
        }

        public Chunk TryGetChunk(ChunkPosition chunkPosition)
        {
            return _tileWorldDataAsset.Chunks.GetValueOrDefault(chunkPosition);
        }

        public void UpdateChunkRange(IEnumerable<Chunk> chunks)
        {
            foreach (var chunk in chunks)
            {
                _tileWorldDataAsset.SetChunk(chunk);
            }
        }
    }
}