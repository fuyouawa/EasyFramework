using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using EasyFramework.Core;
using UnityEngine;

namespace EasyFramework.ToolKit
{
    public enum PooledUnityObjectState
    {
        Avtive,
        Unused,
    }

    class PooledUnityObjectData
    {
        public GameObject Instance { get; }
        public IUnityObjectPool OwningPool { get; }

        /// <summary>
        /// 生命时间，小于等于0则代表无限生命时间（直到手动回收）
        /// </summary>
        public float Lifetime { get; set; }

        public float RemainingLifetime { get; set; }

        public PooledUnityObjectState State { get; set; }

        public PooledUnityObjectData(GameObject instance, IUnityObjectPool owningPool)
        {
            Instance = instance;
            OwningPool = owningPool;
        }
    }

    /// <summary>
    /// Unity对象池实现，用于管理Unity游戏对象的对象池
    /// </summary>
    public class UnityObjectPool : ObjectPoolBase, IUnityObjectPool
    {
        [SerializeField] private GameObject _original;

        /// <summary>
        /// 对象池中对象的原始预制体
        /// </summary>
        public GameObject Original => _original;

        GameObject IUnityObjectPool.Original
        {
            get => _original;
            set
            {
                if (value.GetComponent(ObjectType) == null)
                {
                    throw new InvalidOperationException(
                        $"The original prefab '{value.name}' must contain component of type '{ObjectType}'.");
                }

                _original = value;
            }
        }

        [SerializeField] private Transform _transform;

        /// <inheritdoc />
        public Transform Transform
        {
            get => _transform;
            set => _transform = value;
        }

        /// <summary>
        /// 对象池中对象的默认生命周期（秒）
        /// </summary>
        public float DefaultObjectLifetime { get; set; } = 0f;

        /// <summary>
        /// 更新间隔
        /// </summary>
        public float TickInterval { get; set; } = 0.5f;
        
        private Type _defaultAddComponentType = typeof(PooledUnityObject);

        /// <summary>
        /// <para>对象池中对象默认添加的组件类型</para>
        /// <para>如果为null代表不添加默认组件</para>
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// 当value不为空，并且不是Component的子类或者是抽象类、接口时抛出
        /// </exception>
        public Type DefaultAddComponentType
        {
            get => _defaultAddComponentType;
            set
            {
                if (value == null)
                {
                    _defaultAddComponentType = null;
                    return;
                }

                if (!typeof(Component).IsAssignableFrom(value))
                {
                    throw new InvalidOperationException($"Type '{value}' must inherit from '{typeof(Component)}'.");
                }

                if (value.IsAbstract || value.IsInterface)
                {
                    throw new InvalidOperationException($"Type '{value}' cannot be abstract or interface.");
                }

                _defaultAddComponentType = value;
            }
        }

        private float _tickElapsedTime;

        public override bool TryRecycle(object instance)
        {
            if (instance is Component component)
            {
                instance = component.GetComponent(ObjectType);
            }
            else if (instance is GameObject gameObject)
            {
                instance = gameObject.GetComponent(ObjectType);
            }
            else
            {
                //TODO 异常
            }

            return base.TryRecycle(instance);
        }

        private readonly Dictionary<GameObject, PooledUnityObjectData> _objectDatasByInstance = new Dictionary<GameObject, PooledUnityObjectData>();

        void IUnityObjectPool.Update(float deltaTime)
        {
            _tickElapsedTime += deltaTime;
            while (_tickElapsedTime >= TickInterval)
            {
                OnTick(TickInterval);
                _tickElapsedTime -= TickInterval;
            }
        }

        /// <inheritdoc />
        protected override object GetNewObject()
        {
            var inst = UnityEngine.Object.Instantiate(Original);
            ProcessInstance(inst);
            return inst.GetComponent(ObjectType);
        }

