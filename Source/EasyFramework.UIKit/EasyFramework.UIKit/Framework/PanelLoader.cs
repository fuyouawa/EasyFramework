using Cysharp.Threading.Tasks;
using EasyFramework.Core;
using UnityEngine;

namespace EasyFramework.UIKit
{
    public interface IPanelLoader
    {
        UniTask<GameObject> LoadPrefabAsync(PanelInfo panelInfo);
        UniTask<GameObject> InstantiateAsync(GameObject panelPrefab);
    }

    public class DefaultPanelLoader : IPanelLoader
    {
        public async UniTask<GameObject> LoadPrefabAsync(PanelInfo panelInfo)
        {
            var path = panelInfo.AssetAddress.DefaultIfNullOrEmpty(panelInfo.PanelType.Name);
            return Resources.Load<GameObject>(path);
        }

        public async UniTask<GameObject> InstantiateAsync(GameObject panelPrefab)
        {
            return GameObject.Instantiate(panelPrefab);
        }
    }
}
