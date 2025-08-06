// using EasyToolKit.Inspector;
// using System;
// using UnityEngine;

// namespace EasyToolKit.TileWorldPro
// {
//     [Serializable]
//     public class TilemapSettings
//     {
//         [LabelText("地图名称")]
//         [SerializeField] private string _mapName = "TilemapCreator_Map";

//         [LabelText("地基范围")]
//         [SerializeField] private Vector2Int _baseRange = new Vector2Int(20, 20);

//         [LabelText("瓦片大小")]
//         [SerializeField] private float _tileSize = 1f;

//         [LabelText("实时增量构建")]
//         [SerializeField] private bool _realTimeIncrementalBuild = true;

//         [LabelText("最大增量构建深度")]
//         [SerializeField] private int _maxIncrementalBuildDepth = 5;

//         [Title("调试")]
//         [LabelText("绘制调试地基")]
//         [SerializeField] private bool _drawDebugBase = true;

//         [LabelText("地基调试颜色")]
//         [SerializeField] private Color _baseDebugColor = Color.white;

//         [LabelText("显示调试数据")]
//         [SerializeField] private bool _drawDebugData = false;

//         public string MapName => _mapName;

//         public bool RealTimeIncrementalBuild => _realTimeIncrementalBuild;

//         public Vector2Int BaseRange => _baseRange;

//         public bool DrawDebugBase => _drawDebugBase;

//         public Color BaseDebugColor => _baseDebugColor;

//         public int MaxIncrementalBuildDepth => _maxIncrementalBuildDepth;

//         public float TileSize => _tileSize;

//         public bool DrawDebugData => _drawDebugData;
//     }
// }
