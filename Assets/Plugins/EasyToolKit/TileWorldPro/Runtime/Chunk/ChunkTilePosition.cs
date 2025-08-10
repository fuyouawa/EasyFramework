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
            if (x > 1 << 12)
                throw new ArgumentException($"X '{x}' must be less than 4096", nameof(x));
            if (y > 1 << 8)
                throw new ArgumentException($"Y '{y}' must be less than 256", nameof(y));
            if (z > 1 << 12)
                throw new ArgumentException($"Z '{z}' must be less than 4096", nameof(z));

            _x = x;
            _y = y;
            _z = z;
        }

        public static ChunkTilePosition FromPackedCode(uint packedCode)
        {
            return new ChunkTilePosition(
                (ushort)(packedCode >> 20 & 0x0FFF),
                (ushort)(packedCode >> 8 & 0x0FFF),
                (ushort)(packedCode & 0x00FF));
        }

        public TilePosition ToTilePosition(ChunkArea area)
        {
            return area.GetStartTilePosition() + new TilePosition(_x, _y, _z);
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