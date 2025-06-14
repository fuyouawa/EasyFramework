using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EasyFramework.Core;
using UnityEngine;

namespace EasyFramework.UIKit
{
    public class UIManager : MonoSingleton<UIManager>, IUIManager
    {
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

        private readonly Dictionary<Type, IPanel> _panelInstanceByType = new Dictionary<Type, IPanel>();

        public bool HasPanel(Type panelType)
        {
            return _panelInstanceByType.ContainsKey(panelType);
        }

        public async UniTask CreatePanelAsync(Type panelType, GameObject panelPrefab, UILevel level = UILevel.Common)
        {
            if (!panelPrefab.HasComponent(panelType))
            {
                throw new ArgumentException($"Panel prefab does not have component of type {panelType.Name}");
            }

            if (_panelInstanceByType.ContainsKey(panelType))
            {
                throw new InvalidOperationException($"Panel of type {panelType.Name} already exists");
            }

            var logicLevel = GetLogicLevel(level);
            var inst = Instantiate(panelPrefab);
            var panel = (IPanel)inst.GetComponent(panelType);
            panel.Info = new PanelInfo(panelType, panelPrefab, level);
            
            _root.SetPanelLevel(panel, level);
            logicLevel.PushPanel(panel);
            _panelInstanceByType[panelType] = panel;

            if (panel.State == PanelState.Uninitialized)
            {
                await panel.InitializeAsync();
            }
        }

        public IPanel OpenPanel(Type panelType, IPanelData panelData = null, UILevel? level = null)
        {
            if (!_panelInstanceByType.TryGetValue(panelType, out var panel))
            {
                throw new KeyNotFoundException($"Panel of type {panelType.Name} has not been created. Call CreatePanelAsync first.");
            }

            if (panel.State == PanelState.Opened)
            {
                throw new InvalidOperationException($"Panel of type {panelType.Name} is already open.");
            }

            if (level.HasValue && level.Value != panel.Info.Level)
            {
                var logicLevel = GetLogicLevel(panel.Info.Level);
                logicLevel.RemovePanel(panel);

                logicLevel = GetLogicLevel(level.Value);
                logicLevel.PushPanel(panel);

                _root.SetPanelLevel(panel, level.Value);
                panel.Info.Level = level.Value;
            }

            panel.Open(panelData);
            return panel;
        }
    }
}
