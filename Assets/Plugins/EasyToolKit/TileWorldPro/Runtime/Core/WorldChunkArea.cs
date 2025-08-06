using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    public class WorldChunkArea
    {
        private readonly WorldChunkPosition _position;
        private readonly Vector3Int _size;

        public WorldChunkPosition Position => _position;
        public Vector3Int Size => _size;
        public WorldChunk Chunk => _position.Chunk;

        public WorldChunkArea(WorldChunkPosition position, Vector3Int size)
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

        public WorldChunkTilePosition GetChunkTilePositionOf(TilePosition tilePosition)
        {
            var x = tilePosition.X % _size.x;
            var y = tilePosition.Y % _size.y;
            var z = tilePosition.Z % _size.z;
            return new WorldChunkTilePosition((ushort)x, (ushort)y, (ushort)z)
            {
                Chunk = Chunk
            };
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