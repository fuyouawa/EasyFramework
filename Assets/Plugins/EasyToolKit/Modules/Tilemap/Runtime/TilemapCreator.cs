using EasyToolKit.Inspector;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EasyToolKit.Tilemap
{
    public class TilemapCreator : MonoBehaviour
    {
        [SerializeField, InlineEditor] private TilemapAsset _asset;

        [SerializeField] private GameObject _mapObject;

        public TilemapAsset Asset => _asset;

        public GameObject MapObject
        {
            get
            {
                if (_mapObject == null)
                {
                    _mapObject = new GameObject(Asset.Settings.MapName);
                    SceneManager.MoveGameObjectToScene(_mapObject, gameObject.scene);
                    _mapObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
                    _mapObject.transform.localScale = Vector3.one;
                }
                return _mapObject;
            }
        }

        public Vector3Int WorldPositionToTilePosition(Vector3 worldPosition)
        {
            if (Asset == null)
            {
                throw new NullReferenceException();
            }

            var local = worldPosition - transform.position;
            var tileSize = Asset.Settings.TileSize;
            int gridX = Mathf.FloorToInt(local.x / tileSize);
            int gridY = Mathf.FloorToInt(local.y / tileSize);
            int gridZ = Mathf.FloorToInt(local.z / tileSize);

            return new Vector3Int(gridX, gridY, gridZ);
        }

        public Vector3 TilePositionToWorldPosition(Vector3Int tilePosition)
        {
            if (Asset == null)
            {
                throw new NullReferenceException();
            }

            var tileSize = Asset.Settings.TileSize;
            return transform.position + new Vector3(tilePosition.x * tileSize, tilePosition.y * tileSize, tilePosition.z * tileSize);
        }

        public Vector3 WorldPositionToBlockPosition(Vector3 worldPosition)
        {
            return TilePositionToWorldPosition(WorldPositionToTilePosition(worldPosition));
        }

        public void EnsureInitializeMap()
        {
            var terrainObjects = MapObject.GetComponentsInChildren<TerrainObject>(true);

            foreach (var terrainObject in terrainObjects)
            {
                if (!Asset.TerrainTileDefinitions.Any(terrainTile => terrainTile.Guid == terrainObject.TargetTerrainTileDefinitionGuid))
                {
                    Destroy(terrainObject.gameObject);
                }
            }

            foreach (var terrainTileDef in Asset.TerrainTileDefinitions)
            {
                var terrainObject = terrainObjects.FirstOrDefault(terrainObject => terrainObject.TargetTerrainTileDefinitionGuid == terrainTileDef.Guid);
                if (terrainObject == null)
                {
                    terrainObject = CreateTerrainObject(terrainTileDef);
                }
            }
        }

        public void ClearAllMap()
        {
            EnsureInitializeMap();

            foreach (var terrainObject in MapObject.GetComponentsInChildren<TerrainObject>(true))
            {
                terrainObject.ClearTiles();
            }
        }

        public void ClearMap(Guid targetTerrainTileDefinitionGuid)
        {
            var targetTerrainObject = FindTerrainObject(targetTerrainTileDefinitionGuid);
            if (targetTerrainObject == null)
            {
                return;
            }

            targetTerrainObject.ClearTiles();
        }

        public TerrainObject CreateTerrainObject(TerrainTileDefinition terrainTileDefinition)
        {
            var terrainObject = new GameObject(terrainTileDefinition.Name).AddComponent<TerrainObject>();
            terrainObject.transform.SetParent(MapObject.transform);
            terrainObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            terrainObject.transform.localScale = Vector3.one;
            terrainObject.TargetTerrainTileDefinitionGuid = terrainTileDefinition.Guid;
            return terrainObject;
        }

        public TerrainObject FindTerrainObject(Guid terrainTileDefinitionGuid)
        {
            return MapObject.GetComponentsInChildren<TerrainObject>(true)
                .FirstOrDefault(terrainObject => terrainObject.TargetTerrainTileDefinitionGuid == terrainTileDefinitionGuid);
        }

        public TerrainObject FindOrCreateTerrainObject(Guid terrainTileDefinitionGuid)
        {
            var terrainObject = FindTerrainObject(terrainTileDefinitionGuid);

            if (terrainObject == null)
            {
                terrainObject = CreateTerrainObject(Asset.TryGetTerrainTileDefinitionByGuid(terrainTileDefinitionGuid));
            }
            return terrainObject;
        }

        public void GenerateMap(Guid targetTerrainTileDefinitionGuid)
        {
            var targetTerrainObject = FindOrCreateTerrainObject(targetTerrainTileDefinitionGuid);

            foreach (var terrainTile in Asset.EnumerateTerrainTiles(targetTerrainTileDefinitionGuid))
            {
                var tilePosition = terrainTile.TilePosition;
                var tileWorldPosition = TilePositionToWorldPosition(tilePosition);
                var ruleType = Asset.CalculateTerrainRuleTypeAt(tilePosition);
                var tileInstance = terrainTile.Definition.RuleSetAsset.GetTileInstanceByRuleType(ruleType);
                tileInstance.transform.SetParent(targetTerrainObject.transform);
                tileInstance.transform.position = tileWorldPosition;
            }
        }
    }
}
