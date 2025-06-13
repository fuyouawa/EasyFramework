using Sirenix.OdinInspector;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public class PooledUnityObject : MonoBehaviour, IPooledObjectCallbackReceiver
    {
        [ShowInInspector, ReadOnly] private PooledUnityObjectState _state;

        void IPooledObjectCallbackReceiver.OnRent(IObjectPool owningPool)
        {
            _state = PooledUnityObjectState.Avtive;
            gameObject.SetActive(true);
        }

        void IPooledObjectCallbackReceiver.OnRelease(IObjectPool owningPool)
        {
            _state = PooledUnityObjectState.Unused;
            gameObject.SetActive(false);
        }
    }
}
