using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    public class TileWorldStartPoint : MonoBehaviour
    {
        public static readonly float Epsilon = 0.00001f;

        public TilePosition WorldPositionToTilePosition(Vector3 worldPosition, float tileSize)
        {
            var local = worldPosition - transform.position;
            int gridX = (local.x / tileSize).SafeFloorToInt(Epsilon);
            int gridY = (local.y / tileSize).SafeFloorToInt(Epsilon);
            int gridZ = (local.z / tileSize).SafeFloorToInt(Epsilon);

            return new TilePosition(gridX, gridY, gridZ);
        }

        public Vector3 TilePositionToWorldPosition(TilePosition tilePosition, float tileSize)
        {
            return transform.position + (Vector3)tilePosition * tileSize;
        }

        public Vector3 WorldPositionToTileWorldPosition(Vector3 worldPosition, float tileSize)
        {
            var tilePosition = WorldPositionToTilePosition(worldPosition, tileSize);
            var tileWorldPosition = TilePositionToWorldPosition(tilePosition, tileSize);
            // Debug.Log($"tilePosition: {tilePosition} worldPosition: {worldPosition} tileWorldPosition: {tileWorldPosition}");
            return tileWorldPosition;
        }
    }
}