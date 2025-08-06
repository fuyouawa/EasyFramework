using System;

namespace EasyToolKit.TileWorldPro
{
    public struct WorldChunkTilePosition : IEquatable<WorldChunkTilePosition>
    {
        private readonly ushort _x;
        private readonly ushort _y;
        private readonly ushort _z;
        private WorldChunk _chunk;

        public ushort X => _x;
        public ushort Y => _y;
        public ushort Z => _z;

        public WorldChunk Chunk
        {
            get => _chunk;
            set => _chunk = value;
        }

        public static WorldChunkTilePosition FromCode(int code)
        {
            return new WorldChunkTilePosition(
                (ushort)(code >> 20 & 0x0FFF),
                (ushort)(code >> 8 & 0x0FFF),
                (ushort)(code & 0x00FF));
        }

        public WorldChunkTilePosition(ushort x, ushort y, ushort z)
        {
            if (x > 1 << 12)
                throw new ArgumentException("X must be less than 4096", nameof(x));
            if (y > 1 << 8)
                throw new ArgumentException("Y must be less than 256", nameof(y));
            if (z > 1 << 12)
                throw new ArgumentException("Z must be less than 4096", nameof(z));

            _x = x;
            _y = y;
            _z = z;
            _chunk = null;
        }

        public TilePosition ConvertToTilePosition()
        {
            return _chunk.Area.GetStartTilePosition() + new TilePosition(_x, _y, _z);
        }

        public override readonly int GetHashCode()
        {
            return ((_x & 0x0FFF) << 20) | ((_z & 0x0FFF) << 8) | (_y & 0x00FF);
        }

        public bool Equals(WorldChunkTilePosition other)
        {
            return _x == other._x && _y == other._y && _z == other._z;
        }

        public override bool Equals(object obj)
        {
            return obj is WorldChunkTilePosition other && Equals(other);
        }

        public static bool operator ==(WorldChunkTilePosition left, WorldChunkTilePosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(WorldChunkTilePosition left, WorldChunkTilePosition right)
        {
            return !left.Equals(right);
        }

        public override readonly string ToString()
        {
            return $"({_x}, {_y}, {_z})";
        }
    }
}