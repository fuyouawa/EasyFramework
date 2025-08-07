using System;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [Serializable]
    public struct ChunkArea
    {
        [SerializeField] private ChunkPosition _position;
        [SerializeField] private Vector2Int _size;

        public ChunkPosition Position => _position;
        public Vector2Int Size => _size;

        public ChunkArea(ChunkPosition position, Vector2Int size)
        {
            _position = position;
            _size = size;
        }

        public TilePosition GetStartTilePosition()
        {
            return new TilePosition(
                _position.X * _size.x,
                0,
                _position.Y * _size.y);
        }

        public TilePosition GetEndTilePosition()
        {
            return new TilePosition(
                (_position.X + 1) * _size.x,
                _size.y,
                (_position.Y + 1) * _size.y);
        }

        public ChunkTilePosition TilePositionToChunkTilePosition(TilePosition tilePosition)
        {
            var x = tilePosition.X % _size.x;
            var y = tilePosition.Z % _size.y;
            return new ChunkTilePosition((ushort)x, (ushort)tilePosition.Y, (ushort)y);
        }

        public bool Contains(TilePosition tilePosition)
        {
            var startTilePosition = GetStartTilePosition();
            var endTilePosition = GetEndTilePosition();
            return tilePosition.X >= startTilePosition.X && tilePosition.X <= endTilePosition.X &&
                   tilePosition.Y >= startTilePosition.Y && tilePosition.Y <= endTilePosition.Y &&
                   tilePosition.Z >= startTilePosition.Z && tilePosition.Z <= endTilePosition.Z;
        }
    }
}