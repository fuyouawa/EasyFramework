using System;
using Cysharp.Threading.Tasks;
using EasyFramework.Core;
using UnityEngine;

namespace EasyFramework.UIKit
{
    public abstract class UIPanel : MonoBehaviour, IPanel
    {
        protected UILevel Level
        {
            get
            {
                if (Order < (int)UILevel.Background)
                {
                    return UILevel.Lowest;
                }

                if (Order < (int)UILevel.Common)
                {
                    return UILevel.Background;
                }
                
                if (Order < (int)UILevel.PopUI)
                {
                    return UILevel.Common;
                }
                
                if (Order < (int)UILevel.Topest)
                {
                    return UILevel.PopUI;
                }
                
                return UILevel.Topest;
            }
        }
        protected int Order { get; private set; }
        protected PanelState PanelState { get; private set; }
        protected UIManager Manager => UIManager.Instance;

        Transform IPanel.Transform => transform;
        PanelState IPanel.State => PanelState;

        int IPanel.Order
        {
            get => Order;
            set => Order = value;
        }

        UniTask IPanel.InitializeAsync()
        {
            PanelState = PanelState.Initializing;
            return OnInitAsync();
        }

        void IPanel.Open(IPanelData panelData)
        {
            PanelState = PanelState.Opening;
            OnOpen(panelData);
        }

        void IPanel.Close()
        {
            if (PanelState == PanelState.Closed || PanelState == PanelState.Killed)
                return;

            OnClose();
            PanelState = PanelState.Closed;
        }

        void IPanel.Kill()
        {
            if (PanelState == PanelState.Killed)
                return;
            Assert.True(PanelState == PanelState.Closed);

            OnKill();
            Destroy(gameObject);
            PanelState = PanelState.Killed;
        }

        protected virtual UniTask OnInitAsync()
        {
            return UniTask.CompletedTask;
        }

        protected virtual void OnOpen(IPanelData panelData)
        {
        }

        protected virtual void OnClose()
        {
        }

        protected virtual void OnKill()
        {
        }
    }
}
