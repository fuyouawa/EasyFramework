using System;
using UnityEngine;

namespace EasyFramework.UIKit
{
    public class PanelInfo
    {
        public Type PanelType { get; }
        public GameObject PanelPrefab { get; }
        public UILevel Level { get; set; }

        public PanelInfo(Type panelType, GameObject panelPrefab, UILevel level)
        {
            PanelType = panelType;
            PanelPrefab = panelPrefab;
            Level = level;
        }
    }
}
