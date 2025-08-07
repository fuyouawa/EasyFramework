using System;
using EasyToolKit.Inspector;
using UnityEngine;

namespace EasyToolKit.TileWorldPro
{
    [Serializable]
    public class TileWorldBuilderSettings
    {
        [LabelText("实时增量构建")]
        [SerializeField] private bool _realTimeIncrementalBuild = true;

        [LabelText("最大增量构建深度")]
        [SerializeField] private int _maxIncrementalBuildDepth = 5;

        public bool RealTimeIncrementalBuild => _realTimeIncrementalBuild;

        public int MaxIncrementalBuildDepth => _maxIncrementalBuildDepth;
    }
}