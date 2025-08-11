using System;
using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [Serializable]
    public class TileWorldDesignerSettings
    {
        [LabelText("地基范围")]
        [SerializeField] private Vector2Int _baseRange = new Vector2Int(20, 20);

        [Title("调试")]
        [LabelText("绘制调试地基")]
        [SerializeField] private bool _drawDebugBase = true;

        [LabelText("地基调试颜色")]
        [SerializeField] private Color _baseDebugColor = Color.white;

        [LabelText("显示调试数据")]
        [SerializeField] private bool _drawDebugData = false;

        public Vector2Int BaseRange => _baseRange;

        public bool DrawDebugBase => _drawDebugBase;

        public Color BaseDebugColor => _baseDebugColor;

        public bool DrawDebugData => _drawDebugData;
    }
}