        /// <summary>
        /// 处理新创建的游戏对象实例
        /// </summary>
        /// <param name="instance">新创建的游戏对象实例</param>
        protected virtual void ProcessInstance(GameObject instance)
        {
            var data = new PooledUnityObjectData(instance, this)
            {
                Lifetime = DefaultObjectLifetime,
                RemainingLifetime = DefaultObjectLifetime
            };

            if (!_objectDatasByInstance.TryAdd(instance, data))
            {
                //TODO 异常
            }

            instance.transform.SetParent(Transform);
            if (_defaultAddComponentType != null && !instance.HasComponent(_defaultAddComponentType))
            {
                instance.AddComponent(_defaultAddComponentType);
            }
        }

        /// <inheritdoc />
        protected override void OnSpawn(object instance)
        {
            var gameObject = ((Component)instance).gameObject;
            _objectDatasByInstance[gameObject].State = PooledUnityObjectState.Avtive;

            var receivers = gameObject.GetComponents<IPooledObjectCallbackReceiver>();

            if (receivers.Length > 0)
            {
                foreach (var receiver in receivers)
                {
                    receiver.OnSpawn(this);
                }
            }
        }

        /// <inheritdoc />
        protected override void OnRecycle(object instance)
        {
            var gameObject = ((Component)instance).gameObject;
            _objectDatasByInstance[gameObject].State = PooledUnityObjectState.Unused;
            
            var receivers = gameObject.GetComponents<IPooledObjectCallbackReceiver>();
            if (receivers.Length > 0)
            {
                foreach (var receiver in receivers)
                {
                    receiver.OnRecycle(this);
                }
            }
        }

        private readonly List<PooledUnityObjectData> _pendingRemoveObjects = new List<PooledUnityObjectData>();
        private readonly List<PooledUnityObjectData> _pendingRecycleObjects = new List<PooledUnityObjectData>();
        private readonly List<PooledUnityObjectData> _pendingDestroyObjects = new List<PooledUnityObjectData>();

        /// <summary>
        /// 定时更新所有活跃对象的状态
        /// </summary>
        /// <param name="interval">更新间隔（秒）</param>
        protected virtual void OnTick(float interval)
        {
            foreach (var data in _objectDatasByInstance.Values)
            {
                if (data.Instance == null)
                {
                    _pendingRemoveObjects.Add(data);
                    continue;
                }

                if (data.Lifetime > 0 && data.State == PooledUnityObjectState.Avtive)
                {
                    data.RemainingLifetime -= interval;
                    if (data.RemainingLifetime <= 0)
                    {
                        _pendingRecycleObjects.Add(data);
                        data.RemainingLifetime = data.Lifetime;
                    }
                }

                if (data.State == PooledUnityObjectState.Unused)
                {
                    //TODO 销毁不在使用的实例（类似Lifetime，但计时到了后直接加入_pendingDestroyObjects）
                }
            }

            DoPendingRecycleObjects();
            DoPendingDestroyObjects();
            DoPendingRemoveObjects();
        }

        private void DoPendingRecycleObjects()
        {
            if (_pendingRecycleObjects.Count > 0)
            {
                foreach (var data in _pendingRecycleObjects)
                {
                    if (!TryRecycle(data.Instance))
                    {
                        _pendingDestroyObjects.Add(data);
                    }

                    data.State = PooledUnityObjectState.Unused;
                }

                _pendingRecycleObjects.Clear();
            }
        }

        private void DoPendingDestroyObjects()
        {
            if (_pendingDestroyObjects.Count > 0)
            {
                foreach (var data in _pendingDestroyObjects)
                {
                    UnityEngine.Object.Destroy(data.Instance);
                    _pendingRemoveObjects.Add(data);

                    data.State = PooledUnityObjectState.Unused;
                }

                _pendingDestroyObjects.Clear();
            }
        }

        private void DoPendingRemoveObjects()
        {
            if (_pendingRemoveObjects.Count > 0)
            {
                foreach (var data in _pendingRemoveObjects)
                {
                    _objectDatasByInstance.Remove(data.Instance);

                    data.State = PooledUnityObjectState.Unused;
                }

                _pendingRemoveObjects.Clear();
            }
        }
    }
}
