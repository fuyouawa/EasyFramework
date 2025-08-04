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

        public void EnsureInitializeMap()
        {
            var terrainObjects = MapObject.GetComponentsInChildren<TerrainObject>(true);

            foreach (var terrainObject in terrainObjects)
            {
                if (!Asset.TerrainMap.DefinitionSet.Contains(terrainObject.TargetTerrainDefinitionGuid))
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

            foreach (var terrainTileDefiniton in Asset.TerrainMap.DefinitionSet)
            {
                var terrainObject = terrainObjects.FirstOrDefault(terrainObject =>
                    terrainObject.TargetTerrainDefinitionGuid == terrainTileDefiniton.Guid);
                if (terrainObject == null)
                {
                    terrainObject = CreateTerrainObject(terrainTileDefiniton);
                }

                terrainObject.TilemapCreator = this;
            }
        }

        public void ClearAllMap()
        {
            foreach (var terrainObject in MapObject.GetComponentsInChildren<TerrainObject>(true))
            {
                terrainObject.ClearTiles();
            }
        }

        public void ClearMap(Guid targetTerrainDefinitionGuid)
        {
            EnsureInitializeMap();

            var targetTerrainObject = FindTerrainObject(targetTerrainDefinitionGuid);
            if (targetTerrainObject == null)
            {
                return;
            }

            targetTerrainObject.ClearTiles();
        }

        public TerrainObject CreateTerrainObject(TerrainDefinition terrainDefinition)
        {
            var terrainObject = new GameObject(terrainDefinition.Name).AddComponent<TerrainObject>();
            terrainObject.transform.SetParent(MapObject.transform);
            terrainObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            terrainObject.transform.localScale = Vector3.one;
            terrainObject.TargetTerrainDefinitionGuid = terrainDefinition.Guid;
            terrainObject.TilemapCreator = this;
            return terrainObject;
        }

        public TerrainObject FindTerrainObject(Guid terrainDefinitionGuid)
        {
            return MapObject.GetComponentsInChildren<TerrainObject>(true)
                .FirstOrDefault(terrainObject =>
                    terrainObject.TargetTerrainDefinitionGuid == terrainDefinitionGuid);
        }

        public void BuildAllMap()
        {
            foreach (var terrainTileDefinition in Asset.TerrainMap.DefinitionSet)
            {
                BuildMapFor(terrainTileDefinition.Guid);
            }
        }

        public void BuildMapFor(Guid targetTerrainDefinitionGuid)
        {
            EnsureInitializeMap();

            var targetTerrainObject = FindTerrainObject(targetTerrainDefinitionGuid);

            foreach (var terrainTilePosition in Asset.TerrainMap.EnumerateMatchedMap(targetTerrainDefinitionGuid))
            {
                targetTerrainObject.BuildTile(terrainTilePosition);
            }
        }

        public void IncrementalBuildAt(Guid targetTerrainDefinitionGuid, Vector3Int tilePosition)
        {
            EnsureInitializeMap();

            var targetTerrainObject = FindTerrainObject(targetTerrainDefinitionGuid);

            foreach (var terrainTilePosition in Asset.TerrainMap.EnumerateNearlyMatchedMap(
                         targetTerrainDefinitionGuid,
                         tilePosition,
                         Asset.Settings.MaxIncrementalBuildDepth))
            {
                targetTerrainObject.DestroyTileAt(terrainTilePosition.TilePosition);
                targetTerrainObject.BuildTile(terrainTilePosition);
            }
        }

        public void DestroyTileAt(Guid targetTerrainDefinitionGuid, Vector3Int tilePosition)
        {
            EnsureInitializeMap();

            var targetTerrainObject = FindTerrainObject(targetTerrainDefinitionGuid);
            targetTerrainObject.DestroyTileAt(tilePosition);
        }
    }
}
