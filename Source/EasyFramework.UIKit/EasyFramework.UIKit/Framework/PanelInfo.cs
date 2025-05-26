using System;

namespace EasyFramework.UIKit
{
    public class PanelInfo
    {
        public Type PanelType { get; }
        public string AssetAddress { get; }
        public UILevel Level { get; }
        public IPanelData Data { get; }
        public PanelOpenType OpenType { get; }

        public PanelInfo(Type panelType, string assetAddress, UILevel level, IPanelData data, PanelOpenType openType)
        {
            PanelType = panelType;
            AssetAddress = assetAddress;
            Level = level;
            Data = data;
            OpenType = openType;
        }
    }
}
