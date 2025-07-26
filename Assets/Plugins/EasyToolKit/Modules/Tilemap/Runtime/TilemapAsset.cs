using System;
using System.Collections.Generic;
using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [Serializable]
    public class TilemapSettings
    {
        [LabelText("地基大小")]
        public Vector2 BaseSize = new Vector2(20, 20);
    }

    [CreateAssetMenu(menuName = "EasyToolKit/Tilemap/Create TilemapAsset", fileName = "TilemapAsset")]
    [EasyInspector]
    public class TilemapAsset : ScriptableObject
    {
        [FoldoutGroup("设置")]
        [HideLabel]
        public TilemapSettings Settings = new TilemapSettings();

        [EndFoldoutGroup]

        [MetroListDrawerSettings]
        [LabelText("地形瓦片数据表")]
        public List<TerrainTileData> TerrainTileDatas = new List<TerrainTileData>();
    }
}
