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
        [SerializeField, HideLabel] private TerrainMap _terrainMap = new TerrainMap();

        public TilemapSettings Settings => _settings;
        public TerrainMap TerrainMap => _terrainMap;
    }
}
