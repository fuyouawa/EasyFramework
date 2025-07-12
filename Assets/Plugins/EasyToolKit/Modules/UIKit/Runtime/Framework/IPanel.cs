using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EasyToolKit.UIKit
{
    public interface IPanelData {}

    public enum PanelState
    {
        Uninitialized,
        Initializing,
        Initialized,
        Opened,
        Closed,
        Killed
    }

    public interface IPanel
    {
        Transform Transform { get; }
        PanelState State { get; }
        int Order { get; internal set; }
        PanelInfo Info { get; internal set; }
        
        UniTask InitializeAsync();
        
        void Open(IPanelData panelData);

        void Close();

        void Kill();
    }
}
