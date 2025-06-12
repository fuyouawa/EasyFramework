using System;
using UnityEngine;

namespace EasyFramework.ToolKit
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

        private float _elapsedTime;

        public Transform Transform => transform;

        public void Tick(float interval)
        {
            _elapsedTime += interval;

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
