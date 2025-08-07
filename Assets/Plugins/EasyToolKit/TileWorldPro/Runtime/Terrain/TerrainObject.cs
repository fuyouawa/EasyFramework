using System;
using EasyToolKit.Core;
using EasyToolKit.Inspector;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    public class TerrainObject : SerializedMonoBehaviour
    {
        [OdinSerialize] private Guid _terrainGuid;

        [SerializeField, ReadOnly] private TerrainObjectManager _manager;

        private TerrainDefinition _terrainDefinition;

        public TerrainDefinition TerrainDefinition
        {
            get
            {
                if (_terrainDefinition == null)
                {
                    _terrainDefinition = Manager.Builder.TileWorldAsset.TerrainDefinitionSet.TryGetByGuid(_terrainGuid);
                }
                return _terrainDefinition;
            }
        }

        public TerrainObjectManager Manager => _manager;

        public void Initialize(TerrainObjectManager manager, Guid terrainGuid)
        {
            _manager = manager;
            _terrainGuid = terrainGuid;
        }

        public void Clear()
        {
            var childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = transform.GetChild(0);
                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        public bool AddTile(TilePosition tilePosition, TerrainTileRuleType ruleType)
        {
            var tileInstance = TerrainDefinition.RuleSetAsset.GetTileInstanceByRuleType(ruleType);
            if (tileInstance == null)
            {
                Debug.LogError($"The Rule Type '{ruleType}' of tile instance is null for tile position '{tilePosition}'");
                return false;
            }

            RegisterTile(tileInstance, tilePosition, ruleType);
            return true;
        }

        public bool DestroyTileAt(TilePosition tilePosition)
        {
            foreach (Transform child in transform)
            {
                if (!child.TryGetComponent<TerrainTileObject>(out var tileObject))
                    continue;

                if (tileObject.TerrainObject != this)
                {
                    Debug.LogError($"The tile object '{child.name}' is not belong to this terrain object.");
                    continue;
                }

                if (tileObject.TilePosition == tilePosition)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(child.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(child.gameObject);
                    }
                    return true;
                }
            }
            return false;
        }

        private void RegisterTile(GameObject tileInstance, TilePosition tilePosition, TerrainTileRuleType ruleType)
        {
            tileInstance.transform.SetParent(transform);
            tileInstance.transform.position += Manager.Builder.StartPoint.TilePositionToWorldPosition(tilePosition, Manager.Builder.TileWorldAsset.TileSize);

            var tileObject = tileInstance.AddComponent<TerrainTileObject>();
            tileObject.TerrainObject = this;
            tileObject.RuleType = ruleType;
            tileObject.TilePosition = tilePosition;
        }
    }
}
