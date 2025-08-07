using System;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [Serializable]
    public struct ChunkTilePosition : IEquatable<ChunkTilePosition>, ISerializationCallbackReceiver
    {
        private ushort _x;
        private ushort _y;
        private ushort _z;

        [SerializeField] private int _serializedCode;

        public ushort X => _x;
        public ushort Y => _y;
        public ushort Z => _z;

        public int Code => _serializedCode;

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
            _serializedCode = ((_x & 0x0FFF) << 20) | ((_z & 0x0FFF) << 8) | (_y & 0x00FF);
        }

        public TilePosition ToTilePosition(ChunkArea area)
        {
            return area.GetStartTilePosition() + new TilePosition(_x, _y, _z);
        }

        public override readonly int GetHashCode()
        {
            return _serializedCode;
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

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _serializedCode = ((_x & 0x0FFF) << 20) | ((_z & 0x0FFF) << 8) | (_y & 0x00FF);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _x = (ushort)(_serializedCode >> 20 & 0x0FFF);
            _y = (ushort)(_serializedCode >> 8 & 0x0FFF);
            _z = (ushort)(_serializedCode & 0x00FF);
        }
    }
}