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

        public bool RealTimeIncrementalBuild => _realTimeIncrementalBuild;
    }
}