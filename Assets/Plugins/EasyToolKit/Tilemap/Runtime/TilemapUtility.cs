using System;
using EasyToolKit.Core;
using UnityEditor.VersionControl;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    public static class TilemapUtility
    {
        public static readonly float Epsilon = 0.00001f;

        public static Vector3Int WorldPositionToTilePosition(
            Vector3 relativePosition,
            Vector3 worldPosition,
            float tileSize)
        {
            var local = worldPosition - relativePosition;
            int gridX = (local.x / tileSize).SafeFloorToInt(Epsilon);
            int gridY = (local.y / tileSize).SafeFloorToInt(Epsilon);
            int gridZ = (local.z / tileSize).SafeFloorToInt(Epsilon);

            return new Vector3Int(gridX, gridY, gridZ);
        }

        public static Vector3 TilePositionToWorldPosition(Vector3 relativePosition, Vector3 tilePosition,
            float tileSize)
        {
            return relativePosition + new Vector3(tilePosition.x * tileSize, tilePosition.y * tileSize,
                tilePosition.z * tileSize);
        }

        public static Vector3 WorldPositionToBlockPosition(Vector3 relativePosition, Vector3 worldPosition,
            float tileSize)
        {
            return TilePositionToWorldPosition(relativePosition,
                WorldPositionToTilePosition(relativePosition, worldPosition, tileSize),
                tileSize);
        }
    }
}
