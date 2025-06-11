using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EasyFramework.Core
{
    public class UnityObjectPool : ObjectPoolBase, IUnityObjectPool
    {
        public GameObject Original { get; }
        public Transform Transform { get; set; }
        public float DefaultObjectLifetime { get; set; } = 1f;

        private Type _defaultPooledComponentType;

        public Type DefaultPooledComponentType
        {
            get => _defaultPooledComponentType;
            set
            {
                // 检查并确保类型有效
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), $"Default pooled component type cannot be null.");
                }

                // 必须继承自 UnityEngine.Component
                if (!typeof(Component).IsAssignableFrom(value))
                {
                    throw new InvalidOperationException($"Type '{value}' must inherit from '{typeof(Component)}'.");
                }

                // 必须实现 IPooledUnityObject 接口
                if (!typeof(IPooledUnityObject).IsAssignableFrom(value))
                {
                    throw new InvalidOperationException($"Type '{value}' must implement '{typeof(IPooledUnityObject)}'.");
                }

                // 不允许抽象类或接口
                if (value.IsAbstract || value.IsInterface)
                {
                    throw new InvalidOperationException($"Type '{value}' cannot be abstract or interface.");
                }

                _defaultPooledComponentType = value;
            }
        }

        public float RecycleInterval { get; set; } = 0.5f;

        public UnityObjectPool(string name, Type objectType, GameObject original)
            : base(name, objectType)
        {
            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));
            if (original == null)
                throw new ArgumentNullException(nameof(original));

            // 校验 original 上是否挂载了目标组件
            if (original.GetComponent(objectType) == null)
            {
                throw new InvalidOperationException($"Original prefab '{original.name}' must contain component of type '{objectType}'.");
            }

            Original = original;

            // 默认使用框架提供的 PooledUnityObject 类型，防止未显式设置导致空引用
            _defaultPooledComponentType = typeof(PooledUnityObject);
        }

        private float _recycleElapsedTime;

        private readonly Dictionary<GameObject, IPooledUnityObject> _activeInstancesByGameObject = new Dictionary<GameObject, IPooledUnityObject>();

        void IUnityObjectPool.Update(float deltaTime)
        {
            _recycleElapsedTime += deltaTime;
            while (_recycleElapsedTime >= RecycleInterval)
            {
                OnTick(RecycleInterval);
                _recycleElapsedTime -= RecycleInterval;
            }
        }

        protected override object GetNewObject()
        {
            var inst = UnityEngine.Object.Instantiate(Original);
            ProcessInstance(inst);
            return inst.GetComponent(ObjectType);
        }

        protected override async UniTask<object> GetNewObjectAsync()
        {
            return GetNewObject();
        }

        protected virtual void ProcessInstance(GameObject instance)
        {
            // 获取所有 IPooledUnityObject 组件
            var comps = instance.GetComponents<IPooledUnityObject>();

            // 若超过 1 个，说明使用者手动挂载了多个，抛异常提醒
            if (comps.Length > 1)
            {
                throw new InvalidOperationException($"GameObject '{instance.name}' cannot contain more than one component implementing '{typeof(IPooledUnityObject)}'.");
            }

            IPooledUnityObject pooledComponent;

            if (comps.Length == 0)
            {
                // 自动添加默认实现
                pooledComponent = (IPooledUnityObject)instance.AddComponent(_defaultPooledComponentType);
            }
            else
            {
                pooledComponent = comps[0];
            }

            // 设置默认生命周期
            pooledComponent.Lifetime = DefaultObjectLifetime;
        }

        protected override void OnSpawn(object instance)
        {
            var gameObject = ((Component)instance).gameObject;
            var obj = ComponentSearcher.Instance.Get<IPooledUnityObject>(gameObject);
            _activeInstancesByGameObject[gameObject] = obj;
            obj.OnSpawn();
        }

        protected override void OnRecycle(object instance)
        {
            var gameObject = ((Component)instance).gameObject;
            var obj = ComponentSearcher.Instance.Get<IPooledUnityObject>(gameObject);
            _activeInstancesByGameObject.Remove(gameObject);
            obj.OnRecycle();
        }

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
