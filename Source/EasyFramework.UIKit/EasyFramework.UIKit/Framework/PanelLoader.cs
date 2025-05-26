using Cysharp.Threading.Tasks;
using UnityEngine;

namespace EasyFramework.UIKit
{
    public interface IPanelLoader
    {
        UniTask<GameObject> LoadAsync(PanelInfo panelInfo);
    }

    public class DefaultPanelLoader : IPanelLoader
    {
        public async UniTask<GameObject> LoadAsync(PanelInfo panelInfo)
        {
            return Resources.Load<GameObject>(panelInfo.AssetAddress);
        }
    }
}
