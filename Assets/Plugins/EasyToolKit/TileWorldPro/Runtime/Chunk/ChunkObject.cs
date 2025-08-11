using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Inspector;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [EasyInspector]
    public class ChunkObject : SerializedMonoBehaviour
    {
        [SerializeField, ReadOnly] private TileWorldBuilder _builder;
        [OdinSerialize, ShowInInspector, ReadOnly] private ChunkArea _area;

        private Dictionary<Guid, ChunkTerrainObject> _terrainObjects;

        public TileWorldBuilder Builder => _builder;
        public ChunkArea Area => _area;
        public IReadOnlyDictionary<Guid, ChunkTerrainObject> TerrainObjects
        {
            get
            {
                if (_terrainObjects == null)
                {
                    _terrainObjects = transform.GetComponentsInChildren<ChunkTerrainObject>(true)
                        .ToDictionary(terrainObject => terrainObject.TerrainDefinition.Guid);
                }
                return _terrainObjects;
            }
        }

        public void Initialize(TileWorldBuilder builder, ChunkArea area)
        {
            _builder = builder;
            _area = area;
            Refresh();
        }

        public void Refresh()
        {
            var terrainObjects = transform.GetComponentsInChildren<ChunkTerrainObject>(true).ToList();

            foreach (var terrainObject in terrainObjects)
            {
                if (!Builder.TileWorldAsset.TerrainDefinitionSet.Contains(terrainObject.TerrainDefinition.Guid))
                {
                    if (Application.isPlaying)
                    {
                        Destroy(terrainObject.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(terrainObject.gameObject);
                    }

                    terrainObjects.Remove(terrainObject);
                }
            }

            foreach (var terrainDefinition in Builder.TileWorldAsset.TerrainDefinitionSet)
            {
                var terrainObject = terrainObjects.FirstOrDefault(terrainObject =>
                    terrainObject.TerrainDefinition.Guid == terrainDefinition.Guid);

                if (terrainObject == null)
                {
                    terrainObject = CreateTerrainObject(terrainDefinition);
                    terrainObject.Initialize(this, terrainDefinition.Guid);
                    terrainObjects.Add(terrainObject);
                }
            }

            _terrainObjects = terrainObjects.ToDictionary(terrainObject => terrainObject.TerrainDefinition.Guid);
        }

        public ChunkTerrainObject GetTerrainObject(Guid terrainGuid)
        {
            if (TerrainObjects.TryGetValue(terrainGuid, out var terrainObject) && terrainObject != null)
            {
                return terrainObject;
            }

            var terrainDefinition = Builder.TileWorldAsset.TerrainDefinitionSet.TryGetByGuid(terrainGuid);
            terrainObject = CreateTerrainObject(terrainDefinition);
            terrainObject.Initialize(this, terrainDefinition.Guid);
            _terrainObjects[terrainGuid] = terrainObject;
            return terrainObject;
        }

        public void AddTile(GameObject tileInstance, Guid terrainGuid, ChunkTilePosition chunkTilePosition, TerrainTileRuleType ruleType)
        {
            var terrainObject = GetTerrainObject(terrainGuid);
            terrainObject.AddTile(tileInstance, chunkTilePosition, ruleType);
        }

        public void DestroyTile(Guid terrainGuid, ChunkTilePosition chunkTilePosition)
        {
            var terrainObject = GetTerrainObject(terrainGuid);
            terrainObject.DestroyTileAt(chunkTilePosition);
        }

        public void ClearAll(bool destroyTerrainObjects = true)
        {
            foreach (var terrainObject in TerrainObjects.Values)
            {
                if (destroyTerrainObjects)
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
                else
                {
                    terrainObject.DestroyAll();
                }
            }

            if (destroyTerrainObjects)
            {
                _terrainObjects.Clear();
            }
        }

        public void ClearTerrain(Guid terrainGuid, bool destroyTerrainObject = true)
        {
            if (TerrainObjects.TryGetValue(terrainGuid, out var terrainObject))
            {
                if (destroyTerrainObject)
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
                else
                {
                    terrainObject.DestroyAll();
                }
            }
        }

        private ChunkTerrainObject CreateTerrainObject(TerrainDefinition terrainDefinition)
        {
            var terrainObject = new GameObject($"Terrain_{terrainDefinition.Name}").AddComponent<ChunkTerrainObject>();
            terrainObject.transform.SetParent(transform);
            terrainObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            terrainObject.transform.localScale = Vector3.one;
            terrainObject.Initialize(this, terrainDefinition.Guid);
            return terrainObject;
        }
    }
}