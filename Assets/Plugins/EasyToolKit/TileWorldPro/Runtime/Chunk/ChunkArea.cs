using System;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    public struct ChunkArea
    {
        private readonly ChunkPosition _position;
        private readonly Vector2Int _size;

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
                (_position.X + 1) * _size.x - 1,
                _size.y,
                (_position.Y + 1) * _size.y - 1);
        }

        public ChunkTilePosition TilePositionToChunkTilePosition(TilePosition tilePosition)
        {
            var x = tilePosition.X % _size.x;
            var y = tilePosition.Z % _size.y;
            Assert.IsTrue(x >= 0 && x < _size.x, $"X '{x}' must be less than {_size.x} and greater than 0");
            Assert.IsTrue(y >= 0 && y < _size.y, $"Y '{y}' must be less than {_size.y} and greater than 0");

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