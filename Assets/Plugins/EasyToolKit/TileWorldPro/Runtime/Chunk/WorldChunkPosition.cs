using System;

namespace EasyToolKit.TileWorldPro
{
    public class WorldChunkPosition : IEquatable<WorldChunkPosition>
    {
        private readonly ushort _x;
        private readonly ushort _y;
        private WorldChunk _chunk;

        public ushort X => _x;
        public ushort Y => _y;

        public WorldChunk Chunk
        {
            get => _chunk;
            set => _chunk = value;
        }

        public WorldChunkPosition(ushort x, ushort y)
        {
            _x = x;
            _y = y;
            _chunk = null;
        }

        public override int GetHashCode()
        {
            return ((int)_x << 16) | (int)_y;
        }

        public bool Equals(WorldChunkPosition other)
        {
            return _x == other._x && _y == other._y;
        }

        public override bool Equals(object obj)
        {
            return obj is WorldChunkPosition other && Equals(other);
        }

        public static bool operator ==(WorldChunkPosition left, WorldChunkPosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(WorldChunkPosition left, WorldChunkPosition right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"({_x}, {_y})";
        }
    }
}