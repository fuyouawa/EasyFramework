using System;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    public struct ChunkPosition : IEquatable<ChunkPosition>
    {
        private readonly ushort _x;
        private readonly ushort _y;

        public ushort X => _x;
        public ushort Y => _y;

        public ChunkPosition(ushort x, ushort y)
        {
            _x = x;
            _y = y;
        }

        public override int GetHashCode()
        {
            return ((int)_x << 16) | (int)_y;
        }

        public bool Equals(ChunkPosition other)
        {
            return _x == other._x && _y == other._y;
        }

        public override bool Equals(object obj)
        {
            return obj is ChunkPosition other && Equals(other);
        }

        public static bool operator ==(ChunkPosition left, ChunkPosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ChunkPosition left, ChunkPosition right)
        {
            return !left.Equals(right);
        }

        public static implicit operator Vector2Int(ChunkPosition position)
        {
            return new Vector2Int(position.X, position.Y);
        }

        public static implicit operator ChunkPosition(Vector2Int position)
        {
            return new ChunkPosition((ushort)position.x, (ushort)position.y);
        }

        public override string ToString()
        {
            return $"({_x}, {_y})";
        }
    }
}