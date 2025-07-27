using System;
using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Inspector;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    public struct TerrainTileBlock
    {
        public Vector3Int TilePosition;
        public TerrainTileDefinition Definition;
    }

    [CreateAssetMenu(menuName = "EasyToolKit/Tilemap/Create TilemapAsset", fileName = "TilemapAsset")]
    [EasyInspector]
    public class TilemapAsset : SerializedScriptableObject
    {
        [FoldoutGroup("设置")]
        [HideLabel]
        [SerializeField] private TilemapSettings _settings = new TilemapSettings();

        [EndFoldoutGroup]
        [MetroListDrawerSettings]
        [LabelText("地形瓦片表")]
        [SerializeField] private List<TerrainTileDefinition> _terrainTileDefinitions = new List<TerrainTileDefinition>();

        [OdinSerialize] private Dictionary<Vector3Int, Guid> _terrainTileMap = new Dictionary<Vector3Int, Guid>();

        public TilemapSettings Settings => _settings;
        public List<TerrainTileDefinition> TerrainTileDefinitions => _terrainTileDefinitions;

        public TerrainTileDefinition TryGetTerrainTileDefinitionByGuid(Guid guid)
        {
            return _terrainTileDefinitions.FirstOrDefault(terrainTile => terrainTile.Guid == guid);
        }

        public void SetTerrainTile(Vector3Int tilePosition, TerrainTileDefinition terrainTileDefinition)
        {
            _terrainTileMap[tilePosition] = terrainTileDefinition.Guid;
        }

        public TerrainTileDefinition TryGetTerrainTile(Vector3Int tilePosition)
        {
            if (_terrainTileMap.TryGetValue(tilePosition, out var guid))
            {
                return TryGetTerrainTileDefinitionByGuid(guid);
            }
            return null;
        }

        public IEnumerable<TerrainTileBlock> EnumerateTerrainTiles()
        {
            foreach (var kvp in _terrainTileMap)
            {
                var tilePosition = kvp.Key;
                var guid = kvp.Value;
                var definition = TryGetTerrainTileDefinitionByGuid(guid);
                if (definition != null)
                {
                    yield return new TerrainTileBlock
                    {
                        TilePosition = tilePosition,
                        Definition = definition,
                    };
                }
            }
        }

        public bool RemoveTerrainTile(Vector3Int tilePosition)
        {
            return _terrainTileMap.Remove(tilePosition);
        }
    }
}
