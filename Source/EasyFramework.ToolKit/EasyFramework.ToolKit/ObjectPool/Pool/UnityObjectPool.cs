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
        Idle,
    }

    class PooledUnityInstance
    {
        /// <summary>
        /// 目标实例
        /// </summary>
        public GameObject Target { get; }

        /// <summary>
        /// 所属池
        /// </summary>
        public IUnityObjectPool OwningPool { get; }

        /// <summary>
        /// 目标组件（用于快速返回）
        /// </summary>
        public Component TargetComponent { get; }

        /// <summary>
        /// 激活中对象的回收时间，小于0则无限制
        /// </summary>
        public float ActiveLifetime { get; set; }
        /// <summary>
        /// 空闲中对象的销毁时间，小于0则无限制
        /// </summary>
        public float IdleLifetime { get; set; }

        /// <summary>
        /// 计时器
        /// </summary>
        public float ElapsedTime { get; set; }

        public PooledUnityObjectState State { get; set; }

        public PooledUnityInstance(GameObject target, IUnityObjectPool owningPool, Component targetComponent)
        {
            Target = target;
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

        private Type _defaultComponentType = typeof(PooledUnityObjectAutoActivator);

        /// <summary>
        /// <para>为对象池中对象默认添加的组件类型。</para>
        /// <para>如果为null代表不添加默认组件。</para>
        /// </summary>
        public Type DefaultComponentType
        {
            get => _defaultComponentType;
            set
            {
                if (value == null)
                {
                    _defaultComponentType = null;
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

                _defaultComponentType = value;
            }
        }

        private float _tickElapsedTime;

        public override int ActiveCount => _activeInstanceDict.Count;
        public override int IdleCount => _idleInstances.Count;

        private readonly Dictionary<GameObject, PooledUnityInstance> _activeInstanceDict =
            new Dictionary<GameObject, PooledUnityInstance>();

        private readonly List<PooledUnityInstance> _idleInstances = new List<PooledUnityInstance>();


        protected override object TryRentFromIdle()
        {
            PooledUnityInstance data;
            if (_idleInstances.Count > 0)
            {
                data = _idleInstances[^1];
                _idleInstances.RemoveAt(_idleInstances.Count - 1);
            }
            else
            {
                var inst = Instantiate();
                data = new PooledUnityInstance(inst, this, inst.GetComponent(ObjectType))
                {
                    ActiveLifetime = DefaultTimeToRecycleObject,
                    IdleLifetime = DefaultTimeToDestroyObject,
                };
            }

            _activeInstanceDict.Add(data.Target, data);

            data.State = PooledUnityObjectState.Avtive;
            data.ElapsedTime = 0f;

            var receivers = data.Target.GetComponents<IPoolCallbackReceiver>();

            if (receivers.Length > 0)
            {
                foreach (var receiver in receivers)
                {
                    receiver.OnRent(this);
                }
            }

            return data.TargetComponent;
        }

        protected override bool TryReleaseToIdle(object instance)
        {
            var gameObject = GetGameObject(instance);
            var data = _activeInstanceDict[gameObject];

            data.State = PooledUnityObjectState.Idle;
            data.ElapsedTime = 0;

            _activeInstanceDict.Remove(gameObject);
            _idleInstances.Add(data);

            var receivers = gameObject.GetComponents<IPoolCallbackReceiver>();
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
            return _activeInstanceDict.Remove(gameObject);
        }

        protected override void ShrinkIdleObjectsToFitCapacity(int shrinkCount)
        {
            _idleInstances.RemoveRange(0, shrinkCount);
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

            if (_defaultComponentType != null && !inst.HasComponent(_defaultComponentType))
            {
                inst.AddComponent(_defaultComponentType);
            }

            return inst;
        }


        private readonly List<PooledUnityInstance> _pendingRemoveInstances = new List<PooledUnityInstance>();
        private readonly List<PooledUnityInstance> _pendingRecycleInstances = new List<PooledUnityInstance>();
        private readonly List<PooledUnityInstance> _pendingDestroyInstances = new List<PooledUnityInstance>();

        /// <summary>
        /// 定时更新所有活跃对象的状态
        /// </summary>
        /// <param name="interval">更新间隔（秒）</param>
        protected virtual void OnTick(float interval)
        {
            foreach (var data in _activeInstanceDict.Values)
            {
                if (data.ActiveLifetime > 0)
                {
                    data.ElapsedTime += interval;
                    if (data.ElapsedTime >= data.ActiveLifetime)
                    {
                        _pendingRecycleInstances.Add(data);
                    }
                }
            }

            foreach (var data in _idleInstances)
            {
                if (data.IdleLifetime > 0)
                {
                    data.ElapsedTime += interval;
                    if (data.ElapsedTime >= data.IdleLifetime)
                    {
                        _pendingDestroyInstances.Add(data);
                    }
                }
            }

            DoPendingRecycleObjects();
            DoPendingDestroyObjects();
            DoPendingRemoveObjects();
        }

        private void DoPendingRecycleObjects()
        {
            if (_pendingRecycleInstances.Count > 0)
            {
                foreach (var data in _pendingRecycleInstances)
                {
                    if (!TryRelease(data.Target))
                    {
                        throw new InvalidOperationException(
                            $"Failed to recycle the specified instance back to pool '{Name}'.");
                    }
                }

                _pendingRecycleInstances.Clear();
            }
        }

        private void DoPendingDestroyObjects()
        {
            if (_pendingDestroyInstances.Count > 0)
            {
                foreach (var data in _pendingDestroyInstances)
                {
                    UnityEngine.Object.Destroy(data.Target);
                    _pendingRemoveInstances.Add(data);
                }

                _pendingDestroyInstances.Clear();
            }
        }

        private void DoPendingRemoveObjects()
        {
            if (_pendingRemoveInstances.Count > 0)
            {
                foreach (var data in _pendingRemoveInstances)
                {
                    if (data.State == PooledUnityObjectState.Avtive)
                    {
                        _activeInstanceDict.Remove(data.Target);
                    }
                    else
                    {
                        _idleInstances.Remove(data);
                    }
                }

                _pendingRemoveInstances.Clear();
            }
        }
    }
}
