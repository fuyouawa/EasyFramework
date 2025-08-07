using System;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [Serializable]
    public struct ChunkPosition : IEquatable<ChunkPosition>
    {
        [SerializeField] private ushort _x;
        [SerializeField] private ushort _y;

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

        public override string ToString()
        {
            return $"({_x}, {_y})";
        }
    }
}