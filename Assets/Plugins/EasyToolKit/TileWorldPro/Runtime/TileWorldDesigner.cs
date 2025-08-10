using System;
using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    public class TileWorldDesigner : MonoBehaviour
    {
        [LabelText("起始点")]
        [SerializeField] private TileWorldStartPoint _startPoint;

        [FoldoutBoxGroup("设置")]
        [HideLabel]
        [SerializeField] private TileWorldDesignerSettings _settings;

        [EndFoldoutBoxGroup]
        [LabelText("资产")]
        [SerializeField, InlineEditor] private TileWorldAsset _tileWorldAsset;

        public TileWorldStartPoint StartPoint => _startPoint;

        public TileWorldDesignerSettings Settings => _settings;

        public TileWorldAsset TileWorldAsset => _tileWorldAsset;
    }
}