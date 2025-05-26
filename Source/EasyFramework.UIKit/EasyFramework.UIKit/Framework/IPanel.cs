using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EasyFramework.UIKit
{
    public interface IPanelData {}

    public enum PanelState
    {
        Initializing,
        Opening,
        Closed,
        Killed
    }

    public interface IPanel
    {
        Transform Transform { get; }
        PanelState State { get; }
        int Order { get; internal set; }
        
        UniTask InitializeAsync();

        void Open(IPanelData panelData);

        void Close();

        void Kill();
    }
}
