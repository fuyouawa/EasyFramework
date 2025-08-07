using System;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [Serializable]
    public struct TilePosition : IEquatable<TilePosition>
    {
        /// <summary>
        /// TilePosition(0, 0, 0)
        /// </summary>
        public static readonly TilePosition Zero = new TilePosition(0, 0, 0);
        /// <summary>
        /// TilePosition(0, 0, 1)
        /// </summary>
        public static readonly TilePosition Forward = new TilePosition(0, 0, 1);
        /// <summary>
        /// TilePosition(0, 0, -1)
        /// </summary>
        public static readonly TilePosition Back = new TilePosition(0, 0, -1);
        /// <summary>
        /// TilePosition(-1, 0, 0)
        /// </summary>
        public static readonly TilePosition Left = new TilePosition(-1, 0, 0);
        /// <summary>
        /// TilePosition(1, 0, 0)
        /// </summary>
        public static readonly TilePosition Right = new TilePosition(1, 0, 0);

        /// <summary>
        /// TilePosition(0, 0, 1) + TilePosition(-1, 0, 0)
        /// </summary>
        public static readonly TilePosition ForwardLeft = new TilePosition(0, 0, 1) + new TilePosition(-1, 0, 0);
        /// <summary>
        /// TilePosition(0, 0, 1) + TilePosition(1, 0, 0)
        /// </summary>
        public static readonly TilePosition ForwardRight = new TilePosition(0, 0, 1) + new TilePosition(1, 0, 0);
        /// <summary>
        /// TilePosition(0, 0, -1) + TilePosition(-1, 0, 0)
        /// </summary>
        public static readonly TilePosition BackLeft = new TilePosition(0, 0, -1) + new TilePosition(-1, 0, 0);
        /// <summary>
        /// TilePosition(0, 0, -1) + TilePosition(1, 0, 0)
        /// </summary>
        public static readonly TilePosition BackRight = new TilePosition(0, 0, -1) + new TilePosition(1, 0, 0);

        [SerializeField] private Vector3Int _position;

        public int X => _position.x;
        public int Y => _position.y;
        public int Z => _position.z;

        public TilePosition(int x, int y, int z)
        {
            _position = new Vector3Int(x, y, z);
        }

        public TilePosition(Vector3Int position)
        {
            _position = position;
        }

        public ChunkPosition ToChunkPosition(Vector2Int chunkSize)
        {
            var chunkX = Mathf.FloorToInt((float)_position.x / chunkSize.x);
            var chunkY = Mathf.FloorToInt((float)_position.z / chunkSize.y);
            return new ChunkPosition((ushort)chunkX, (ushort)chunkY);
        }

        public bool Equals(TilePosition other)
        {
            return _position == other._position;
        }

        public override bool Equals(object obj)
        {
            return obj is TilePosition other && Equals(other);
        }

        public static bool operator ==(TilePosition left, TilePosition right)
        {
            return left.Equals(right);
        }

        public static TilePosition operator +(TilePosition left, TilePosition right)
        {
            return new TilePosition(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static TilePosition operator -(TilePosition left, TilePosition right)
        {
            return new TilePosition(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static TilePosition operator *(TilePosition position, int scale)
        {
            return new TilePosition(position.X * scale, position.Y * scale, position.Z * scale);
        }

        public static TilePosition operator *(int scale, TilePosition position)
        {
            return new TilePosition(position.X * scale, position.Y * scale, position.Z * scale);
        }

        public static TilePosition operator /(TilePosition position, int scale)
        {
            return new TilePosition(position.X / scale, position.Y / scale, position.Z / scale);
        }

        public static TilePosition operator /(int scale, TilePosition position)
        {
            return new TilePosition(scale / position.X, scale / position.Y, scale / position.Z);
        }

        public static bool operator !=(TilePosition left, TilePosition right)
        {
            return !left.Equals(right);
        }

        public static implicit operator Vector3Int(TilePosition position)
        {
            return position._position;
        }

        public static implicit operator Vector3(TilePosition position)
        {
            return position._position;
        }

        public override int GetHashCode()
        {
            return _position.GetHashCode();
        }

        public override string ToString()
        {
            return _position.ToString();
        }
    }
}