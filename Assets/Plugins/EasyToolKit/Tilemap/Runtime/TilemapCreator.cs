using EasyToolKit.Core;
using EasyToolKit.Inspector;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EasyToolKit.Tilemap
{
    public class TilemapCreator : MonoBehaviour
    {
        public static readonly float Epsilon = 0.00001f;

        [LabelText("地图资产")]
        [SerializeField, InlineEditor] private TilemapAsset _asset;

        [SerializeField, HideInInspector] private GameObject _mapObject;

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
            int gridX = (local.x / tileSize).SafeFloorToInt(Epsilon);
            int gridY = (local.y / tileSize).SafeFloorToInt(Epsilon);
            int gridZ = (local.z / tileSize).SafeFloorToInt(Epsilon);

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
                if (!Asset.TerrainTileMap.DefinitionSet.Contains(terrainObject.TargetTerrainTileDefinitionGuid))
                {
                    if (Application.isPlaying)
                    {
                        Destroy(terrainObject.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(terrainObject.gameObject);
                    }
                }
            }

            foreach (var terrainTileDefiniton in Asset.TerrainTileMap.DefinitionSet)
            {
                var terrainObject = terrainObjects.FirstOrDefault(terrainObject => terrainObject.TargetTerrainTileDefinitionGuid == terrainTileDefiniton.Guid);
                if (terrainObject == null)
                {
                    terrainObject = CreateTerrainObject(terrainTileDefiniton);
                }
            }
        }

        public void ClearAllMap()
        {
            foreach (var terrainObject in MapObject.GetComponentsInChildren<TerrainObject>(true))
            {
                terrainObject.ClearTiles();
            }
        }

        public void ClearMap(Guid targetTerrainTileDefinitionGuid)
        {
            EnsureInitializeMap();

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

        public void GenerateAllMap()
        {
            foreach (var terrainTileDefinition in Asset.TerrainTileMap.DefinitionSet)
            {
                GenerateMap(terrainTileDefinition.Guid);
            }
        }

        public void GenerateMap(Guid targetTerrainTileDefinitionGuid)
        {
            EnsureInitializeMap();

            var targetTerrainObject = FindTerrainObject(targetTerrainTileDefinitionGuid);

            foreach (var terrainTile in Asset.TerrainTileMap.EnumerateMatchedMap(targetTerrainTileDefinitionGuid))
            {
                var tilePosition = terrainTile.TilePosition;
                var tileWorldPosition = TilePositionToWorldPosition(tilePosition);
                var ruleType = Asset.TerrainTileMap.CalculateRuleTypeAt(tilePosition);
                var tileInstance = terrainTile.Definition.RuleSetAsset.GetTileInstanceByRuleType(ruleType);
                if (tileInstance == null)
                {
                    Debug.LogError($"The Rule Type '{ruleType}' of tile instance is null for tile position '{tilePosition}'");
                    continue;
                }

                tileInstance.transform.SetParent(targetTerrainObject.transform);
                tileInstance.transform.position = tileWorldPosition;
            }
        }
    }
}
