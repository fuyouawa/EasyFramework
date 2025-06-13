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
        public Component TargetComponent { get; }

        public float TimeToRecycle { get; set; }
        public float TimeToDestroy { get; set; }

        public float ElapsedTime { get; set; }

        public PooledUnityObjectState State { get; set; }

        public PooledUnityObjectData(GameObject instance, IUnityObjectPool owningPool, Component targetComponent)
        {
            Instance = instance;
            OwningPool = owningPool;
            TargetComponent = targetComponent;
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

        public float DefaultTimeToRecycleObject { get; set; } = 0f;
        public float DefaultTimeToDestroyObject { get; set; } = 10f;

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

        public override int ActiveCount => _activeObjectDataByInstance.Count;
        public override int AvailableCount => _availableObjectDatas.Count;

        private readonly Dictionary<GameObject, PooledUnityObjectData> _activeObjectDataByInstance =
            new Dictionary<GameObject, PooledUnityObjectData>();

        private readonly List<PooledUnityObjectData> _availableObjectDatas = new List<PooledUnityObjectData>();


        protected override object TryRentFromAvailable()
        {
            PooledUnityObjectData data;
            if (_availableObjectDatas.Count > 0)
            {
                data = _availableObjectDatas[^1];
                _availableObjectDatas.RemoveAt(_availableObjectDatas.Count - 1);
            }
            else
            {
                var inst = Instantiate();
                data = new PooledUnityObjectData(inst, this, inst.GetComponent(ObjectType))
                {
                    TimeToRecycle = DefaultTimeToRecycleObject,
                    TimeToDestroy = DefaultTimeToDestroyObject,
                };
            }

            _activeObjectDataByInstance.Add(data.Instance, data);

            data.State = PooledUnityObjectState.Avtive;
            data.ElapsedTime = 0f;

            var receivers = data.Instance.GetComponents<IPooledObjectCallbackReceiver>();

            if (receivers.Length > 0)
            {
                foreach (var receiver in receivers)
                {
                    receiver.OnRent(this);
                }
            }

            return data.TargetComponent;
        }

        protected override bool TryReleaseToAvailable(object instance)
        {
            var gameObject = GetGameObject(instance);
            var data = _activeObjectDataByInstance[gameObject];

            data.State = PooledUnityObjectState.Unused;
            data.ElapsedTime = 0;

            _activeObjectDataByInstance.Remove(gameObject);
            _availableObjectDatas.Add(data);

            var receivers = gameObject.GetComponents<IPooledObjectCallbackReceiver>();
            if (receivers.Length > 0)
            {
                foreach (var receiver in receivers)
                {
                    receiver.OnRelease(this);
                }
            }

            return true;
        }

        protected override bool TryRemoveFromActive(object instance)
        {
            var gameObject = GetGameObject(instance);
            return _activeObjectDataByInstance.Remove(gameObject);
        }

        protected override void ShrinkAvailableObjectsToFitCapacity(int shrinkCount)
        {
            _availableObjectDatas.RemoveRange(0, shrinkCount);
        }

        private GameObject GetGameObject(object instance)
        {
            if (instance is Component component)
            {
                return component.gameObject;
            }

            if (instance is GameObject gameObject)
            {
                return gameObject;
            }

            throw new ArgumentException($"Instance must be a '{typeof(GameObject)}' or '{typeof(Component)}'.", nameof(instance));
        }

        void IUnityObjectPool.Update(float deltaTime)
        {
            _tickElapsedTime += deltaTime;
            while (_tickElapsedTime >= TickInterval)
            {
                OnTick(TickInterval);
                _tickElapsedTime -= TickInterval;
            }
        }

        protected GameObject Instantiate()
        {
            var inst = UnityEngine.Object.Instantiate(Original, Transform);

            if (_defaultAddComponentType != null && !inst.HasComponent(_defaultAddComponentType))
            {
                inst.AddComponent(_defaultAddComponentType);
            }

            return inst;
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
            foreach (var data in _activeObjectDataByInstance.Values)
            {
                if (data.TimeToRecycle > 0)
                {
                    data.ElapsedTime += interval;
                    if (data.ElapsedTime >= data.TimeToRecycle)
                    {
                        _pendingRecycleObjects.Add(data);
                    }
                }
            }

            foreach (var data in _availableObjectDatas)
            {
                if (data.TimeToDestroy > 0)
                {
                    data.ElapsedTime += interval;
                    if (data.ElapsedTime >= data.TimeToDestroy)
                    {
                        _pendingDestroyObjects.Add(data);
                    }
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
                    if (!TryRelease(data.Instance))
                    {
                        throw new InvalidOperationException(
                            $"Failed to recycle the specified instance back to pool '{Name}'.");
                    }
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
                    if (data.State == PooledUnityObjectState.Avtive)
                    {
                        _activeObjectDataByInstance.Remove(data.Instance);
                    }
                    else
                    {
                        _availableObjectDatas.Remove(data);
                    }
                }

                _pendingRemoveObjects.Clear();
            }
        }
    }
}
