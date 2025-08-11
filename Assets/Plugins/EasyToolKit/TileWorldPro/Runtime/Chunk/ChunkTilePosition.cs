using System;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [Serializable]
    public struct ChunkTilePosition : IEquatable<ChunkTilePosition>
    {
        [SerializeField] private ushort _x;
        [SerializeField] private ushort _y;
        [SerializeField] private ushort _z;

        public ushort X => _x;
        public ushort Y => _y;
        public ushort Z => _z;

        public ChunkTilePosition(ushort x, ushort y, ushort z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public TilePosition ToTilePosition(ChunkArea area)
        {
            return area.GetStartTilePosition() + new TilePosition(_x, _y, _z);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_x, _y, _z);
        }

        public bool Equals(ChunkTilePosition other)
        {
            return _x == other._x && _y == other._y && _z == other._z;
        }

        public override bool Equals(object obj)
        {
            return obj is ChunkTilePosition other && Equals(other);
        }

        public static bool operator ==(ChunkTilePosition left, ChunkTilePosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ChunkTilePosition left, ChunkTilePosition right)
        {
            return !left.Equals(right);
        }

        public override readonly string ToString()
        {
            return $"({_x}, {_y}, {_z})";
        }
    }
}