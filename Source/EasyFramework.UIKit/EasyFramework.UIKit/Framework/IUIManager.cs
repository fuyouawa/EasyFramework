using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EasyFramework.UIKit
{
    public interface IUIManager
    {
        UniTask CreatePanelAsync(Type panelType, GameObject panelPrefab, UILevel level = UILevel.Common);

        IPanel OpenPanel(Type panelType, IPanelData panelData = null, UILevel? level = null);
        
        bool HasPanel(Type panelType);
    }
}
