using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyFramework.UIKit
{
    public class UILogicLevel
    {
        private readonly List<IPanel> _openedPanels = new List<IPanel>();

        public UILevel Level { get; }
        public IReadOnlyList<IPanel> OpenedPanels => _openedPanels;
        
        public UILogicLevel(UILevel level)
        {
            Level = level;
        }

        public void PushPanel(IPanel panel)
        {
            if (_openedPanels.Count > 1)
            {
                panel.Order = _openedPanels.Last().Order + 1;
            }
            else
            {
                panel.Order = (int)Level;
            }
            _openedPanels.Add(panel);
        }

        public void PopPanel(IPanel panel)
        {
            _openedPanels.Remove(panel);
        }

        public IPanel[] FindPanelsByType(Type panelType)
        {
            return _openedPanels.Where(panel => panel.Info.PanelType == panelType).ToArray();
        }

        public IPanel FindFirstPanelByType(Type panelType)
        {
            return _openedPanels.FirstOrDefault(panel => panel.Info.PanelType == panelType);
        }
    }
}
