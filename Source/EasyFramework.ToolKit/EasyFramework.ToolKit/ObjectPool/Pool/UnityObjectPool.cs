using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using EasyFramework.Core;
using UnityEngine;

namespace EasyFramework.ToolKit
{
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

        private Type _defaultPooledComponentType = typeof(PooledUnityObject);

        /// <summary>
        /// 对象池中对象的默认组件类型
        /// </summary>
        /// <exception cref="ArgumentNullException">当value为null时抛出</exception>
        /// <exception cref="InvalidOperationException">
        /// 当value不是Component的子类，
        /// 或者未实现IPooledUnityObject接口，
        /// 或者是抽象类或接口时抛出
        /// </exception>
        public Type DefaultPooledComponentType
        {
            get => _defaultPooledComponentType;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), $"Default pooled component type cannot be null.");
                }

                if (!typeof(Component).IsAssignableFrom(value))
                {
                    throw new InvalidOperationException($"Type '{value}' must inherit from '{typeof(Component)}'.");
                }

                if (!typeof(IPooledUnityObject).IsAssignableFrom(value))
                {
                    throw new InvalidOperationException(
                        $"Type '{value}' must implement '{typeof(IPooledUnityObject)}'.");
                }

                if (value.IsAbstract || value.IsInterface)
                {
                    throw new InvalidOperationException($"Type '{value}' cannot be abstract or interface.");
                }

                _defaultPooledComponentType = value;
            }
        }

        /// <summary>
        /// 对象回收检查的时间间隔（秒）
        /// </summary>
        public float RecycleInterval { get; set; } = 0.5f;

        private float _recycleElapsedTime;

        public override bool TryRecycle(object instance)
        {
            var type = instance.GetType();
            if (!typeof(Component).IsAssignableFrom(type))
            {
                throw new InvalidOperationException(
                    $"The type '{type}' of instance '{instance}' must inherit from '{typeof(Component)}'.");
            }

            if (type != ObjectType)
            {
                instance = ((Component)instance).GetComponent(ObjectType);
            }

            return base.TryRecycle(instance);
        }

        // 通过GameObject引用存储活跃的IPooledUnityObject实例
        private readonly Dictionary<GameObject, IPooledUnityObject> _activeInstancesByGameObject =
            new Dictionary<GameObject, IPooledUnityObject>();

        void IUnityObjectPool.Update(float deltaTime)
        {
            _recycleElapsedTime += deltaTime;
            while (_recycleElapsedTime >= RecycleInterval)
            {
                OnTick(RecycleInterval);
                _recycleElapsedTime -= RecycleInterval;
            }
        }

        /// <inheritdoc />
        protected override object GetNewObject()
        {
            var inst = UnityEngine.Object.Instantiate(Original);
            ProcessInstance(inst);
            return inst.GetComponent(ObjectType);
        }

        // /// <inheritdoc />
        // protected override async UniTask<object> GetNewObjectAsync()
        // {
        //     return GetNewObject();
        // }

        /// <summary>
        /// 处理新创建的游戏对象实例
        /// </summary>
        /// <param name="instance">新创建的游戏对象实例</param>
        /// <exception cref="InvalidOperationException">当instance包含多个IPooledUnityObject组件时抛出</exception>
        protected virtual void ProcessInstance(GameObject instance)
        {
            var comps = instance.GetComponents<IPooledUnityObject>();

            if (comps.Length > 1)
            {
                throw new InvalidOperationException(
                    $"GameObject '{instance.name}' cannot contain more than one component implementing '{typeof(IPooledUnityObject)}'.");
            }

            IPooledUnityObject pooledComponent;

            if (comps.Length == 0)
            {
                pooledComponent = (IPooledUnityObject)instance.AddComponent(_defaultPooledComponentType);
            }
            else
            {
                pooledComponent = comps[0];
            }

            // 设置默认生命周期
            pooledComponent.Lifetime = DefaultObjectLifetime;
        }

        /// <inheritdoc />
        protected override void OnSpawn(object instance)
        {
            var gameObject = ((Component)instance).gameObject;
            var obj = gameObject.GetComponent<IPooledUnityObject>();
            obj.OwningPool = this;
            _activeInstancesByGameObject[gameObject] = obj;
            obj.OnSpawn();
        }

        /// <inheritdoc />
        protected override void OnRecycle(object instance)
        {
            var gameObject = ((Component)instance).gameObject;
            var obj = gameObject.GetComponent<IPooledUnityObject>();
            obj.OwningPool = this;
            _activeInstancesByGameObject.Remove(gameObject);
            obj.OnRecycle();
        }

        /// <summary>
        /// 定时更新所有活跃对象的状态
        /// </summary>
        /// <param name="interval">更新间隔（秒）</param>
        protected virtual void OnTick(float interval)
        {
            // 复制一份防止Tick时变更
            var temp = _activeInstancesByGameObject.Values.ToArray();

            foreach (var o in temp)
            {
                o.Tick(interval);
            }
        }
    }
}
