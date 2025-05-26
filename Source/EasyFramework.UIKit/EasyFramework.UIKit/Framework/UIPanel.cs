using System;
using Cysharp.Threading.Tasks;
using EasyFramework.Core;
using UnityEngine;

namespace EasyFramework.UIKit
{
    public abstract class UIPanel : MonoBehaviour, IPanel
    {
        protected PanelState PanelState { get; set; }
        Transform IPanel.Transform => transform;
        PanelState IPanel.State => PanelState;

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
