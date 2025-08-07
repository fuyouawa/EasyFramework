using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    public class TerrainObjectManager : MonoBehaviour
    {
        private TileWorldBuilder _builder;

        public TileWorldBuilder Builder => _builder;

        private readonly Dictionary<Guid, TerrainObject> _terrainObjectsCache = new Dictionary<Guid, TerrainObject>();

        public void Initialize(TileWorldBuilder builder)
        {
            _builder = builder;
            Refresh();
        }

        public void Refresh()
        {
            var terrainObjects = transform.GetComponentsInChildren<TerrainObject>(true).ToList();

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

            foreach (var terrainObject in terrainObjects)
            {
                _terrainObjectsCache[terrainObject.TerrainDefinition.Guid] = terrainObject;
            }
        }

        public TerrainObject GetTerrainObject(Guid targetTerrainGuid)
        {
            if (_terrainObjectsCache.TryGetValue(targetTerrainGuid, out var terrainObject) && terrainObject != null)
            {
                return terrainObject;
            }

            Refresh();
            return GetTerrainObject(targetTerrainGuid);
        }

        public void Clear()
        {
            foreach (var terrainObject in _terrainObjectsCache.Values)
            {
                terrainObject.Clear();
            }
        }

        private TerrainObject CreateTerrainObject(TerrainDefinition terrainDefinition)
        {
            var terrainObject = new GameObject(terrainDefinition.Name).AddComponent<TerrainObject>();
            terrainObject.transform.SetParent(transform);
            terrainObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            terrainObject.transform.localScale = Vector3.one;
            terrainObject.Initialize(this, terrainDefinition.Guid);
            return terrainObject;
        }
    }
}
