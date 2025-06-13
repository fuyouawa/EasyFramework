using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    /// <summary>
    /// 用于自动处理 Unity 对象在对象池中的激活/隐藏逻辑的组件。
    /// 当对象从池中取出时自动激活 GameObject，回收时自动隐藏，并更新当前状态。
    /// </summary>
    public class PooledUnityObjectAutoActivator : MonoBehaviour, IPoolCallbackReceiver
    {
        [ShowInInspector, ReadOnly] private PooledUnityObjectState _state;

        void IPoolCallbackReceiver.OnRent(IObjectPool owningPool)
        {
            _state = PooledUnityObjectState.Avtive;
            gameObject.SetActive(true);
        }

        void IPoolCallbackReceiver.OnRelease(IObjectPool owningPool)
        {
            _state = PooledUnityObjectState.Idle;
            gameObject.SetActive(false);
        }
    }
}
