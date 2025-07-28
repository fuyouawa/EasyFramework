using EasyToolKit.Inspector;
using System;
using UnityEngine;

namespace EasyToolKit.Tilemap
{
    [Serializable]
    public class TilemapSettings
    {
        [LabelText("地基范围")]
        [SerializeField] private Vector2Int _baseRange = new Vector2Int(20, 20);

        [LabelText("瓦片大小")]
        [SerializeField] private float _tileSize = 1f;

        [Header("调试")]
        [LabelText("地基颜色")]
        [SerializeField] private Color _baseColor = Color.white;

        [LabelText("显示调试数据")]
        [SerializeField] private bool _drawDebugData = false;

        public Vector2Int BaseRange => _baseRange;

        public Color BaseColor => _baseColor;

        public float TileSize => _tileSize;

        public bool DrawDebugData => _drawDebugData;
    }
}
