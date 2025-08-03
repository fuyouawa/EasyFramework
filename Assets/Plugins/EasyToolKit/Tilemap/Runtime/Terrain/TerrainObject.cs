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
            var ruleType = TilemapCreator.Asset.TerrainMap.CalculateRuleTypeAt(tilePosition);
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
            }
            return false;
        }


        private void RegisterTile(GameObject tileInstance, Vector3Int tilePosition, TerrainTileRuleType ruleType)
        {
            tileInstance.transform.SetParent(transform);
            tileInstance.transform.position += TilemapCreator.TilePositionToWorldPosition(tilePosition);
        }
    }
}
