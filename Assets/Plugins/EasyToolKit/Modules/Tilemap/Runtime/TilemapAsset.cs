using System;
using System.Collections.Generic;
using EasyToolKit.Inspector;
using EasyToolKit.ThirdParty.OdinSerializer;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
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
        [SerializeField] private List<TerrainTile> _terrainTiles = new List<TerrainTile>();

        [OdinSerialize] private Dictionary<Vector3Int, Guid> _terrainTileMap = new Dictionary<Vector3Int, Guid>();

        public TilemapSettings Settings => _settings;
        public List<TerrainTile> TerrainTiles => _terrainTiles;
    }
}
