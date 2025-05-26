using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EasyFramework.Core;

namespace EasyFramework.UIKit
{
    public class UIManager : MonoSingleton<UIManager>
    {
        public static IPanelLoader DefaultPanelLoader = new DefaultPanelLoader();
        
        private readonly UILogicLevel[] _logicLevels;

        private UIManager()
        {
            _logicLevels = new[]
            {
                new UILogicLevel(UILevel.Lowest),
                new UILogicLevel(UILevel.Background),
                new UILogicLevel(UILevel.Common),
                new UILogicLevel(UILevel.PopUI),
                new UILogicLevel(UILevel.Topest),
            };
        }

        public UILogicLevel GetLogicLevel(UILevel level)
        {
            return level switch
            {
                UILevel.Lowest => _logicLevels[0],
                UILevel.Background => _logicLevels[1],
                UILevel.Common => _logicLevels[2],
                UILevel.PopUI => _logicLevels[3],
                UILevel.Topest => _logicLevels[4],
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, null)
            };
        }

        public async UniTask<IPanel> OpenPanelAsync(
            Type panelType,
            string assetAddress = null,
            UILevel level = UILevel.Common,
            IPanelData panelData = null,
            PanelOpenType panelOpenType = PanelOpenType.Single,
            IPanelLoader panelLoader = null)
        {
            var info = new PanelInfo(panelType, assetAddress, level, panelData, panelOpenType);
            var logicLevel = GetLogicLevel(level);

            IPanel panel = null;
            if (panelOpenType == PanelOpenType.Single)
            {
                panel = logicLevel.FindFirstPanel(panelType);
            }

            if (panel == null)
            {
                panelLoader ??= DefaultPanelLoader;
                var inst = await panelLoader.LoadAsync(info);
                panel = inst.GetComponent<IPanel>();
                logicLevel.PushPanel(panel);
                await panel.InitializeAsync();
            }

            panel.Open(panelData);
            return panel;
        }
    }
}
