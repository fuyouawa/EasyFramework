using System;
using UnityEngine;

namespace EasyFramework.Core
{
    public class PooledUnityObject : MonoBehaviour, IPooledUnityObject
    {
        IObjectPool IPooledObject.OwningPool
        {
            get => _owningPool;
            set
            {
                if (value is IUnityObjectPool pool)
                {
                    _owningPool = pool;
                }
                else
                {
                    throw new InvalidCastException($"OwningPool must implement '{typeof(IUnityObjectPool)}'.");
                }
            }
        }

        private IUnityObjectPool _owningPool;

        public float Lifetime { get; set; }

        // 已经流逝的时间
        private float _elapsedTime;

        public Transform Transform => transform;

        public void Tick(float interval)
        {
            // 增加流逝时间
            _elapsedTime += interval;

            // 当达到生命周期时回收
            if (_elapsedTime >= Lifetime)
            {
                Recycle();
            }
        }

        private void Recycle()
        {
            if (!_owningPool.TryRecycle(this))
            {
                Destroy(gameObject);
            }
        }

        void IPooledObject.OnSpawn()
        {
            // 重置计时器
            _elapsedTime = 0f;

            transform.SetParent(_owningPool.Transform);
            gameObject.SetActive(true);
        }

        void IPooledObject.OnRecycle()
        {
            transform.SetParent(_owningPool.Transform);
            gameObject.SetActive(false);
        }
    }
}
