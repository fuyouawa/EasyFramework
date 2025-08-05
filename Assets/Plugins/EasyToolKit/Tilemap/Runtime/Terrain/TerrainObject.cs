using System;
using EasyToolKit.Inspector;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    public class TerrainObject : SerializedMonoBehaviour
    {
        [OdinSerialize] private Guid _targetTerrainDefinitionGuid;

        [SerializeField, ReadOnly] private TilemapCreator _tilemapCreator;

        public Guid TargetTerrainDefinitionGuid
        {
            get => _targetTerrainDefinitionGuid;
            set => _targetTerrainDefinitionGuid = value;
        }

        public TilemapCreator TilemapCreator
        {
            get => _tilemapCreator;
            set => _tilemapCreator = value;
        }

        public void ClearTiles()
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

        public bool BuildTile(TerrainTilePosition terrainTilePosition)
        {
            var tilePosition = terrainTilePosition.TilePosition;
            var ruleType = TilemapCreator.Asset.TerrainMap.CalculateRuleTypeAt(TilemapCreator.TerrainConfigAsset, tilePosition);
            var tileInstance = terrainTilePosition.Definition.RuleSetAsset.GetTileInstanceByRuleType(ruleType);
            if (tileInstance == null)
            {
                Debug.LogError($"The Rule Type '{ruleType}' of tile instance is null for tile position '{tilePosition}'");
                return false;
            }

            RegisterTile(tileInstance, tilePosition, ruleType);
            return true;
        }

        public bool DestroyTileAt(Vector3Int tilePosition)
        {
            foreach (Transform child in transform)
            {
                var tileObject = child.GetComponent<TerrainTileObject>();
                if (tileObject == null)
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


        private void RegisterTile(GameObject tileInstance, Vector3Int tilePosition, TerrainTileRuleType ruleType)
        {
            tileInstance.transform.SetParent(transform);
            tileInstance.transform.position += TilemapUtility.TilePositionToWorldPosition(
                TilemapCreator.transform.position,
                tilePosition,
                TilemapCreator.Asset.Settings.TileSize);

            var tileObject = tileInstance.AddComponent<TerrainTileObject>();
            tileObject.TerrainObject = this;
            tileObject.RuleType = ruleType;
            tileObject.TilePosition = tilePosition;
        }
    }
}
