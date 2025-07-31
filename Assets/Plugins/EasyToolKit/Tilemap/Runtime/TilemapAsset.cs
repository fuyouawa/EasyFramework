using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [CreateAssetMenu(menuName = "EasyToolKit/Tilemap/Create TilemapAsset", fileName = "TilemapAsset")]
    [EasyInspector]
    public class TilemapAsset : ScriptableObject
    {
        [FoldoutGroup("设置")]
        [HideLabel]
        [SerializeField] private TilemapSettings _settings = new TilemapSettings();

        [EndFoldoutGroup]
        [SerializeField, HideLabel] private TerrainTileMap _terrainTileMap = new TerrainTileMap();

        public TilemapSettings Settings => _settings;
        public TerrainTileMap TerrainTileMap => _terrainTileMap;
    }
}
