using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EasyFramework.Core;
using UnityEngine;

namespace EasyFramework.UIKit
{
    public class UIManager : MonoSingleton<UIManager>
    {
        public static IPanelLoader DefaultPanelLoader = new DefaultPanelLoader();

        [SerializeField] private UIRoot _root;
        private readonly UILogicLevel[] _logicLevels;

        public UIRoot Root => _root;

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

        public async UniTask<T> OpenPanelAsync<T>(
            string assetAddress = null,
            UILevel level = UILevel.Common,
            IPanelData panelData = null,
            PanelOpenType panelOpenType = PanelOpenType.Single,
            IPanelLoader panelLoader = null)
            where T : class, IPanel
        {
            var panel = await OpenPanelAsync(typeof(T), assetAddress, level, panelData, panelOpenType, panelLoader);
            return panel as T;
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
                panel = logicLevel.FindFirstPanelByType(panelType);

                if (panel != null)
                {
                    if (panel.State == PanelState.Opened)
                    {
                        Debug.LogWarning($"Panel '{panelType}' has been opened.");
                        return panel;
                    }

                    Assert.True(panel.State != PanelState.Killed);
                }
            }

            if (panel == null)
            {
                panelLoader ??= DefaultPanelLoader;
                var prefab = await panelLoader.LoadPrefabAsync(info);
                var inst = await panelLoader.InstantiateAsync(prefab);
                panel = inst.GetComponent<IPanel>();
            }

            panel.Info = info;
            _root.SetPanelLevel(panel, level);
            logicLevel.PushPanel(panel);

            if (panel.State == PanelState.Uninitialized)
            {
                await panel.InitializeAsync();
            }

            panel.Open(panelData);
            return panel;
        }
    }
}
