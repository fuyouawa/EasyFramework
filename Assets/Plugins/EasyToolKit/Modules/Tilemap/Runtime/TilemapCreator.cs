using System;
using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    public class TilemapCreator : MonoBehaviour
    {
        [SerializeField, InlineEditor] private TilemapAsset _asset;
        
        private GameObject _target;

        public TilemapAsset Asset => _asset;

        public Vector3Int WorldPositionToTilePosition(Vector3 worldPosition)
        {
            if (_asset == null)
            {
                throw new NullReferenceException();
            }

            var local = worldPosition - transform.position;
            var tileSize = _asset.Settings.TileSize;
            int gridX = Mathf.FloorToInt(local.x / tileSize);
            int gridY = Mathf.FloorToInt(local.y / tileSize);
            int gridZ = Mathf.FloorToInt(local.z / tileSize);

            return new Vector3Int(gridX, gridY, gridZ);
        }

        public Vector3 TilePositionToWorldPosition(Vector3Int tilePosition)
        {
            if (_asset == null)
            {
                throw new NullReferenceException();
            }

            var tileSize = _asset.Settings.TileSize;
            return transform.position + new Vector3(tilePosition.x * tileSize, tilePosition.y * tileSize, tilePosition.z * tileSize);
        }

        public Vector3 WorldPositionToBlockPosition(Vector3 worldPosition)
        {
            return TilePositionToWorldPosition(WorldPositionToTilePosition(worldPosition));
        }
    }
}
